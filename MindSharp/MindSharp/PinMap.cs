using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware;

namespace MindSharp
{
    public abstract class PinMap
    {
        public byte BankA_Address { get; protected set; }
        public byte BankB_Address { get; protected set; }

        public Cpu.Pin Motor_Clock { get; protected set; }
        public Cpu.Pin Motor_Data { get; protected set; }

        public Cpu.Pin BAS1_Clock { get; protected set; }
        public Cpu.Pin BAS1_Data { get; protected set; }
        public AnalogIn.Pin BAS1_Data_Analog { get; protected set; }

        public Cpu.Pin BAS2_Clock { get; protected set; }
        public Cpu.Pin BAS2_Data { get; protected set; }
        public AnalogIn.Pin BAS2_Data_Analog { get; protected set; }

        public Cpu.Pin BBS1_Clock { get; protected set; }
        public Cpu.Pin BBS1_Data { get; protected set; }
        public AnalogIn.Pin BBS1_Data_Analog { get; protected set; }

        public Cpu.Pin BBS2_Clock { get; protected set; }
        public Cpu.Pin BBS2_Data { get; protected set; }
        public AnalogIn.Pin BBS2_Data_Analog { get; protected set; }

        public Cpu.Pin LeftBtn_RedLed { get; protected set; }
        public Cpu.Pin RightBtn_BlueLed { get; protected set; }
        public Cpu.Pin GoBtn_GreenLed { get; protected set; }

        public PinMap()
        {
            //defaults
            BankA_Address = 0x03; //documented as 0x06;
            BankB_Address = 0x04; //documented as 0x08;

            SetupPinMap();
        }

        protected abstract void SetupPinMap();

        public Cpu.Pin GetDataPin(SensorPlug plug)
        {
            switch (plug)
            {
                case SensorPlug.BAS1:
                    return BAS1_Data;
                case SensorPlug.BAS2:
                    return BAS2_Data;
                case SensorPlug.BBS1:
                    return BBS1_Data;
                case SensorPlug.BBS2:
                    return BBS2_Data;
                default:
                    return (Cpu.Pin)0;
            }
        }

        public Cpu.Pin GetClockPin(SensorPlug plug)
        {
            switch (plug)
            {
                case SensorPlug.BAS1:
                    return BAS1_Clock;
                case SensorPlug.BAS2:
                    return BAS2_Clock;
                case SensorPlug.BBS1:
                    return BBS1_Clock;
                case SensorPlug.BBS2:
                    return BBS2_Clock;
                default:
                    return (Cpu.Pin)0;
            }
        }

        public AnalogIn.Pin GetAnalogDataPin(SensorPlug plug)
        {
            switch (plug)
            {
                case SensorPlug.BAS1:
                    return BAS1_Data_Analog;
                case SensorPlug.BAS2:
                    return BAS2_Data_Analog;
                case SensorPlug.BBS1:
                    return BBS1_Data_Analog;
                case SensorPlug.BBS2:
                    return BBS2_Data_Analog;
                default:
                    return (AnalogIn.Pin)0;
            }
        }

        public bool IsBankA(MotorPlug plug)
        {
            switch (plug)
            {
                case MotorPlug.BankA_M1:
                    return true;
                case MotorPlug.BankA_M2:
                    return true;
                default:
                    return false;
            }
        }
    }
}
