using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.Hardware;
using Microsoft.SPOT.Hardware;

namespace MindSharp
{
    public class NxtMotor
    {


        public NxtMotor(NxShield shield, MotorPlug plug)
        {

            SoftwareI2C s = new SoftwareI2C(shield.PinMap.GetClockPin(SensorPlug.BAS1), shield.PinMap.GetDataPin(SensorPlug.BAS1), 0);
            s.ScanI2CDevices();

            SoftwareI2C s2 = new SoftwareI2C(shield.PinMap.GetClockPin(SensorPlug.BAS2), shield.PinMap.GetDataPin(SensorPlug.BAS2), 0);
            s2.ScanI2CDevices();

            var t = new I2CDevice.I2CTransaction[2];
            var regaddr = new byte[] { 0x00 };
            var data = new byte[8];

            t[0] = Microsoft.SPOT.Hardware.I2CDevice.CreateWriteTransaction(regaddr);
            t[1] = Microsoft.SPOT.Hardware.I2CDevice.CreateReadTransaction(data);

            var r = s.Execute(shield.PinMap.BankA_Address, new I2CDevice.I2CTransaction[1] { Microsoft.SPOT.Hardware.I2CDevice.CreateReadTransaction(new byte[1]) });

            //regaddr[0] = 0x00;
            regaddr[0] = 0x08;

            var result = s.Execute(shield.PinMap.BankA_Address, t);

           // string version = Convertion.ToString(data);

            //motor controller is on BAS1
            //SoftwareI2CBus b = new SoftwareI2CBus(shield.PinMap.GetClockPin(SensorPlug.BAS1), shield.PinMap.GetDataPin(SensorPlug.BAS1));
            
           // var bus = b.CreateI2CDevice(shield.PinMap.BankA_Address, 0);
            
            //get version
            //byte[] data = new byte[2];
            //bus.Read(data, 0, 1);
        }
    }
}
