using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware;

namespace MindSharp.Peripherals
{
    public class NxtLight
    {
        OutputPort _lightPort;
        AnalogIn _brightnessPort;
        bool _light;

        public NxtLight(NxShield shield, SensorPlug plug)
        {
            //init
            _lightPort = new OutputPort(shield.PinMap.GetClockPin(plug), false);
            _light = false;
            _brightnessPort = new AnalogIn(shield.PinMap.GetAnalogDataPin(plug));
            _brightnessPort.SetLinearScale(0, 100);
        }

        public int GetBrightness()
        {
            return _brightnessPort.Read();
        }

        public bool Light
        {
            get
            {
                return _light;
            }
            set
            {
                _light = value;
                _lightPort.Write(_light);
            }
        }
    }
}
