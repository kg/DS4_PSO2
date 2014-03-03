using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Squared.DualShock4;

namespace DS4_PSO2 {
    public partial class MainWindow : Form {
        DualShock4 CurrentDualShock = null;

        public MainWindow () {
            InitializeComponent();
        }

        private void tmrUpdate_Tick (object sender, EventArgs e) {
            if (CurrentDualShock != null) {
                if (CurrentDualShock.TryUpdate()) {
                    txtDualShock4State.Text = String.Format(
                        "Axes:\r\n{0}\r\nDPad:\r\n{1}\r\nButtons:\r\n{2}\r\n{3}",
                        CurrentDualShock.Axes,
                        CurrentDualShock.DPad,
                        CurrentDualShock.Buttons,
                        CurrentDualShock.Touchpad
                    );

                    if (chkJoystickEnabled.Checked)
                        UpdateVirtualJoystick();
                } else {
                    // If updating the state of the controller failed, we take this
                    //  as an indication that the controller is disconnected.
                    // We can't use HidDevice.IsConnected because it gets chatty with
                    //  the plug&play service for no good reason.
                    // Don't put expensive operations in property getters, kids.
                    CurrentDualShock.Dispose();
                    CurrentDualShock = null;
                }
            }

            if (CurrentDualShock == null) {
                txtDualShock4Status.Text = "Not connected";
            } else {
                txtDualShock4Status.Text = "Connected";
            }
        }

        private void UpdateVirtualJoystick () {
        }

        private void tmrEnumerate_Tick (object sender, EventArgs e) {
            // Most of the HidDevice methods call Enumerate behind the scenes,
            //  which talks to the plug&play service, devouring CPU time.
            // As a result, we only look for a new controller here when none is available,
            //  and we do this at a slow rate.

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
    }
}
