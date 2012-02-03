using System;
using Microsoft.SPOT;
using MindSharp.Communications;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace MindSharp.Peripherals
{
    public class NxtUltrasonic
    {
        II2c _i2c;

        public NxtUltrasonic(NxShield shield, SensorPlug plug)
        {

			//var c2 = new SoftwareI2c2(shield.PinMap.GetClockPin(SensorPlug.BAS1), shield.PinMap.GetDataPin(SensorPlug.BAS1), 0);
			////c.UseUltrasonicHack = true;
			//var result2 = c2.ReadRegister(0x03, 0x41, false);

			//return;

			//var c = new SoftwareI2c2(shield.PinMap.GetClockPin(plug), shield.PinMap.GetDataPin(plug), 0);
			//c.UseUltrasonicHack = true;
			//c.WriteRegister(0x02, 0x00, 0, false);
			//var result = c.ReadRegister(0x02, 0x00, false);


			//return;
			_i2c = shield.GetI2c(plug);

			////continous mode
			//var portTransaction = new I2CDevice.I2CTransaction[1];
			//portTransaction[0] = I2CDevice.CreateWriteTransaction(new byte[] { 0x41, 0x02 }); //port
			//var size = _i2c.Execute(0x02, portTransaction);

			//var s = _i2c as SoftwareI2c;
			//if (!s.SCL.Active)
			//    s.SCL.Active = true;
			//s.SCL.Write(true);


			//Thread.Sleep(20);


			var t = new I2CDevice.I2CTransaction[2];
			((SoftwareI2c)_i2c).UltrasonicHack = true;
			var regaddr = new byte[] { 0x00 };
			var data = new byte[8];

			t[0] = I2CDevice.CreateWriteTransaction(regaddr);
			t[1] = I2CDevice.CreateReadTransaction(data);
			regaddr[0] = 0x00;
			var r = _i2c.Execute(0x02, t);



			//portTransaction[0] = I2CDevice.CreateReadTransaction(

			//writeByte(0x41, 0x02);

			//writeByte(0x41, 0x02);
			//delay(20);  // required for ultrasonic to work.
			//return readByte(0x42); 
			//_i2c = shield.GetI2c(plug);


			//var portTransaction = new I2CDevice.I2CTransaction[1];
			//portTransaction[0] = I2CDevice.CreateWriteTransaction(new byte[1] { 0x41 });

			////portTransaction[1] = I2CDevice.CreateReadTransaction(result);

			//var size = _i2c.Execute(2, portTransaction);

			////_i2c.Execute(0x02, 

			//var result = _i2c.ScanAll();
        }
    }
}
