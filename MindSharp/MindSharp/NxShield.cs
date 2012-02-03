using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using MindSharp.Communications;

namespace MindSharp
{
    public class NxShield
    {
        II2c[] _i2cs;
		II2c _shieldI2c;
		ushort _bankAAddress;
		ushort _bankBAddress;

        public PinMap PinMap { get; protected set; }

        public NxShield(PinMap pinMap)
        {
			//init fields
			_i2cs = new II2c[4];

            //set fields
            PinMap = pinMap;
			//_shieldI2c = GetI2c(MotorPlug.BankA_M1);
			//var devices = _shieldI2c.Scan("NXShldD");
			//_bankAAddress = devices[0].Address;
			//_bankBAddress = devices[1].Address;
        }

        public II2c GetI2c(SensorPlug plug)
        {
            switch (plug)
            {
                case SensorPlug.BAS1:
                    if (_i2cs[0] == null)
                        _i2cs[0] = CreateI2C(PinMap.BAS1_Clock, PinMap.BAS1_Data);
                    return _i2cs[0];
                case SensorPlug.BAS2:
                    if (_i2cs[1] == null)
                        _i2cs[1] = CreateI2C(PinMap.BAS2_Clock, PinMap.BAS2_Data);
                    return _i2cs[1];
                case SensorPlug.BBS1:
                    if (_i2cs[2] == null)
                        _i2cs[2] = CreateI2C(PinMap.BBS1_Clock, PinMap.BBS1_Data);
                    return _i2cs[2];
                case SensorPlug.BBS2:
                    if (_i2cs[3] == null)
                        _i2cs[3] =  CreateI2C(PinMap.BBS2_Clock, PinMap.BBS2_Data);
                    return _i2cs[3];
            }
            return null;
        }

        II2c CreateI2C(Cpu.Pin clock, Cpu.Pin data)
        {
            return new SoftwareI2c(clock, data, 0);
        }

        public II2c GetI2c(MotorPlug plug)
        {
            if (_i2cs[0] == null)
                _i2cs[0] = new SoftwareI2c(PinMap.BAS1_Clock, PinMap.BAS1_Data, 0);
            return _i2cs[0];
        }


		public float GetBatteryVoltage()
		{


			//SoftwareI2CBus b = new SoftwareI2CBus(PinMap.BAS1_Clock, PinMap.BAS1_Data);
			//var ss = b.CreateI2CDevice(_bankAAddress, 0);
			//var dataRaw2 = new byte[1];
			//int write;
			//int read;
			//var success = ss.WriteRead(new byte[] { 0x41 }, 1, 1, dataRaw2, 1, 1, out write, out read);

			//var dataarray = new byte[1];
			//ReadPort(_shieldI2c, 0x03, 0x4, ref dataarray);
			//return dataarray[0];

			

			var portTransaction = new I2CDevice.I2CTransaction[2];
			portTransaction[0] = I2CDevice.CreateWriteTransaction(new byte[] {0x41}); //port
			var dataRaw = new byte[1];
			portTransaction[1] = I2CDevice.CreateReadTransaction(dataRaw);
			var size = _shieldI2c.Execute(0x03, portTransaction);

			var voltage = ((float)(dataRaw[0] * 37)) / 1000;

			return voltage;

		}

		private void ReadPort(II2c dev, ushort address, byte port, ref byte[] result, int timeout = 100)
		{
			var portTransaction = new I2CDevice.I2CTransaction[2];
			portTransaction[0] = I2CDevice.CreateWriteTransaction(new byte[] { port }); //port
			portTransaction[1] = I2CDevice.CreateReadTransaction(result);

			var size = dev.Execute(address, portTransaction, timeout);

			if (size != (result.Length + 1))
			{
				throw new I2cExecuteException(size);
			}
		}

    }

}
