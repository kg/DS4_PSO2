using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DS4_PSO2 {
    public static class VJoy {
        public enum VjdStat {
            VJD_STAT_OWN, 
            VJD_STAT_FREE,
            VJD_STAT_BUSY,
            VJD_STAT_MISS,
            VJD_STAT_UNKN,
        }

        public enum HID_USAGES {
            HID_USAGE_X = 48,
            HID_USAGE_Y,
            HID_USAGE_Z,
            HID_USAGE_RX,
            HID_USAGE_RY,
            HID_USAGE_RZ,
            HID_USAGE_SL0,
            HID_USAGE_SL1,
            HID_USAGE_WHL,
            HID_USAGE_POV
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct JoystickState {
            public byte bDevice;
            public int Throttle;
            public int Rudder;
            public int Aileron;
            public int AxisX;
            public int AxisY;
            public int AxisZ;
            public int AxisXRot;
            public int AxisYRot;
            public int AxisZRot;
            public int Slider;
            public int Dial;
            public int Wheel;
            public int AxisVX;
            public int AxisVY;
            public int AxisVZ;
            public int AxisVBRX;
            public int AxisVBRY;
            public int AxisVBRZ;
            public uint Buttons;
            public uint bHats;
            public uint bHatsEx1;
            public uint bHatsEx2;
            public uint bHatsEx3;
        }

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool vJoyEnabled ();

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern short GetvJoyVersion ();

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetvJoyProductString ();

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetvJoyManufacturerString ();

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetvJoySerialNumberString ();

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetVJDStatus (uint id);

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AcquireVJD (uint id);

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RelinquishVJD (uint id);

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool UpdateVJD (uint id, ref JoystickState state);

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetVJDButtonNumber (uint id);

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetVJDDiscPovNumber (uint id);

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetVJDContPovNumber (uint id);

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetVJDAxisExist (uint id, uint axis);

        /* ALL THESE ARE made public */
        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ResetVJD (uint id);

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ResetAll ();

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ResetButtons (uint id);

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ResetPovs (uint id);

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetAxis (int value, uint id, uint axisIndex);

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetBtn (bool value, uint id, uint buttonIndex);

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetDiscPov (int value, uint id, uint povIndex);

        [DllImport("vJoyInterface.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetContPov (int value, uint id, uint povIndex);

        public static bool TryAcquire (uint id) {
            try {
                if (!vJoyEnabled())
                    return false;

                return AcquireVJD(id);
            } catch (EntryPointNotFoundException) {
                // DLL not found
                return false;
            }
        }
    }
}
