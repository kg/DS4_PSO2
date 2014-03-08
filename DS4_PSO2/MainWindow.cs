using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Squared.DualShock4;

namespace DS4_PSO2 {
    public partial class MainWindow : Form {
        GestureOverlay GestureOverlay;

        uint? JoystickToUse = null;
        DualShock4 CurrentDualShock = null;
        Thread UpdaterThread;
        bool JoystickFailed;

        private readonly TrackBar[] SliderBySensorIndex;

        private static readonly Dictionary<DualShock4Direction, uint> DPadMapping = new Dictionary<DualShock4Direction, uint> {
            { DualShock4Direction.Neutral, 0xFFFFFFFF },
            { DualShock4Direction.Up, 0 },
            { DualShock4Direction.UpRight, 4500 },
            { DualShock4Direction.Right, 9000 },
            { DualShock4Direction.DownRight, 13500 },
            { DualShock4Direction.Down, 18000 },
            { DualShock4Direction.DownLeft, 22500 },
            { DualShock4Direction.Left, 27000 },
            { DualShock4Direction.UpLeft, 31500 }
        };

        const float AxisThreshold = 0.4f;
        const float MinimumGestureLength = 60f;
        const float GestureAngleDeadzoneDegrees = 32f;
        const float GesturePressDuration = 75f;
        const float GestureConfirmDelay = 900f;

        const DualShock4Button GestureConfirmButton = DualShock4Button.Circle;

        const int GestureButtonBase = 12;
        const int AfterGestureButtonBase = 16;

        string MostRecentGestureText = null;
        DateTime MostRecentGestureTime;
        bool GestureConfirmActive;

        public MainWindow () {
            InitializeComponent();

            UpdaterThread = new Thread(UpdaterThreadFunc);
            UpdaterThread.IsBackground = true;
            UpdaterThread.Priority = ThreadPriority.AboveNormal;
            UpdaterThread.Name = "DualShock 4 state monitor";
            UpdaterThread.Start();

            SliderBySensorIndex = new[] { tbGyroX, tbGyroY, tbGyroZ, tbAccelX, tbAccelY, tbAccelZ };

            GestureOverlay = new GestureOverlay();
        }

        private void UpdaterThreadFunc () {
            var previousJoystick = JoystickToUse;
            var gestureTimes = new DateTime[4];

            while (true) {
                uint? activeJoystick;

                // Lock the main window to guard its state
                lock (this) {
                    if (CurrentDualShock != null) {
                        if (!CurrentDualShock.TryUpdate()) {
                            // If updating the state of the controller failed, we take this
                            //  as an indication that the controller is disconnected.
                            // We can't use HidDevice.IsConnected because it gets chatty with
                            //  the plug&play service for no good reason.
                            // Don't put expensive operations in property getters, kids.
                            CurrentDualShock.Dispose();
                            CurrentDualShock = null;
                        }
                    }

                    activeJoystick = JoystickToUse;
                }

                if (previousJoystick != activeJoystick) {
                    if (previousJoystick.HasValue) {
                        DeInitJoystick(previousJoystick.Value);
                        Console.WriteLine("Released joystick #{0}", previousJoystick.Value);
                    }

                    if (activeJoystick.HasValue) {
                        if (!InitJoystick(activeJoystick.Value)) {
                            Console.WriteLine("Could not initialize joystick #{0}", activeJoystick.Value);
                            activeJoystick = null;
                            JoystickFailed = true;
                        } else {
                            Console.WriteLine("Initialized joystick #{0}", activeJoystick.Value);
                        }
                    }
                }

                if (CurrentDualShock != null) {
                    HandleGestures(gestureTimes);
                }

                if (activeJoystick.HasValue && (CurrentDualShock != null))
                    UpdateJoystick(activeJoystick.Value, gestureTimes);

                previousJoystick = activeJoystick;

                // DS4 sends updates every ~4 milliseconds. This will probably sleep too long, but
                //  Thread.Sleep(0) doesn't sleep long enough.
                // In practice as long as we don't sleep for more than 15ms, this should be fast enough
                //  to pump a new update into the joystick for a game.
                Thread.Sleep(15);
            }
        }

        private void DeInitJoystick (uint id) {
            VJoy.RelinquishVJD(id);
        }

        private bool InitJoystick (uint id) {
            return VJoy.TryAcquire(id);
        }

        private static int RescaleSignedAxis (float value) {
            return (int)(((value + 1f) / 2f) * 32767f);
        }

        private static int RescaleUnsignedAxis (float value) {
            return (int)(value * 32767f);
        }

