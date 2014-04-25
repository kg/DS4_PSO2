using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Squared.DualShock4;

namespace DS4_PSO2 {
    public partial class MainWindow : Form {
        // The minimum distance (in touchpanel pixels) a swipe must go in order
        //  to be considered a gesture.
        const float MinimumGestureLength = 80f;
        // The distance the swipe angle can be from one of the cardinal angles
        //  used by the gesture system (in degrees) and still be considered valid.
        const float GestureAngleRangeDegrees = 32f;
        // How long fake button presses last.
        const float GesturePressDuration = 60f;
        // The delay before an automatic confirmation is triggered when you are
        //  using individual swipes to scroll.
        const float GestureConfirmDelay = 950f;
        // How long you have to hold a touch before it begins repeating.
        const float GestureRepeatStartDelay = 500f;
        // How long elapses between repeats.
        const float GestureRepeatInterval = 330f;

        // Delay before auto-closing the overlay (in milliseconds)
        const int OverlayAutoCloseDelay = 300;

        // The interval (in milliseconds) between joystick updates.
        const int UpdateInterval = 5;

        // What button is used for gesture confirmations.
        // You can set this to null in order to disable confirmations.
        static readonly DualShock4Button? GestureConfirmButton = DualShock4Button.Circle;

        // Auto-fire support
        static readonly Dictionary<DualShock4Button, DateTime> ButtonPressedWhen = new Dictionary<DualShock4Button, DateTime>();
        static readonly Dictionary<DualShock4Button, int> AutoFireButtons = new Dictionary<DualShock4Button, int> {
        };
        const int AutoFireDurationMs = 20;
        const int AutoFireDelayMs = 20;

        // The base index of the buttons used by gestures.
        const int GestureButtonBase = 12;

        const int AfterGestureButtonBase = 16;

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

        GestureOverlay GestureOverlay;

        // The index of the currently active vJoy joystick
        uint? JoystickToUse = null;

        // Indicates that something went wrong with vJoy
        bool JoystickFailed;

        // The currently active DualShock4 controller
        DualShock4 CurrentDualShock = null;

        Thread UpdaterThread;

        // Gesture system state
        string MostRecentGestureText;
        DateTime MostRecentTouchUpTime, MostRecentHeldTouchUpTime;
        bool GestureConfirmActive;
        bool ShownConfirmMessage;

        public MainWindow () {
            InitializeComponent();

            UpdaterThread = new Thread(UpdaterThreadFunc);
            UpdaterThread.IsBackground = true;
            UpdaterThread.Priority = ThreadPriority.AboveNormal;
            UpdaterThread.Name = "DualShock 4 state monitor";
            UpdaterThread.Start();

            SliderBySensorIndex = new[] { tbGyroX, tbGyroY, tbGyroZ, tbAccelX, tbAccelY, tbAccelZ };

            GestureOverlay = new GestureOverlay();
            niTrayIcon.Visible = true;

            RefreshProfileList();
        }

