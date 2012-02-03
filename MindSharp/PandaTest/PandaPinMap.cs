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
		public override Cpu.Pin BAS1_Clock { get { return (Cpu.Pin)FEZ_Pin.Digital.An5; } }
		public override Cpu.Pin BAS1_Data { get { return (Cpu.Pin)FEZ_Pin.Digital.An4; } }
		public override AnalogIn.Pin BAS1_Data_Analog { get { return (AnalogIn.Pin)FEZ_Pin.AnalogIn.An4; } }

		public override Cpu.Pin BAS2_Clock { get { return (Cpu.Pin)FEZ_Pin.Digital.Di2; } }
		public override Cpu.Pin BAS2_Data { get { return (Cpu.Pin)FEZ_Pin.Digital.An0; } }
		public override AnalogIn.Pin BAS2_Data_Analog { get { return (AnalogIn.Pin)FEZ_Pin.AnalogIn.An0; } }

		public override Cpu.Pin BBS1_Clock { get { return (Cpu.Pin)FEZ_Pin.Digital.Di4; } }
		public override Cpu.Pin BBS1_Data { get { return (Cpu.Pin)FEZ_Pin.Digital.An1; } }
		public override AnalogIn.Pin BBS1_Data_Analog { get { return (AnalogIn.Pin)FEZ_Pin.AnalogIn.An1; } }

		public override Cpu.Pin BBS2_Clock { get { return (Cpu.Pin)FEZ_Pin.Digital.Di7; } }
		public override Cpu.Pin BBS2_Data { get { return (Cpu.Pin)FEZ_Pin.Digital.An2; } }
		public override AnalogIn.Pin BBS2_Data_Analog { get { return (AnalogIn.Pin)FEZ_Pin.AnalogIn.An2; } }

		public override Cpu.Pin LeftBtn_RedLed { get { return (Cpu.Pin)FEZ_Pin.Digital.Di8; } }
		public override Cpu.Pin RightBtn_BlueLed { get { return (Cpu.Pin)FEZ_Pin.Digital.Di12; } }
		public override Cpu.Pin GoBtn_GreenLed { get { return (Cpu.Pin)FEZ_Pin.Digital.An3; } }
    }
}