        private static void SetButton (ref uint buttons, int index, bool state) {
            uint mask = (1u << index);
            uint maskedState = state ? (0xFFFFFFFFu & mask) : 0;

            buttons = (buttons & ~mask) | maskedState;
        }

        private void SetButton (ref uint buttons, int index, DualShock4Button button) {
            var state = CurrentDualShock.Buttons[button];
            if (button == GestureConfirmButton)
                state |= GestureConfirmActive;

            SetButton(ref buttons, index, state);
        }

        private void HandleGestures (DateTime[] gestureTimes) {
            var now = DateTime.UtcNow;
            var previous = CurrentDualShock.Touchpad.GetPreviousState(0);
            var current = CurrentDualShock.Touchpad[0];

            // We might miss swipes if our refresh interval is too slow
            if (previous.IsActive && !current.IsActive) {
                // FIXME: Read it from previous instead?
                var deltaX = current.X - current.StartX;
                var deltaY = current.Y - current.StartY;
                var gestureLength = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                var gestureAngle = (float)Math.Atan2(deltaY, deltaX) * (float)(180 / Math.PI);

                if (gestureAngle < 0)
                    gestureAngle += 360;

                var possibleGestureAngles = new[] { 0f, 90f, 180f, 270f, 360f };
                var gestureAngleNames = new[] { "Right", "Down", "Left", "Up" };
                int? mappedAngle = null;
                float mappedAngleDistance = 99999;

                for (var i = 0; i < possibleGestureAngles.Length; i++) {
                    var angle = possibleGestureAngles[i];
                    var angleDistance = (float)Math.Abs(gestureAngle - angle);

                    if ((angleDistance < GestureAngleDeadzoneDegrees) && (angleDistance < mappedAngleDistance)) {
                        if (i == 4)
                            mappedAngle = 0;
                        else
                            mappedAngle = i;

                        mappedAngleDistance = angleDistance;
                        // Debug.WriteLine("Matched gesture angle of {0:000.00} degrees to angle of {1:000.00} degrees", gestureAngle, angle);
                    }
                }

                var longEnough = (gestureLength >= MinimumGestureLength);

                if (mappedAngle.HasValue && longEnough) {
                    Console.WriteLine("Swipe at angle {0:000.0} mapped to index {1}", gestureAngle, mappedAngle.Value);

                    gestureTimes[mappedAngle.Value] = now;
                    MostRecentGestureTime = now;
                    MostRecentGestureText = gestureAngleNames[mappedAngle.Value];
                } else {
                    Console.WriteLine(
                        "Touch went from {0:0000},{1:0000} to {2:0000},{3:0000} (length {4})",
                        current.StartX, current.StartY,
                        current.X, current.Y,
                        gestureLength                    
                    );

                    if (longEnough)
                        MostRecentGestureText = String.Format("Unknown ({0:000.0} degrees)", gestureAngle);
                    else
                        MostRecentGestureText = String.Format("Too short ({0:000.0} px)", gestureLength);
                }
            }
        }

        private bool UpdateJoystick (uint id, DateTime[] gestureTimes) {
            // FIXME: It'd be faster to use the UpdateVJD function that takes a bitpacked blob,
            //  but I'm too lazy to get that right currently.

            var now = DateTime.UtcNow;

            VJoy.JoystickState state = default(VJoy.JoystickState);

            state.bDevice = (byte)id;
            state.AxisX = RescaleSignedAxis(CurrentDualShock.Axes[DualShock4Axis.LeftStickX]);
            state.AxisY = RescaleSignedAxis(CurrentDualShock.Axes[DualShock4Axis.LeftStickY]);
            state.AxisXRot = RescaleSignedAxis(CurrentDualShock.Axes[DualShock4Axis.RightStickX]);
            state.AxisYRot = RescaleSignedAxis(CurrentDualShock.Axes[DualShock4Axis.RightStickY]);
            state.AxisZRot = RescaleUnsignedAxis(CurrentDualShock.Axes[DualShock4Axis.L2]);
            state.Slider = RescaleUnsignedAxis(CurrentDualShock.Axes[DualShock4Axis.R2]);

            uint buttons = 0;
            TimeSpan delta;

            for (var i = 0; i < gestureTimes.Length; i++) {
                delta = now - gestureTimes[i];

                // We hold a virtual button for a set number of milliseconds after a gesture
                SetButton(
                    ref buttons, GestureButtonBase + i,
                    (delta.TotalMilliseconds < GesturePressDuration)
                );
            }

            // Then shortly after the gesture button press we press a confirmation button
            // If you perform the same gesture again, it resets the timer for the confirmation
            //  so that you can swipe multiple times before one confirmation happens
            delta = (now - MostRecentGestureTime);

            GestureConfirmActive = (delta.TotalMilliseconds >= GestureConfirmDelay) &&
                (delta.TotalMilliseconds <= GestureConfirmDelay + GesturePressDuration);

            SetButton(ref buttons, 0, DualShock4Button.Square);
            SetButton(ref buttons, 1, DualShock4Button.Cross);
            SetButton(ref buttons, 2, DualShock4Button.Circle);
            SetButton(ref buttons, 3, DualShock4Button.Triangle);

            SetButton(ref buttons, 4, DualShock4Button.L1);
            SetButton(ref buttons, 5, DualShock4Button.R1);
            SetButton(ref buttons, 6, DualShock4Button.Share);
            SetButton(ref buttons, 7, DualShock4Button.Options);
            SetButton(ref buttons, 8, DualShock4Button.L3);
            SetButton(ref buttons, 9, DualShock4Button.R3);

            SetButton(ref buttons, 10, (CurrentDualShock.Axes[DualShock4Axis.L2] > AxisThreshold));
            SetButton(ref buttons, 11, (CurrentDualShock.Axes[DualShock4Axis.R2] > AxisThreshold));

            SetButton(ref buttons, AfterGestureButtonBase, DualShock4Button.PS);
            SetButton(ref buttons, AfterGestureButtonBase + 1, DualShock4Button.TouchpadClick);

            state.Buttons = buttons;
            state.bHats = DPadMapping[CurrentDualShock.DPad];

            if (!VJoy.UpdateVJD(id, ref state))
                return false;

            return true;
        }

