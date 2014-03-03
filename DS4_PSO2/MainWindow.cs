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
                    txtDualShock4State.Text = "<state>";

                    if (chkJoystickEnabled.Checked)
                        UpdateVirtualJoystick();
                } else {
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
