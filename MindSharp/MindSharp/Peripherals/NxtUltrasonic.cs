using System;
using Microsoft.SPOT;
using MindSharp.Communications;

namespace MindSharp.Peripherals
{
    public class NxtUltrasonic
    {
        II2C _i2c;

        public NxtUltrasonic(NxShield shield, SensorPlug plug)
        {

            _i2c = shield.GetI2c(plug);
            _i2c.ScanAll();
        }
    }
}