        private void tmrUpdate_Tick (object sender, EventArgs e) {
            if ((this.WindowState == FormWindowState.Minimized) || (!this.Visible)) {
                this.ShowInTaskbar = false;
                niTrayIcon.Visible = true;
                return;
            } else {
                niTrayIcon.Visible = false;
            }

            lock (this) {
                if (JoystickFailed) {
                    JoystickFailed = false;
                    chkJoystickEnabled.Checked = false;
                }

                if (chkJoystickEnabled.Checked)
                    JoystickToUse = (uint)nudJoystickNumber.Value;
                else
                    JoystickToUse = null;

                if (CurrentDualShock == null) {
                    txtDualShock4Status.Text = "Not connected";
                } else {
                    txtDualShock4Status.Text = "Connected";

                    txtDualShock4State.Text = String.Format(
                        "Axes:\r\n{0}\r\nDPad: {1}\r\nButtons:\r\n{2}\r\n{3}\r\n{4}",
                        CurrentDualShock.Axes,
                        CurrentDualShock.DPad,
                        CurrentDualShock.Buttons,
                        CurrentDualShock.Touchpad,
                        CurrentDualShock.Sensors
                    );

                    for (var i = 0; i < SliderBySensorIndex.Length; i++) {
                        try {
                            SliderBySensorIndex[i].Value = (int)Math.Round(CurrentDualShock.Sensors[i] * 10);
                        } catch {
                        }
                    }
                }

                if (GestureOverlay.Visible && (CurrentDualShock != null)) {
                    GestureOverlay.Update(CurrentDualShock, MostRecentGestureText);
                    MostRecentGestureText = null;
                }
            }
        }

        private void tmrEnumerate_Tick (object sender, EventArgs e) {
            // Most of the HidDevice methods call Enumerate behind the scenes,
            //  which talks to the plug&play service, devouring CPU time.
            // As a result, we only look for a new controller here when none is available,
            //  and we do this at a slow rate.

            lock (this)
            if (CurrentDualShock == null) {
                var dualshocks = DualShock4Info.Enumerate();

                if (dualshocks.Length > 0)
                    CurrentDualShock = new DualShock4(dualshocks[0]);

                foreach (var info in dualshocks) {
                    if ((CurrentDualShock == null) || !Object.ReferenceEquals(info.Device, CurrentDualShock.Device))
                        info.Dispose();
                }
            }
        }

