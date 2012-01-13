using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace MindSharp.Communications
{
    public interface II2C
    {
        int Execute(ushort address, I2CDevice.I2CTransaction[] trans, int timeout = 100);
        I2CDeviceInfo[] ScanAll();
        I2CDeviceInfo[] Scan(string deviceName);
    }
}
