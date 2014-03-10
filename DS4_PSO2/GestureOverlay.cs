using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Squared.DualShock4;

namespace DS4_PSO2 {
    public partial class GestureOverlay : Form {
        [DllImportAttribute("user32.dll")]
        public extern static IntPtr GetDC (IntPtr handle);

        [DllImportAttribute("user32.dll", ExactSpelling = true)]
        public extern static int ReleaseDC (IntPtr handle, IntPtr hDC);

        [DllImportAttribute("gdi32.dll")]
        public extern static IntPtr CreateCompatibleDC (IntPtr hDC);

        [DllImportAttribute("gdi32.dll")]
        public extern static bool DeleteDC (IntPtr hdc);

        [DllImportAttribute("gdi32.dll")]
        public extern static IntPtr SelectObject (IntPtr hDC, IntPtr hObject);

        [DllImportAttribute("gdi32.dll")]
        public extern static bool DeleteObject (IntPtr hObject);

        [DllImportAttribute("gdi32.dll")]
        public extern static IntPtr CreateCompatibleBitmap (IntPtr hDC, int width, int height);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        static extern bool UpdateLayeredWindow (IntPtr hwnd, IntPtr hdcDst,
           ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pptSrc, uint crKey,
           [In] ref BLENDFUNCTION pblend, uint dwFlags);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateDIBSection (IntPtr hdc, [In] ref BITMAPINFO pbmi,
            uint pila, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos (IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO {
          public Int32 biSize;
          public Int32 biWidth;
          public Int32 biHeight;
          public Int16 biPlanes;
          public Int16 biBitCount;
          public Int32 biCompression;
          public Int32 biSizeImage;
          public Int32 biXPelsPerMeter;
          public Int32 biYPelsPerMeter;
          public Int32 biClrUsed;
          public Int32 biClrImportant;
        }

        struct BLENDFUNCTION {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        struct TextEntry {
            public DateTime When;
            public string Text;
        }

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        const int ULW_ALPHA = 2;
        const byte AC_SRC_OVER = 0;
        const byte AC_SRC_ALPHA = 1;

        const UInt32 SWP_NOSIZE = 0x01;
        const UInt32 SWP_NOMOVE = 0x02;

        IntPtr ScreenDC, BitmapDC;
        IntPtr BackingBitmapBits;
        IntPtr BackingBitmapHandle;
        Bitmap BackingBitmap;
        Graphics BackingGraphics;

        const int HistoryLength = 20;
        const int TextHistoryLength = 7;
        const int TextFadeTimeMs = 1500;
        readonly List<DualShock4Touchpad.TouchInfo> TouchHistory = new List<DualShock4Touchpad.TouchInfo>();
        readonly List<TextEntry> TextHistory = new List<TextEntry>(); 

        public GestureOverlay () {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override CreateParams CreateParams {
            get { 
                var p = base.CreateParams;

                const int WS_POPUP = -2147483648; // 0x80000000
                const int WS_EX_TOOLWINDOW = 0x00000080;
                const int WS_EX_LAYERED = 0x80000;
                const int WS_EX_TRANSPARENT = 0x20;
                const int WS_EX_TOPMOST = 0x00000008;

                p.Style |= WS_POPUP;
                p.ExStyle |= WS_EX_TOOLWINDOW | WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOPMOST;

                return p;
            }
        }

        protected override void CreateHandle() {
            base.CreateHandle();

            var bi = new BITMAPINFO {
                biSize = Marshal.SizeOf(typeof(BITMAPINFO)),
                biWidth = ClientSize.Width,
                biHeight = -ClientSize.Height,
                biPlanes = 1,
                biBitCount = 32,
                biCompression = 0
            };

            ScreenDC = GetDC(IntPtr.Zero);
            BitmapDC = CreateCompatibleDC(ScreenDC);
            BackingBitmapHandle = CreateDIBSection(BitmapDC, ref bi, 0, out BackingBitmapBits, IntPtr.Zero, 0);
            SelectObject(BitmapDC, BackingBitmapHandle);
            BackingBitmap = new Bitmap(ClientSize.Width, ClientSize.Height, 4 * ClientSize.Width, PixelFormat.Format32bppPArgb, BackingBitmapBits);
            BackingGraphics = Graphics.FromImage(BackingBitmap);

            BackingGraphics.CompositingMode = CompositingMode.SourceOver;
            BackingGraphics.CompositingQuality = CompositingQuality.GammaCorrected;
            BackingGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            BackingGraphics.SmoothingMode = SmoothingMode.HighQuality;
            BackingGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            BeginInvoke((Action)Repaint);
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Dispose();

            Repaint();
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            e.Dispose();
        }

        public void Repaint () {
            if (BackingGraphics == null)
                return;

            BackingGraphics.Clear(Color.Transparent);

            using (var pen = new Pen(Color.FromArgb(31, Color.Black), 2))
                BackingGraphics.DrawRectangle(pen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);

            if (TouchHistory.Count > 2) {
                for (var i = 1; i < TouchHistory.Count; i++) {
                    var prior = TouchHistory[i - 1];
                    var current = TouchHistory[i];

                    var priorAlpha = ((i - 1) * 255 / (TouchHistory.Count - 1));
                    var currentAlpha = (i * 255 / (TouchHistory.Count - 1));

                    if (prior.IsActive && current.IsActive) {
                        var p1 = new PointF(prior.X / 5f, prior.Y / 5f);
                        var p2 = new PointF(current.X / 5f, current.Y / 5f);

                        using (var brush = new LinearGradientBrush(new PointF(0, 0), new PointF(1, 1), Color.FromArgb(priorAlpha, Color.White), Color.FromArgb(currentAlpha, Color.White)))
                        using (var pen = new Pen(brush, 3.5f))
                            BackingGraphics.DrawLine(pen, p1, p2);
                    }
                }

                var last = TouchHistory[TouchHistory.Count - 1];
                if (last.IsActive) {
                    using (var pen = new Pen(Color.White, 2f))
                        BackingGraphics.DrawLine(pen, last.StartX / 5f, last.StartY / 5f, last.X / 5f, last.Y / 5f);

                    using (var brush = new SolidBrush(Color.White))
                        BackingGraphics.FillEllipse(brush, (last.X / 5f) - 3f, (last.Y / 5f) - 3f, 6f, 6f);
                }
            }

            if (TextHistory.Count > 0) {
                float y2 = ClientSize.Height;

                var now = DateTime.UtcNow;

                foreach (var te in TextHistory) {
                    var size = BackingGraphics.MeasureString(te.Text, Font);
                    y2 -= size.Height;

                    int alpha = (int)(((now - te.When).TotalMilliseconds * 255) / TextFadeTimeMs);
                    if (alpha < 0)
                        alpha = 0;
                    else if (alpha > 255)
                        alpha = 255;

                    using (var shadowBrush = new SolidBrush(Color.FromArgb( 255 - alpha, Color.Black)))
                    using (var brush = new SolidBrush(Color.FromArgb(255 - alpha, Color.White))) {
                        BackingGraphics.DrawString(te.Text, Font, shadowBrush, 1, y2 + 1);
                        BackingGraphics.DrawString(te.Text, Font, brush, 0, y2);
                    }
                }
            }

            FlushGraphicsToScreen();
        }

        private void FlushGraphicsToScreen () {
            var ps = Screen.PrimaryScreen;
            var pDest = new Point(ps.WorkingArea.Right - ClientSize.Width, ps.WorkingArea.Bottom - ClientSize.Height);
            var pSource = new Point();
            var pSize = ClientSize;
            var blendFunction = new BLENDFUNCTION {
                AlphaFormat = AC_SRC_ALPHA,
                BlendFlags = 0,
                BlendOp = AC_SRC_OVER,
                SourceConstantAlpha = 255
            };

            BackingGraphics.Flush();

            UpdateLayeredWindow(
                Handle, ScreenDC, ref pDest, ref pSize, 
                BitmapDC, ref pSource, 
                0, ref blendFunction, ULW_ALPHA
            );
        }

        public void Update (DualShock4 controller, string latestText) {
            TouchHistory.Add(controller.Touchpad[0]);

            if (TouchHistory.Count > HistoryLength)
                TouchHistory.RemoveAt(0);

            if (latestText != null) {
                TextHistory.Insert(0, new TextEntry {
                    When = DateTime.UtcNow,
                    Text = latestText
                });

                if (TextHistory.Count > TextHistoryLength)
                    TextHistory.RemoveAt(TextHistory.Count - 1);
            }

            Repaint();
        }

        private void GestureOverlay_Resize (object sender, EventArgs e) {
            if (WindowState != FormWindowState.Normal)
                WindowState = FormWindowState.Normal;

            Repaint();
        }

        private void TopmostHackTimer_Tick (object sender, EventArgs e) {
            // Topmost is totally busted on Windows - I think maybe a bad interaction
            //  with other windows that have the flag set, like fullscreen games?

            SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        protected override bool ShowWithoutActivation {
            get { return true; }
        }
    }
}
