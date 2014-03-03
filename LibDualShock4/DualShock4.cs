using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HidLibrary;

namespace Squared.DualShock4 {
    public class DualShock4 : IDisposable {
        private readonly DualShock4Info Info;

        public readonly DualShock4Axes Axes = new DualShock4Axes();
        public readonly DualShock4Buttons Buttons = new DualShock4Buttons();
        public readonly DualShock4Touchpad Touchpad = new DualShock4Touchpad();

        public DualShock4 (DualShock4Info info) {
            Info = info;
        }

        public HidDevice Device {
            get {
                return Info.Device;
            }
        }

        public DualShock4Direction DPad {
            get;
            private set;
        }

        public bool TryUpdate () {
            var buffer = new byte[64];

            if (Device.ReadFile(buffer) != HidDevice.ReadStatus.Success)
                return false;

            Device.flush_Queue();

            Axes.ReadFromBuffer(buffer);
            Buttons.ReadFromBuffer(buffer);
            Touchpad.ReadFromBuffer(buffer);

            DPad = (DualShock4Direction)(buffer[5] & 0xF);

            return true;
        }

        public void Dispose () {
            Info.Dispose();
        }
    }

    public class DualShock4Axes {
        private float[] State = new float [6];

        internal DualShock4Axes () {
        }

        public float this[DualShock4Axis axis] {
            get {
                return State[(int)axis];
            }
            set {
                State[(int)axis] = value;
            }
        }

        public float this[int index] {
            get {
                return State[index];
            }
        }

        public override string ToString () {
            var sb = new StringBuilder();

            for (var i = 0; i < State.Length; i++)
                sb.AppendFormat("{0}: {1:+0.000;-0.000}{2}", (DualShock4Axis)i, State[i], (i < (State.Length - 1)) ? "\r\n" : "");

            return sb.ToString();
        }

        internal float ByteToUnsignedFloat (byte b) {
            return b / 255f;
        }

        internal float ByteToSignedFloat (byte b) {
            return (b - 127) / 127.5f;
        }

        internal void ReadFromBuffer (byte[] buffer) {
            this[DualShock4Axis.LeftStickX] = ByteToSignedFloat(buffer[1]);
            this[DualShock4Axis.LeftStickY] = ByteToSignedFloat(buffer[2]);
            this[DualShock4Axis.RightStickX] = ByteToSignedFloat(buffer[3]);
            this[DualShock4Axis.RightStickY] = ByteToSignedFloat(buffer[4]);
            this[DualShock4Axis.L2] = ByteToUnsignedFloat(buffer[8]);
            this[DualShock4Axis.R2] = ByteToUnsignedFloat(buffer[9]);
        }
    }

    public class DualShock4Buttons {
        private bool[] State = new bool [12];

        internal DualShock4Buttons () {
        }

        public bool this[DualShock4Button button] {
            get {
                return State[(int)button];
            }
            private set {
                State[(int)button] = value;
            }
        }

        public bool this[int index] {
            get {
                return State[index];
            }
        }

        public override string ToString () {
            var sb = new StringBuilder();

            for (var i = 0; i < State.Length; i++)
                sb.AppendFormat("{0}: {1}{2}", (DualShock4Button)i, State[i], (i < (State.Length - 1)) ? "\r\n" : "");

            return sb.ToString();
        }

        internal void ReadFromBuffer (byte[] buffer) {
            this[DualShock4Button.Triangle] = (buffer[5] & (1 << 7)) != 0;
            this[DualShock4Button.Circle] = (buffer[5] & (1 << 6)) != 0;
            this[DualShock4Button.Cross] = (buffer[5] & (1 << 5)) != 0;
            this[DualShock4Button.Square] = (buffer[5] & (1 << 4)) != 0;

            this[DualShock4Button.L1] = (buffer[6] & (1 << 0)) != 0;
            this[DualShock4Button.R1] = (buffer[6] & (1 << 1)) != 0;
            this[DualShock4Button.L3] = (buffer[6] & (1 << 6)) != 0;
            this[DualShock4Button.R3] = (buffer[6] & (1 << 7)) != 0;
            this[DualShock4Button.Share] = (buffer[6] & (1 << 4)) != 0;
            this[DualShock4Button.Options] = (buffer[6] & (1 << 5)) != 0;

            this[DualShock4Button.PS] = (buffer[7] & (1 << 0)) != 0;
            this[DualShock4Button.TouchpadClick] = (buffer[7] & (1 << 2 - 1)) != 0;
        }
    }

    public class DualShock4Touchpad {
        public struct TouchInfo {
            public bool IsActive;
            public byte Id;
            public int X, Y;

            public override string ToString () {
                return String.Format("{3} #{0} @ {1:0000}, {2:0000}", Id, X, Y, IsActive ? "active" : "inactive");
            }
        }

        const int MaxTouches = 2;
        private readonly TouchInfo[] TouchInfos = new TouchInfo[MaxTouches];

        internal DualShock4Touchpad () {
        }

        public TouchInfo this[int index] {
            get {
                return TouchInfos[index];
            }
        }

        public int Count {
            get;
            private set;
        }

        public override string ToString () {
            var sb = new StringBuilder();

            for (var i = 0; i < Count; i++) {
                sb.AppendFormat("Touch #{0}: {1}\r\n", i, TouchInfos[i]);
            }

            return sb.ToString();
        }

        internal void ReadFromBuffer (byte[] buffer) {
            const int offset = 35;
            const int touchDataSize = 4;

            Count = 0;

            for (var i = 0; i < MaxTouches; i++) {
                var localOffset = offset + (i * touchDataSize);
                var touchX = buffer[localOffset + 1] + ((buffer[localOffset + 2] & 0xF) * 255);
                var touchY = ((buffer[localOffset + 2] & 0xF0) >> 4) + (buffer[localOffset + 3] * 16);

                TouchInfos[i] = new TouchInfo {
                    IsActive = (buffer[localOffset] & 0x80) == 0,
                    Id = (byte)(buffer[localOffset] & 0x7F),
                    X = touchX,
                    Y = touchY
                };

                if (TouchInfos[i].IsActive)
                    Count = Math.Max(Count, i + 1);
            }
        }
    }

    public enum DualShock4Axis : int {
        LeftStickX,
        LeftStickY,
        RightStickX,
        RightStickY,
        L2,
        R2
    }

    public enum DualShock4Direction : int {
        Neutral = 8,
        Up = 0,
        UpRight = 1,
        Right = 2,
        DownRight = 3,
        Down = 4,
        DownLeft = 5,
        Left = 6,
        UpLeft = 7
    }

    public enum DualShock4Button : int {
        Triangle = 0,
        Circle,
        Cross,
        Square,
        L1,
        R1,
        L3,
        R3,
        Share,
        Options,
        PS,
        TouchpadClick
    }
}
