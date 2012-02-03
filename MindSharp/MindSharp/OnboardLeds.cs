using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware;

namespace MindSharp
{
    public class OnboardLeds
    {
        OutputPort _redLedPort;
        OutputPort _greenLedPort;
        OutputPort _blueLedPort;
        bool _redLed;
        bool _greenLed;
        bool _blueLed;

        public OnboardLeds(NxShield shield)
        {
            //init
            _redLedPort = new OutputPort((Cpu.Pin)shield.PinMap.LeftBtn_RedLed, true);
            _greenLedPort = new OutputPort((Cpu.Pin)shield.PinMap.GoBtn_GreenLed, true);
            _blueLedPort = new OutputPort((Cpu.Pin)shield.PinMap.RightBtn_BlueLed, true);

            _redLed = false;
            _greenLed = false;
            _blueLed = false;
        }

        public bool Red
        {
            get
            {
                return _redLed;
            }
            set
            {
                _redLed = value;
                _redLedPort.Write(!_redLed);
            }
        }

        public bool Green
        {
            get
            {
                return _greenLed;
            }
            set
            {
                _greenLed = value;
                _greenLedPort.Write(!_greenLed);
            }
        }

        public bool Blue
        {
            get
            {
                return _blueLed;
            }
            set
            {
                _blueLed = value;
                _blueLedPort.Write(!_blueLed);
            }
        }

    }
}
