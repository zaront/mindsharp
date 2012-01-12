using System;
using Microsoft.SPOT;
using MindSharp;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace PandaTest
{
    public class PandaPinMap : PinMap
    {
        protected override void SetupPinMap()
        {
            BAS1_Clock = (Cpu.Pin)FEZ_Pin.Digital.An5;
            BAS1_Data = (Cpu.Pin)FEZ_Pin.Digital.An4;
            BAS1_Data_Analog = (AnalogIn.Pin)FEZ_Pin.AnalogIn.An4;

            BAS2_Clock = (Cpu.Pin)FEZ_Pin.Digital.Di2;
            BAS2_Data = (Cpu.Pin)FEZ_Pin.Digital.An0;
            BAS2_Data_Analog = (AnalogIn.Pin)FEZ_Pin.AnalogIn.An0;

            BBS1_Clock = (Cpu.Pin)FEZ_Pin.Digital.Di4;
            BBS1_Data = (Cpu.Pin)FEZ_Pin.Digital.An1;
            BBS1_Data_Analog = (AnalogIn.Pin)FEZ_Pin.AnalogIn.An1;

            BBS2_Clock = (Cpu.Pin)FEZ_Pin.Digital.Di7;
            BBS2_Data = (Cpu.Pin)FEZ_Pin.Digital.An2;
            BBS2_Data_Analog = (AnalogIn.Pin)FEZ_Pin.AnalogIn.An2;

            LeftBtn_RedLed = (Cpu.Pin)FEZ_Pin.Digital.Di8;
            RightBtn_BlueLed = (Cpu.Pin)FEZ_Pin.Digital.Di12;
            GoBtn_GreenLed = (Cpu.Pin)FEZ_Pin.Digital.An3;
        }
    }
}
