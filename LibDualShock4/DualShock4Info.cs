using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HidLibrary;

namespace Squared.DualShock4 {
    public struct DualShock4Info : IDisposable {
        public readonly HidDevice Device;

        private DualShock4Info (HidDevice device) {
            Device = device;
        }

        public void Dispose () {
            if (Device != null)
                Device.Dispose();
        }

        public static DualShock4Info[] Enumerate () {
            var devices = HidDevices.Enumerate(0x054C, new int[] { 0x05C4 }).ToArray();
            return (from hidd in devices
                    select new DualShock4Info(hidd)).ToArray();
        }
    }
}
