//
//                  Software I2C Driver - 19 April 2011.
//                               Al Moyle
//
//   Based on original I2C Bus Master Code from http://en.wikipedia.org/wiki/I%C2%B2C -
//                Ported to C# by Gus and modified by MarkH.
//               See http://www.tinyclr.com/forum/1/1647/#/1/
//
using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using MSH = Microsoft.SPOT.Hardware;

namespace MindSharp
{
    public class TimeOutException : ApplicationException
    {
        public TimeOutException()
            : base("Timeout Error")
        {
        }

        public TimeOutException(string message)
            : base(message)
        {
        }
    }

    public class SoftwareI2C : IDisposable
    {

        public void ScanI2CDevices()
        {
            var t = new Microsoft.SPOT.Hardware.I2CDevice.I2CTransaction[2];
            var regaddr = new byte[] { 0x00 };
            var data = new byte[8];

            t[0] = Microsoft.SPOT.Hardware.I2CDevice.CreateWriteTransaction(regaddr);
            t[1] = Microsoft.SPOT.Hardware.I2CDevice.CreateReadTransaction(data);

            Debug.Print("Scanning Software Bus for devices...\r\n");

            for (ushort i = 1; i < 128; i++)
            {
                int s;
                s = Execute(i, new Microsoft.SPOT.Hardware.I2CDevice.I2CTransaction[1] { Microsoft.SPOT.Hardware.I2CDevice.CreateReadTransaction(new byte[1]) });

                if (s == 1)
                {
                    regaddr[0] = 0x00;

                    Execute(i, t);

                    string version = ToString(data);

                    regaddr[0] = 0x08;
                    Execute(i, t);
                    string vendor = ToString(data);

                    regaddr[0] = 0x10;
                    Execute(i, t);
                    string dev = ToString(data);

                    Debug.Print("Found at: 0x" + i.ToString());
                    Debug.Print(" Version: " + version);
                    Debug.Print(" Vendor : " + vendor);
                    Debug.Print(" Device : " + dev);
                    Debug.Print("");
                }
            }
        }

        private string ToString(byte[] data)
        {
            return new string(System.Text.UTF8Encoding.UTF8.GetChars(data));
        }


        #region Class Data Members

        internal TristatePort SCL, SDA;
        int I2CSpeed = 100;              // This variable isn't used ... Remove it?
        long TimeOutTicks = 0;           // Number of ticks in time out interval.
        int TimeOut = 10;                // Clock stretching time out interval in msec.

        bool Start;

        #endregion

        #region Constructor

        public SoftwareI2C(Cpu.Pin scl, Cpu.Pin sda, int clock)
        {
            Start = false;
            TimeOutTicks = (long)TimeOut * TimeSpan.TicksPerMillisecond;
            //
            SCL = new TristatePort(scl, false, false, Port.ResistorMode.PullUp);
            SDA = new TristatePort(sda, false, false, Port.ResistorMode.PullUp);
            // scl and sda default to input, the next 2 lines make this explicit.
            MakePinInput(SCL);
            MakePinInput(SDA);
            I2CSpeed = 30;
        }

        #endregion

        private readonly byte I2C_Read = 1;
        private readonly byte I2C_Write = 0;

        #region I2C Interface

        public int Execute(ushort address, MSH.I2CDevice.I2CTransaction[] trans, int timeout = 100)
        {
            int sizetransfered = 0;

            foreach (var t in trans)
            {
                // Read Transaction
                if (t is MSH.I2CDevice.I2CReadTransaction)
                {
                    byte addr = (byte)((address << 1) | I2C_Read);

                    if (Transmit(true, false, addr)) return sizetransfered;		// Return true if error, then quit

                    for (int i = 0; i < t.Buffer.Length; i++)
                    {
                        t.Buffer[i] = Receive((i != (t.Buffer.Length - 1)), false);
                        sizetransfered++;
                    }
                }

                // Write transaction
                else
                {
                    byte addr = (byte)((address << 1) | I2C_Write);

                    if (Transmit(true, false, addr)) return sizetransfered;		// Return true if error, then quit

                    foreach (var b in t.Buffer)
                    {
                        if (Transmit(false, false, b)) return sizetransfered;
                        sizetransfered++;
                    }
                }
            }

            SendStopCondition();

            return sizetransfered;
        }

