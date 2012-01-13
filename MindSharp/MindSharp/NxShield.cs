using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware;
using MindSharp.Communications;

namespace MindSharp
{
    public class NxShield
    {
        II2C[] _i2cs;

        public PinMap PinMap { get; protected set; }

        public NxShield(PinMap pinMap)
        {
            //set fields
            PinMap = pinMap;
            _i2cs = new II2C[4];
        }

        public II2C GetI2c(SensorPlug plug)
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

        II2C CreateI2C(Cpu.Pin clock, Cpu.Pin data)
        {
            return new SoftwareI2C(clock, data, 0);
        }

        public II2C GetI2c(MotorPlug plug)
        {
            if (_i2cs[0] == null)
                _i2cs[0] = new SoftwareI2C(PinMap.BAS1_Clock, PinMap.BAS1_Data, 0);
            return _i2cs[0];
        }
    }

}
