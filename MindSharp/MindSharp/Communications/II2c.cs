using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace MindSharp.Communications
{
    public interface II2c
    {
        int Execute(ushort address, I2CDevice.I2CTransaction[] trans, int timeout = 100);
        I2cDeviceInfo[] ScanAll();
        I2cDeviceInfo[] Scan(string deviceName);
    }
}