        /// <summary>
        /// Sends data to the remote device
        /// </summary>
        /// <param name="sendStartCondition">Perform a HIGH to LOW transition of SDA line while the SCL is high.</param>
        /// <param name="sendStopCondition">Perform a LOW to HIGH transition of SDA line while the SCL is high.</param>
        /// <param name="byteToSend">Byte to transmit</param>
        /// <returns></returns>
        public bool Transmit(bool sendStartCondition, bool sendStopCondition, byte byteToSend)
        {
            if (sendStartCondition)
            {
                try
                {
                    SendStartCondition();
                }
                catch (TimeOutException e)
                {
                    throw new TimeOutException(e.Message);
                }
            }
            for (int bit = 0; bit < 8; bit++)
            {
                try
                {
                    WriteBit((byteToSend & 0x80) != 0);
                }
                catch (TimeOutException e)
                {
                    throw new TimeOutException(e.Message);
                }

                byteToSend <<= 1;
            }
            bool nack;
            try
            {
                nack = ReadBit();
            }
            catch (TimeOutException e)
            {
                throw new TimeOutException(e.Message);
            }
            //
            if (sendStopCondition)
            {
                try
                {
                    SendStopCondition();
                }
                catch (TimeOutException e)
                {
                    throw new TimeOutException(e.Message);
                }
            }
            // Return value is "true" for NAK
            //  "false" for ACK.
            return nack;
        }

        /// <summary>
        /// Receive data from remote device.
        /// </summary>
        /// <param name="acknowledgeBit">
        /// Each device when addressed to has to generate an acknowledge signal after the reception of each byte. 
        /// The master generates an extra clock pulse which is associated with the <c>acknowledgeBit</c>. 
        /// The device that acknowledges pulls down the SDA line during the acknowledge clock pulse.
        /// </param>
        /// <param name="sendStopCondition">Perform a LOW to HIGH transition of SDA line while the SCL is high.</param>
        /// <returns></returns>
        ///
        //  18 April 2011 - Changed parameter "acknowledgeBit" to "sendAcknowledgeBit" to make it consistent with
        //   "sendStopCondition" and parameters of the "Transmit" routine.
        public byte Receive(bool sendAcknowledgeBit, bool sendStopCondition)
        {
            byte d = 0;
            bool b;

            for (int bit = 0; bit < 8; bit++)
            {
                d <<= 1;

                try
                {
                    b = ReadBit();
                }
                catch (TimeOutException e)
                {
                    throw new TimeOutException(e.Message);
                }
                if (b)
                    d |= 1;
            }
            //
            try
            {
                WriteBit(!sendAcknowledgeBit);
            }
            catch (TimeOutException e)
            {
                throw new TimeOutException(e.Message);
            }
            //
            if (sendStopCondition)
            {
                try
                {
                    SendStopCondition();
                }
                catch (TimeOutException e)
                {
                    throw new TimeOutException(e.Message);
                }
            }
            //
            return d;
        }

        #endregion

        #region Internal helpers

        static void MakePinOutput(TristatePort port)
        {
            if (!port.Active)
                port.Active = true;
        }

        static void MakePinInput(TristatePort port)
        {
            if (port.Active)
                port.Active = false;
        }

        //
        // I left this routine in place, but didn't add any delay code.  With
        //  no additional delay beyond the overhead associated with a call to
        //  I2CDELAY, the serial clock frequency is approximately 1.5 KHz.
        //
        void I2CDELAY(int delay)
        {
            return;
            ////add code for delay
            //DateTime startTime = DateTime.Now;

            //int stopTicks = delay * 10;

            //TimeSpan divTime = DateTime.Now - startTime;
            //while (divTime.Ticks < stopTicks)
            //{
            //    divTime = DateTime.Now - startTime;
            //}
        }

        void ClearSCL()
        {
            MakePinOutput(SCL);
        }

