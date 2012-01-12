using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace MindSharp
{
    public class PspNx
    {

        SoftwareI2C _i2c;

        public PspNx(NxShield shield, SensorPlug plug)
        {
            _i2c = shield.GetI2c(plug);

            _i2c.ScanI2CDevices();


        }

        void ReadPort(SoftwareI2C dev, ushort address, byte port, ref byte[] result, int timeout = 100)
        {
            if (dev == null) return;

            var portTransaction = new I2CDevice.I2CTransaction[2];
            portTransaction[0] = I2CDevice.CreateWriteTransaction(new byte[1] { port });

            portTransaction[1] = I2CDevice.CreateReadTransaction(result);

            var size = dev.Execute(address, portTransaction, timeout);

            if (size != (result.Length + 1))
            {
                throw new I2CExecuteException(size);
            }
        }

        public void UpdateButtonState()
        {
            var data = new byte[6];
            try
            {
                ReadPort(_i2c, 0x1, 0x42, ref data, 1000);

            }

            // Sometime we got 0 as size transferred, dunno why, just ignore if it's the case...
            catch (I2CExecuteException ex)
            {
                if (ex.SizeTranferred != 0)
                {
                    throw ex;
                }
                return;
            }

            var b = data[0];

            LeftJoystickButton = ((b >> 1) & 0x01) == 0;
            RightJoystickButton = ((b >> 2) & 0x01) == 0;

            D = ((b >> 4) & 0x01) == 0;
            C = ((b >> 5) & 0x01) == 0;
            B = ((b >> 6) & 0x01) == 0;
            A = ((b >> 7) & 0x01) == 0;

            b = data[1];

            L2 = ((b) & 0x01) == 0;
            R2 = ((b >> 1) & 0x01) == 0;
            L1 = ((b >> 2) & 0x01) == 0;
            R1 = ((b >> 3) & 0x01) == 0;
            Triangle = ((b >> 4) & 0x01) == 0;
            Circle = ((b >> 5) & 0x01) == 0;
            Cross = ((b >> 6) & 0x01) == 0;
            Square = ((b >> 7) & 0x01) == 0;

            LeftJoystick = new Point2 { X = (((float)data[2]) - 128) / 128, Y = (((float)data[3]) - 128) / 128 };
            RightJoystick = new Point2 { X = (((float)data[4]) - 128) / 128, Y = (((float)data[5]) - 128) / 128 };
        }

        public bool L1 { get; private set; }
        public bool L2 { get; private set; }
        public bool R1 { get; private set; }
        public bool R2 { get; private set; }
        public bool A { get; private set; }
        public bool B { get; private set; }
        public bool C { get; private set; }
        public bool D { get; private set; }
        public bool Triangle { get; private set; }
        public bool Square { get; private set; }
        public bool Circle { get; private set; }
        public bool Cross { get; private set; }

        public bool LeftJoystickButton { get; private set; }
        public bool RightJoystickButton { get; private set; }

        public Point2 LeftJoystick { get; private set; }
        public Point2 RightJoystick { get; private set; }
    }

    public struct Point2
    {
        public float X;
        public float Y;
    }

    public class I2CExecuteException : Exception
    {
        public int SizeTranferred { get; private set; }

        public I2CExecuteException(int sizetransferred, string msg = null)
            : base(msg)
        {
            SizeTranferred = sizetransferred;
        }
    }
}
