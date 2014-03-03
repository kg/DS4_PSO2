using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Squared.DualShock4;

namespace DS4_PSO2 {
    public partial class MainWindow : Form {
        DualShock4 CurrentDualShock = null;
        Thread UpdaterThread;

        public MainWindow () {
            InitializeComponent();

            UpdaterThread = new Thread(UpdaterThreadFunc);
            UpdaterThread.IsBackground = true;
            UpdaterThread.Priority = ThreadPriority.AboveNormal;
            UpdaterThread.Name = "DualShock 4 state monitor";
            UpdaterThread.Start();
        }

        private void UpdaterThreadFunc () {
            while (true) {
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
                }

                // DS4 sends updates every ~4 milliseconds. This will probably sleep too long, but
                //  Thread.Sleep(0) doesn't sleep long enough.
                // In practice as long as we don't sleep for more than 15ms, this should be fast enough
                //  to pump a new update into the joystick for a game.
                Thread.Sleep(1);
            }
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

                    var sliders = new[] { tbGyroX, tbGyroY, tbGyroZ, tbAccelX, tbAccelY, tbAccelZ };
                    for (var i = 0; i < sliders.Length; i++) {
                        try {
                            sliders[i].Value = (int)Math.Round(CurrentDualShock.Sensors[i] * 10);
                        } catch {
                        }
                    }
                }
            }
        }

        private void UpdateVirtualJoystick () {
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
    }
}