        void ClearSDA()
        {
            MakePinOutput(SDA);
        }

        bool ReadSCL()
        {
            MakePinInput(SCL);
            return SCL.Read();
        }

        bool ReadSDA()
        {
            MakePinInput(SDA);
            return SDA.Read();
        }

        bool ReadBit()
        {
            // "ReadSDA" makes SDA an input - processor lets go of pin and internal
            //  pull-up resistor makes it high.  Now slave can drive the pin.
            ReadSDA();

            I2CDELAY(I2CSpeed);

            // Clock stretching - Makes SCL an input and pull-up resistor makes
            //  the pin high.  Slave device can pull SCL low to extend clock cycle.
            long endStretch = Utility.GetMachineTime().Ticks + TimeOutTicks;
            while (!ReadSCL())
            {
                // How long have we been stuck in the while loop?
                if (Utility.GetMachineTime().Ticks >= endStretch)
                    throw new TimeOutException();           // Too long, so bail out by throwing an exception.
            }

            // At this point, SCL is high and SDA is valid - so read the bit.
            bool bit = SDA.Read();

            I2CDELAY(I2CSpeed);

            ClearSCL();     // Pull the serial clock line low ...

            return bit;     //  and return.
        }

        bool WriteBit(bool bit)
        {
            if (bit)
            {
                ReadSDA();      // Make SDA an input ... so pin is pulled up.
            }
            else
            {
                ClearSDA();     // Make SDA an output ... so pin is pulled low.
            }

            I2CDELAY(I2CSpeed);
            // Clock stretching - Makes SCL an input and pull-up resistor makes
            //  the pin high.  Slave device can pull SCL low to extend clock cycle.
            long endStretch = Utility.GetMachineTime().Ticks + TimeOutTicks;
            while (!ReadSCL())
            {
                // How long have we been stuck in the while loop?
                if (Utility.GetMachineTime().Ticks >= endStretch)
                    throw new TimeOutException();           // Too long, so bail out by throwing an exception.
            }
            // SCL is high and SDA is valid ...
            //  Check that nobody else is driving SDA
            if (bit && !ReadSDA())
            {
                return false;       // Lost arbitration
            }

            I2CDELAY(I2CSpeed);
            ClearSCL();

            return true;           // Success!
        }

        bool SendStartCondition()
        {
            if (Start)
            {
                // set SDA to 1 
                ReadSDA();
                I2CDELAY(I2CSpeed);
                //
                // Clock stretching - Makes SCL an input and pull-up resistor makes
                //  the pin high.  Slave device can pull SCL low to extend clock cycle.
                long endStretch = Utility.GetMachineTime().Ticks + TimeOutTicks;
                while (!ReadSCL())
                {
                    // How long have we been stuck in the while loop?
                    if (Utility.GetMachineTime().Ticks >= endStretch)
                        throw new TimeOutException();           // Too long, so bail out by throwing an exception.
                }
            }

            if (!ReadSDA())
            {
                return false;
            }

            // SCL is high, set SDA from 1 to 0 
            ClearSDA();
            I2CDELAY(I2CSpeed);
            ClearSCL();

            Start = true;

            return true;
        }

        bool SendStopCondition()
        {
            // set SDA to 0 
            ClearSDA();
            I2CDELAY(I2CSpeed);
            //
            // Clock stretching - Makes SCL an input and pull-up resistor makes
            //  the pin high.  Slave device can pull SCL low to extend clock cycle.
            long endStretch = Utility.GetMachineTime().Ticks + TimeOutTicks;
            while (!ReadSCL())
            {
                // How long have we been stuck in the while loop?
                if (Utility.GetMachineTime().Ticks >= endStretch)
                    throw new TimeOutException();           // Too long, so bail out by throwing an exception.
            }
            //
            // SCL is high, set SDA from 0 to 1 
            if (!ReadSDA())
            {
                return false;
            }

            I2CDELAY(I2CSpeed);
            Start = false;

            return true;
        }

        #endregion

        public void Dispose()
        {
            SCL.Dispose();
            SDA.Dispose();
        }
    }
}

