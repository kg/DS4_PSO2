using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Squared.DualShock4;

namespace DS4_PSO2 {
    public partial class MainWindow : Form {
        uint? JoystickToUse = null;
        DualShock4 CurrentDualShock = null;
        Thread UpdaterThread;

        private readonly TrackBar[] SliderBySensorIndex;

        private static readonly Dictionary<DualShock4Direction, int> DPadMapping = new Dictionary<DualShock4Direction, int> {
            { DualShock4Direction.Neutral, -1 },
            { DualShock4Direction.Up, 0 },
            { DualShock4Direction.UpRight, 4500 },
            { DualShock4Direction.Right, 9000 },
            { DualShock4Direction.DownRight, 13500 },
            { DualShock4Direction.Down, 18000 },
            { DualShock4Direction.DownLeft, 22500 },
            { DualShock4Direction.Left, 27000 },
            { DualShock4Direction.UpLeft, 31500 }
        };

        public MainWindow () {
            InitializeComponent();

            UpdaterThread = new Thread(UpdaterThreadFunc);
            UpdaterThread.IsBackground = true;
            UpdaterThread.Priority = ThreadPriority.AboveNormal;
            UpdaterThread.Name = "DualShock 4 state monitor";
            UpdaterThread.Start();

            SliderBySensorIndex = new[] { tbGyroX, tbGyroY, tbGyroZ, tbAccelX, tbAccelY, tbAccelZ };
        }

        private void UpdaterThreadFunc () {
            var previousJoystick = JoystickToUse;

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
                        } else {
                            Console.WriteLine("Initialized joystick #{0}", activeJoystick.Value);
                        }
                    }
                }

                if (activeJoystick.HasValue && (CurrentDualShock != null))
                    UpdateJoystick(activeJoystick.Value);

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

        private bool UpdateJoystick (uint id) {
            // FIXME: It'd be faster to use the UpdateVJD function that takes a bitpacked blob,
            //  but I'm too lazy to get that right currently.

            VJoy.JoystickState state = default(VJoy.JoystickState);

            state.bDevice = (byte)id;
            state.AxisX = RescaleSignedAxis(CurrentDualShock.Axes[DualShock4Axis.LeftStickX]);
            state.AxisY = RescaleSignedAxis(CurrentDualShock.Axes[DualShock4Axis.LeftStickY]);
            state.AxisXRot = RescaleSignedAxis(CurrentDualShock.Axes[DualShock4Axis.RightStickX]);
            state.AxisYRot = RescaleSignedAxis(CurrentDualShock.Axes[DualShock4Axis.RightStickY]);
            state.AxisZRot = RescaleUnsignedAxis(CurrentDualShock.Axes[DualShock4Axis.L2]);
            state.Slider = RescaleUnsignedAxis(CurrentDualShock.Axes[DualShock4Axis.R2]);

            if (!VJoy.UpdateVJD(id, ref state))
                return false;

            // UpdateVJD is a broken piece of shit and doesn't read the bHats field at all
            int povHat;
            if (!DPadMapping.TryGetValue(CurrentDualShock.DPad, out povHat))
                povHat = -1;

            if (!VJoy.SetContPov(povHat, id, 1))
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

        private void niTrayIcon_Click (object sender, EventArgs e) {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
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

            using (var vjoy = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\vjoy", true))
            using (var parameters = vjoy.CreateSubKey("Parameters"))
            using (var device = parameters.CreateSubKey(deviceKey)) {
                device.SetValue("HidReportDesctiptor", descriptor, RegistryValueKind.Binary);
                device.SetValue("HidReportDesctiptorSize", descriptorSize, RegistryValueKind.DWord);
            }
        }
    }
}