        private void RefreshProfileList () {
            cbProfileName.Items.Clear();

            foreach (var profile in Directory.GetFiles(ProfilesFolder))
                cbProfileName.Items.Add(Path.GetFileNameWithoutExtension(profile));
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
                Thread.Sleep(UpdateInterval);
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

        private static bool GetButton (uint buttons, int index) {
            uint mask = (1u << index);
            return (buttons & mask) != 0;
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
            var invalidGesture = true;

            // We might miss swipes if our refresh interval is too slow
            if (previous.IsActive) {
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
                float mappedAngleDistance = float.MaxValue;

                for (var i = 0; i < possibleGestureAngles.Length; i++) {
                    var angle = possibleGestureAngles[i];
                    var angleDistance = (float)Math.Abs(gestureAngle - angle);

                    if ((angleDistance < GestureAngleRangeDegrees) && (angleDistance < mappedAngleDistance)) {
                        if (i == 4)
                            mappedAngle = 0;
                        else
                            mappedAngle = i;

                        mappedAngleDistance = angleDistance;
                        // Debug.WriteLine("Matched gesture angle of {0:000.00} degrees to angle of {1:000.00} degrees", gestureAngle, angle);
                    }
                }

                var longEnough = (gestureLength >= MinimumGestureLength);
                var gestureAge = current.When - current.StartWhen;

                var swipeEnded = !current.IsActive;
                var repeatActive = gestureAge.TotalMilliseconds >= GestureRepeatStartDelay;

                if (mappedAngle.HasValue && longEnough) {
                    invalidGesture = false;

                    if (swipeEnded) {
                        // Suppress the final swipe if repeat is active. Otherwise, 
                        //  letting off the touch suddenly will trigger a sudden motion.
                        if (!repeatActive) {
                            gestureTimes[mappedAngle.Value] = now;
                            MostRecentGestureText = gestureAngleNames[mappedAngle.Value];
                        }
                    } else if (repeatActive) {
                        var repeatTimeOffset = (gestureAge.TotalMilliseconds - GestureRepeatStartDelay) / GestureRepeatInterval;
                        repeatTimeOffset = GestureRepeatStartDelay + (Math.Floor(repeatTimeOffset) * GestureRepeatInterval);
                        var when = current.StartWhen + TimeSpan.FromMilliseconds(repeatTimeOffset);

                        // We only want to trigger a new gesture if the time doesn't match, so we don't spam the log
                        if (gestureTimes[mappedAngle.Value] != when) {
                            gestureTimes[mappedAngle.Value] = when;
                            MostRecentGestureText = gestureAngleNames[mappedAngle.Value] + " (repeat)";
                            
                            // If there was a pending confirmation press, cancel it as long as repeats are active.
                            MostRecentTouchUpTime = MostRecentHeldTouchUpTime = default(DateTime);
                        }
                    }

                } else if (swipeEnded) {
                    if (longEnough)
                        MostRecentGestureText = String.Format("Unknown ({0:000.0} degrees)", gestureAngle);
                    else
                        MostRecentGestureText = String.Format("Too short ({0:000.0} px)", gestureLength);
                }

                if (swipeEnded && !invalidGesture) {
                    if (repeatActive) {
                        MostRecentHeldTouchUpTime = now;
                    } else {
                        MostRecentTouchUpTime = now;
                    }
                }
            }
        }

        private bool UpdateJoystick (uint id, DateTime[] gestureTimes) {
            var now = DateTime.UtcNow;

            VJoy.JoystickState state = default(VJoy.JoystickState);

            state.bDevice = (byte)id;
            state.AxisX = RescaleSignedAxis(CurrentDualShock.Axes[DualShock4Axis.LeftStickX]);
            state.AxisY = RescaleSignedAxis(CurrentDualShock.Axes[DualShock4Axis.LeftStickY]);
            state.AxisXRot = RescaleSignedAxis(CurrentDualShock.Axes[DualShock4Axis.RightStickX]);
            state.AxisYRot = RescaleSignedAxis(CurrentDualShock.Axes[DualShock4Axis.RightStickY]);

            // Separate trigger axes
            state.AxisZRot = RescaleUnsignedAxis(CurrentDualShock.Axes[DualShock4Axis.L2]);
            state.Slider = RescaleUnsignedAxis(CurrentDualShock.Axes[DualShock4Axis.R2]);

            // XBox 360 DInput style triggers (unified axis)
            var z = 0f +
                (CurrentDualShock.Axes[DualShock4Axis.R2] * -1) +
                (CurrentDualShock.Axes[DualShock4Axis.L2] * 1);
            state.AxisZ = RescaleSignedAxis(z);

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
            delta = (now - MostRecentTouchUpTime);

            GestureConfirmActive = (delta.TotalMilliseconds >= GestureConfirmDelay) &&
                (delta.TotalMilliseconds <= GestureConfirmDelay + GesturePressDuration);

            // If they held the touch long enough for it to repeat a few times, we confirm
            //  immediately after the hold ends.
            delta = (now - MostRecentHeldTouchUpTime);

            GestureConfirmActive |= (delta.TotalMilliseconds >= GesturePressDuration) &&
                (delta.TotalMilliseconds <= (GesturePressDuration * 2));

            if (GestureConfirmActive && GestureConfirmButton.HasValue) {
                if (!ShownConfirmMessage) {
                    ShownConfirmMessage = true;
                    MostRecentGestureText = "Confirm (auto)";
                }
            } else {
                ShownConfirmMessage = false;
            }

            SetButton(ref buttons, 0, DualShock4Button.Cross);
            SetButton(ref buttons, 1, DualShock4Button.Circle);
            SetButton(ref buttons, 2, DualShock4Button.Square);
            SetButton(ref buttons, 3, DualShock4Button.Triangle);

            SetButton(ref buttons, 4, DualShock4Button.L1);
            SetButton(ref buttons, 5, DualShock4Button.R1);
            SetButton(ref buttons, 6, DualShock4Button.Share);
            SetButton(ref buttons, 7, DualShock4Button.Options);
            SetButton(ref buttons, 8, DualShock4Button.L3);
            SetButton(ref buttons, 9, DualShock4Button.R3);

            const float leftAxisThreshold = 0.55f;
            const float rightAxisThreshold = 0.55f;

            SetButton(ref buttons, 10, (CurrentDualShock.Axes[DualShock4Axis.L2] > leftAxisThreshold));
            SetButton(ref buttons, 11, (CurrentDualShock.Axes[DualShock4Axis.R2] > rightAxisThreshold));

            SetButton(ref buttons, AfterGestureButtonBase, DualShock4Button.PS);
            SetButton(ref buttons, AfterGestureButtonBase + 1, DualShock4Button.TouchpadClick);
            
            foreach (var kvp in AutoFireButtons) {
                var autoFireButtonState = CurrentDualShock.Buttons[kvp.Key];
                var buttonState = GetButton(buttons, kvp.Value);

                DateTime pressedWhen;
                if (ButtonPressedWhen.TryGetValue(kvp.Key, out pressedWhen)) {
                    if (autoFireButtonState) {
                        const int modulo = AutoFireDelayMs + AutoFireDurationMs;

                        var pressedDuration = now - pressedWhen;
                        var pressedDurationRounded = pressedDuration.TotalMilliseconds % modulo;

                        if (pressedDurationRounded >= AutoFireDurationMs)
                            autoFireButtonState = false;
                    } else
                        ButtonPressedWhen.Remove(kvp.Key);

                } else {
                    ButtonPressedWhen[kvp.Key] = now;
                }

                SetButton(ref buttons, kvp.Value, autoFireButtonState || buttonState);
            }

            state.Buttons = buttons;
            state.bHats = DPadMapping[CurrentDualShock.DPad];

            return VJoy.UpdateVJD(id, ref state);
        }

        private void tmrUpdate_Tick (object sender, EventArgs e) {
            var hidden = false;
            bool doRepaint = false;

            if ((this.WindowState == FormWindowState.Minimized) || !this.Visible) {
                if (ShowInTaskbar)
                    ShowInTaskbar = false;

                hidden = true;
            } else {
                if (!ShowInTaskbar)
                    ShowInTaskbar = true;

                if (WindowState != FormWindowState.Normal)
                    WindowState = FormWindowState.Normal;
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

                if (!hidden) {
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
                }

                if (ShowGestureOverlay.Checked && (CurrentDualShock != null)) {
                    doRepaint = true;
                    GestureOverlay.Update(CurrentDualShock, MostRecentGestureText);
                    MostRecentGestureText = null;
                } else {
                    if (GestureOverlay.Visible)
                        GestureOverlay.Hide();
                }
            }

            if (doRepaint)
                GestureOverlay.Repaint();

            if (GestureOverlay.IsIdle) {
                if (GestureOverlay.Visible) {
                    var idleDuration = DateTime.UtcNow - GestureOverlay.IdleSince.Value;
                    if (idleDuration.TotalMilliseconds >= OverlayAutoCloseDelay)
                        GestureOverlay.Hide();
                }
            } else {
                if (!GestureOverlay.Visible)
                    GestureOverlay.Show();
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
                    CurrentDualShock = new DualShock4(dualshocks[0], false);

                foreach (var info in dualshocks) {
                    if ((CurrentDualShock == null) || !Object.ReferenceEquals(info.Device, CurrentDualShock.Device))
                        info.Dispose();
                }
            }
        }

        private string AssemblyPath {
            get {
                return GetPathOfAssembly(Assembly.GetExecutingAssembly());
            }
        }

        private string ProfilesFolder {
            get {
                var assemblyFolder = Path.GetDirectoryName(AssemblyPath);

                return Path.Combine(
                    assemblyFolder, "configs"
                );
            }
        }

        private string DevconPath {
            get {
                var assemblyFolder = Path.GetDirectoryName(AssemblyPath);

                return Path.Combine(
                    assemblyFolder, "devcon",
                    Environment.Is64BitOperatingSystem ? "devcon-x64.exe" : "devcon-x86.exe"
                );
            }
        }

        private void btnConfigureJoystick_Click (object sender, EventArgs e) {
            var deviceKey = String.Format("Device{0:00}", nudJoystickNumber.Value);

            var profileSourceText = File.ReadAllText(
                Path.Combine(ProfilesFolder, (string)cbProfileName.SelectedItem + ".reg")
            );

            var profileFixedText = profileSourceText.Replace("\\Device01", "\\" + deviceKey);

            var tempPath = Path.Combine(Path.GetTempPath(), "DS4_PSO2.reg");
            File.WriteAllText(tempPath, profileFixedText);

            // Load configuration from .reg file
            {
                var psi = new ProcessStartInfo(
                    "regedit.exe",
                    "/S " + tempPath
                );

                using (var process = Process.Start(psi))
                    process.WaitForExit();
            }

            // Force restart vjoy device(s) to apply configuration
            {
                var psi = new ProcessStartInfo(
                    DevconPath, 
                    "restart \"root\\VID_1234&PID_BEAD&REV_0203\""
                ) {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                using (var process = Process.Start(psi))
                    process.WaitForExit();
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
            this.Close();
        }

        private void showToolStripMenuItem_Click (object sender, EventArgs e) {
            ShowInTaskbar = true;

            if (WindowState != FormWindowState.Normal)
                WindowState = FormWindowState.Normal;
        }

        private void niTrayIcon_MouseDown (object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ShowInTaskbar = true;

                if (WindowState != FormWindowState.Normal)
                    WindowState = FormWindowState.Normal;
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

        private void cbProfileName_SelectedIndexChanged (object sender, EventArgs e) {
            btnConfigureJoystick.Enabled = (cbProfileName.SelectedIndex >= 0);
        }
    }
}