        private void btnConfigureJoystick_Click (object sender, EventArgs e) {
            var descriptor = new byte[] { 
                0x05, 0x01, 0x15, 0x00, 0x09, 0x04, 0xa1, 0x01, 0x05, 0x01, 0x85, 0x01, 0x09, 0x01, 0x15, 0x00, 0x26,
                0xff, 0x7f, 0x75, 0x20, 0x95, 0x01, 0xa1, 0x00, 0x09, 0x30, 0x81, 0x02, 0x09, 0x31, 0x81, 0x02, 0x81, 0x01, 0x09, 0x33, 0x81, 0x02, 0x09, 0x34, 0x81,
                0x02, 0x09, 0x35, 0x81, 0x02, 0x09, 0x36, 0x81, 0x02, 0x81, 0x01, 0xc0, 0x15, 0x00, 0x27, 0x3c, 0x8c, 0x00, 0x00, 0x35, 0x00, 0x47, 0x3c, 0x8c, 0x00,
                0x00, 0x65, 0x14, 0x75, 0x20, 0x95, 0x01, 0x09, 0x39, 0x81, 0x02, 0x95, 0x03, 0x81, 0x01, 0x05, 0x09, 0x15, 0x00, 0x25, 0x01, 0x55, 0x00, 0x65, 0x00,
                0x19, 0x01, 0x29, 0x16, 0x75, 0x01, 0x95, 0x16, 0x81, 0x02, 0x75, 0x0a, 0x95, 0x01, 0x81, 0x01, 0xc0
            };
            var descriptorSize = 0x6d;

            if (descriptor.Length != descriptorSize)
                throw new InvalidDataException("what");

            var deviceKey = String.Format("Device{0:00}", nudJoystickNumber.Value);

            // Write configuration into registry
            using (var vjoy = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\vjoy", true))
            using (var parameters = vjoy.CreateSubKey("Parameters"))
            using (var device = parameters.CreateSubKey(deviceKey)) {
                device.SetValue("HidReportDesctiptor", descriptor, RegistryValueKind.Binary);
                device.SetValue("HidReportDesctiptorSize", descriptorSize, RegistryValueKind.DWord);
            }

            var assemblyPath = GetPathOfAssembly(Assembly.GetExecutingAssembly());
            var assemblyFolder = Path.GetDirectoryName(assemblyPath);

            // Force restart vjoy device(s) to apply configuration
            {
                var psi = new ProcessStartInfo(
                    Path.Combine(assemblyFolder, "devcon", "devcon-x86.exe"), 
                    @"restart ""root\VID_1234&PID_BEAD&REV_0202"
                ) {
                };

                using (var process = Process.Start(psi))
                    ;
            }

            {
                var psi = new ProcessStartInfo(
                    Path.Combine(assemblyFolder, "devcon", "devcon-x64.exe"),
                    @"restart ""root\VID_1234&PID_BEAD&REV_0202"
                ) {
                };

                using (var process = Process.Start(psi))
                    ;
            }
        }

        private static string GetPathOfAssembly (Assembly assembly) {
            var uri = new Uri(assembly.CodeBase);
            var result = Uri.UnescapeDataString(uri.AbsolutePath);

            if (String.IsNullOrWhiteSpace(result))
                result = assembly.Location;

            result = result.Replace('/', System.IO.Path.DirectorySeparatorChar);

            return result;
        }

        private void MainWindow_FormClosing (object sender, FormClosingEventArgs e) {
            niTrayIcon.Visible = false;
        }

        private void exitToolStripMenuItem_Click (object sender, EventArgs e) {
            niTrayIcon.Visible = false;
            this.Close();
        }

        private void showToolStripMenuItem_Click (object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void niTrayIcon_MouseDown (object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void DoUpdateCheck () {
            ThreadPool.QueueUserWorkItem((_) => {
                try {
                    using (var wc = new WebClient()) {
                        var versionString = wc.DownloadString("http://luminance.org/ds4pso2/latest_version.txt");
                        MessageBox.Show(versionString);
                        var parsedVersion = Version.Parse(versionString);
                        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

                        if (parsedVersion > currentVersion) {
                            BeginInvoke(
                                (Action)(() => ShowUpdateNotice(parsedVersion, currentVersion))
                            );
                        }
                    }
                } catch (Exception exc) {
                    BeginInvoke(
                        (Action)(() => ShowUpdateFailure(exc))
                    );
                }
            });
        }

        private void ShowUpdateNotice (Version newVersion, Version currentVersion) {
            var choice = MessageBox.Show(
                this, String.Format(
                    "A new version is available. You are running {0} and the latest version is {1}. Would you like to download the new version?",
                    currentVersion, newVersion
                ), "Update available", MessageBoxButtons.YesNo
            );

            if (choice == DialogResult.Yes)
                Process.Start("https://github.com/kg/DS4_PSO2/wiki/Downloads");
        }

        private void ShowUpdateFailure (Exception exception) {
            MessageBox.Show(this, exception.ToString(), "Update check failed");
        }

        private void MainWindow_Shown (object sender, EventArgs e) {
            // DoUpdateCheck();
        }

        private void GestureOverlayMode_CheckedChanged (object sender, EventArgs e) {
            UpdateGestureOverlay();
        }

        private void UpdateGestureOverlay () {
            if (NoGestureOverlay.Checked)
                GestureOverlay.Hide();
            else
                GestureOverlay.Show();
        }
    }
}
