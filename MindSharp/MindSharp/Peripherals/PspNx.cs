using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using MindSharp.Communications;

namespace MindSharp.Peripherals
{
    public class PspNx
    {

        II2C _i2c;
		ushort _address;

        public PspNx(NxShield shield, SensorPlug plug)
        {
            _i2c = shield.GetI2c(plug);

			var deviceInfo = _i2c.Scan("PSX-NX");
			if (deviceInfo.Length == 0)
				throw new MissingPeripheralException();

			_address = deviceInfo[0].Address;
        }


        void ReadPort(II2C i2c, ushort address, byte port, ref byte[] result, int timeout = 100)
        {
            var portTransaction = new I2CDevice.I2CTransaction[2];
            portTransaction[0] = I2CDevice.CreateWriteTransaction(new byte[1] { port });

            portTransaction[1] = I2CDevice.CreateReadTransaction(result);

			var size = i2c.Execute(address, portTransaction, timeout);

            if (size != (result.Length + 1))
                throw new I2CExecuteException(size);
        }

        public bool UpdateButtonState(PspState state)
        {
            var data = new byte[6];
            try
            {
                ReadPort(_i2c, _address, 0x42, ref data, 1000);
            }
            catch (I2CExecuteException ex)
            {
                if (ex.SizeTranferred != 0)
                    throw;
				return false;
            }

            var b = data[0];

            state.LeftJoystickButton = ((b >> 1) & 0x01) == 0;
            state.RightJoystickButton = ((b >> 2) & 0x01) == 0;

            state.D = ((b >> 4) & 0x01) == 0;
            state.C = ((b >> 5) & 0x01) == 0;
            state.B = ((b >> 6) & 0x01) == 0;
            state.A = ((b >> 7) & 0x01) == 0;

            b = data[1];

            state.L2 = ((b) & 0x01) == 0;
            state.R2 = ((b >> 1) & 0x01) == 0;
            state.L1 = ((b >> 2) & 0x01) == 0;
            state.R1 = ((b >> 3) & 0x01) == 0;
            state.Triangle = ((b >> 4) & 0x01) == 0;
            state.Circle = ((b >> 5) & 0x01) == 0;
            state.Cross = ((b >> 6) & 0x01) == 0;
            state.Square = ((b >> 7) & 0x01) == 0;

            state.LeftJoystick = new Point { X = (((float)data[2]) - 128) / 128, Y = (((float)data[3]) - 128) / 128 };
            state.RightJoystick = new Point { X = (((float)data[4]) - 128) / 128, Y = (((float)data[5]) - 128) / 128 };

			return true;
        }

    }


    public class PspState
    {
        public bool L1 { get; set; }
        public bool L2 { get; set; }
        public bool R1 { get; set; }
        public bool R2 { get; set; }
        public bool A { get; set; }
        public bool B { get; set; }
        public bool C { get; set; }
        public bool D { get; set; }
        public bool Triangle { get; set; }
        public bool Square { get; set; }
        public bool Circle { get; set; }
        public bool Cross { get; set; }

        public bool LeftJoystickButton { get; set; }
        public bool RightJoystickButton { get; set; }

        public Point LeftJoystick { get; set; }
        public Point RightJoystick { get; set; }
    }

    public struct Point
    {
        public float X;
        public float Y;

		public override string ToString()
		{
			return "{X:" + X.ToString() + ",Y:" + Y.ToString() + "}";
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var compare = (Point)obj;
			return X == compare.X && Y == compare.Y;
		}

		public static bool operator == (Point a, Point b)
		{
			return a.Equals(b);
		}

		public static bool operator != (Point a, Point b)
		{
			return !a.Equals(b);
		}

    }

}
