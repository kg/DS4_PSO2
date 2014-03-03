using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HidLibrary;

namespace Squared.DualShock4 {
    public class DualShock4 : IDisposable {
        private readonly DualShock4Info Info;

        public DualShock4 (DualShock4Info info) {
            Info = info;
        }

        public HidDevice Device {
            get {
                return Info.Device;
            }
        }

        public bool TryUpdate () {
            var buffer = new byte[64];

            if (Device.ReadFile(buffer) != HidDevice.ReadStatus.Success)
                return false;

            return true;
        }

        public void Dispose () {
            Info.Dispose();
        }
    }
}
