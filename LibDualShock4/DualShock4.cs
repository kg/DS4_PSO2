using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HidLibrary;

namespace Squared.DualShock4 {
    public class DualShock4 : IDisposable {
        private readonly DualShock4Info Info;

        public readonly DualShock4Axes Axes = new DualShock4Axes();
        public readonly DualShock4Buttons Buttons = new DualShock4Buttons();
        public readonly DualShock4Touchpad Touchpad = new DualShock4Touchpad();
        public readonly DualShock4Sensors Sensors = new DualShock4Sensors();

        public DualShock4 (DualShock4Info info, bool exclusive) {
            Info = info;
            IsExclusive = exclusive;
        }

        public bool IsExclusive {
            get;
            private set;
        }

        public HidDevice Device {
            get {
                return Info.Device;
            }
        }

        public int Counter {
            get;
            private set;
        }

        public byte BatteryLevel {
            get;
            private set;
        }

        public DualShock4Direction DPad {
            get;
            private set;
        }

        public bool TryUpdate () {
            var buffer = new byte[64];

            int error;
            if (!Device.ReadFile(buffer, IsExclusive, out error)) {
                Console.WriteLine("Read error: {0}", (new Win32Exception(error).Message));
                return false;
            }

            Device.flush_Queue();

            Axes.ReadFromBuffer(buffer);
            Buttons.ReadFromBuffer(buffer);
            Touchpad.ReadFromBuffer(buffer);
            Sensors.ReadFromBuffer(buffer);

            // DPad state is encoded in 4 bits, basically indices into a clock
            DPad = (DualShock4Direction)(buffer[5] & 0xF);

            BatteryLevel = buffer[12];

            // The counter byte has two bits dedicated to buttons
            Counter = buffer[7] & 0xFC;

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

        // For triggers we map them to a [0, 1] range
        internal float ByteToUnsignedFloat (byte b) {
            return b / 255f;
        }

        // For axes with + - we map them to a full [-1, 1] range
        internal float ByteToSignedFloat (byte b) {
            var i = (b - 127);

            // Account for range of [-127, 128]
            if (i < 0)
                return i / 127f;
            else
                return i / 128f;
        }

        internal void ReadFromBuffer (byte[] buffer) {
            // All the axes are represented as single bytes (0-255) that cover
            //  the whole range of the axis, so we map them to floats

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
            // The button states are packed into single bits, so we unpack them out
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
            this[DualShock4Button.TouchpadClick] = (buffer[7] & (1 << 1)) != 0;
        }
    }

    public class DualShock4Touchpad {
        public const int MaxX = 1920;
        public const int MaxY = 943;

        public struct TouchInfo {
            public bool IsActive;
            public byte Id;
            public int X, Y;
            public int StartX, StartY;
            public DateTime StartWhen, When;

            public override string ToString () {
                return String.Format("{3} #{0} @ {1:0000}, {2:0000} (start {4:0000} {5:0000})", Id, X, Y, IsActive ? "active" : "inactive", StartX, StartY);
            }
        }

        const int MaxTouches = 2;
        private readonly TouchInfo[] TouchInfos = new TouchInfo[MaxTouches];
        private readonly TouchInfo[] PreviousTouchInfos = new TouchInfo[MaxTouches];

        internal DualShock4Touchpad () {
        }

        public TouchInfo GetPreviousState (int index) {
            return PreviousTouchInfos[index];
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

            Array.Copy(TouchInfos, PreviousTouchInfos, TouchInfos.Length);

            var now = DateTime.UtcNow;

            for (var i = 0; i < MaxTouches; i++) {
                var localOffset = offset + (i * touchDataSize);
                var isActive = (buffer[localOffset] & 0x80) == 0;
                var touchId = (byte)(buffer[localOffset] & 0x7F);
                var touchX = buffer[localOffset + 1] + ((buffer[localOffset + 2] & 0xF) * 255);
                var touchY = ((buffer[localOffset + 2] & 0xF0) >> 4) + (buffer[localOffset + 3] * 16);
                int startX, startY;
                DateTime startWhen;

                if (
                    (PreviousTouchInfos[i].Id == touchId) && 
                    PreviousTouchInfos[i].IsActive
                ) {
                    startX = PreviousTouchInfos[i].StartX;
                    startY = PreviousTouchInfos[i].StartY;
                    startWhen = PreviousTouchInfos[i].StartWhen;
                } else {
                    startX = touchX;
                    startY = touchY;
                    startWhen = now;
                }

                TouchInfos[i] = new TouchInfo {
                    IsActive = isActive,
                    Id = touchId,
                    X = touchX,
                    Y = touchY,
                    StartX = startX,
                    StartY = startY,
                    StartWhen = startWhen,
                    When = now
                };

                if (isActive) {
                    Count = Math.Max(Count, i + 1);
                }
            }
        }
    }

    public class DualShock4Sensors {
        private readonly float[] Readings = new float[6];

        public float this[int index] {
            get {
                return Readings[index];
            }
            private set {
                Readings[index] = value;
            }
        }

        public float this[DualShock4Sensor sensor] {
            get {
                return Readings[(int)sensor];
            }
        }

        public override string ToString () {
            var sb = new StringBuilder();

            for (var i = 0; i < Readings.Length; i++) {
                sb.AppendFormat("{0}: {1:+00.000;-00.000}\r\n", (DualShock4Sensor)i, Readings[i]);
            }

            return sb.ToString();
        }

        // Sensor readings effectively have a range of [-127, 128] with the fractional
        //  part encoded in a second unsigned byte.
        internal float DecodeHighLow (sbyte high, byte low) {
            float result = high;

            // FIXME: Not totally sure this is right.
            result += ((low / 255f) * Math.Sign(result));

            return result;
        }

        internal unsafe void ReadFromBuffer (byte[] buffer) {
            const int offset = 13;

            fixed (byte* pBuffer = buffer) {
                sbyte* pSigned = (sbyte *)pBuffer;

                for (var i = 0; i < Readings.Length; i++) {
                    int localOffset = (offset) + (i * 2);

                    // The order of the sensor reading bytes is opposite what you'd expect
                    //  on x86, probably because this is USB, so we grab hi/lo in reverse.
                    this[i] = DecodeHighLow(pSigned[localOffset + 1], buffer[localOffset]);
                }
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

    public enum DualShock4Sensor : int {
        GyroscopeX,
        GyroscopeY,
        GyroscopeZ,
        AccelerometerX,
        AccelerometerY,
        AccelerometerZ
    }
}
