//
//                  Software I2C Driver - 19 April 2011.
//                               Al Moyle
//
//   Based on original I2C Bus Master Code from http://en.wikipedia.org/wiki/I%C2%B2C -
//                Ported to C# by Gus and modified by MarkH an modified by PaulT.
//               See http://www.tinyclr.com/forum/1/1647/#/1/
//
using System;
using System.IO;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace MindSharp.Communications
{
    public class SoftwareI2c2
    {
        private const int RETRY_MAX = 15;
        
        private readonly long _timeOutTicks; // Number of ticks in time out interval.
        private readonly TristatePort _scl;
        private readonly TristatePort _sda;
        private const int I2_C_SPEED = 100;
        private const int TIME_OUT = 10; // Clock stretching time out interval in msec.
        private const byte WRITE = 0x00;
        private const byte READ = 0x01;
        private bool _isStarted;
        private int _errorCount;
        private static int _dummy;
        private bool _isMultimaster = false;

		public SoftwareI2c2(Cpu.Pin scl, Cpu.Pin sda, int clock)
        {
            _isStarted = false;
            _timeOutTicks = TIME_OUT*TimeSpan.TicksPerMillisecond;
            
            _scl = new TristatePort( scl, false, false, Port.ResistorMode.PullUp );
            _sda = new TristatePort( sda, false, false, Port.ResistorMode.PullUp );
            
            // scl and sda default to input, the next 2 lines make this explicit.
            MakePinInput( _scl );
            MakePinInput( _sda );
            
            //i2cSpeed = clock/ 1111????????????????
        }

        public bool UseUltrasonicHack { get; set; }

        private static void MakePinOutput( TristatePort port )
        {
            if( !port.Active )
            {
                port.Active = true;
            }
        }

        private static void MakePinInput( TristatePort port )
        {
            if( port.Active )
            {
                port.Active = false;
            }
        }

        //
        // I left this routine in place, but didn't add any delay code.  With
        //  no additional delay beyond the overhead associated with a call to
        //  I2CDELAY, the serial clock frequency is approximately 1.5 KHz.
        //
        private void I2Cdelay( int delay )
        {
            return;
			//for( int j = 0; j<delay; j++ )
			//{
			//    _dummy = j;
			//}            
        }

        private void clearScl()
        {
            MakePinOutput( _scl );
        }

        private void clearSda()
        {
            MakePinOutput( _sda );
        }

        private void releaseSda()
        {
            MakePinInput(_sda);
        }

        private void releaseScl()
        {
            MakePinInput(_scl);
        }

        private void releaseSclWithStretch()
        {
            //
            // Clock stretching - Makes SCL an input and pull-up resistor makes
            //  the pin high.  Slave device can pull SCL low to extend clock cycle.
            long endStretch = Utility.GetMachineTime().Ticks + _timeOutTicks;
            while (!readScl())
            {
                // How long have we been stuck in the while loop?
                if (Utility.GetMachineTime().Ticks >= endStretch)
                    throw new TimeOutException(); // Too long, so bail out by throwing an exception.
            }
        }

        private bool readScl()
        {
            MakePinInput( _scl );
            return _scl.Read();
        }

        private bool readSda()
        {
            MakePinInput( _sda );
            return _sda.Read();
        }

        private bool readBit()
        {
            // "ReadSDA" makes SDA an input - processor lets go of pin and internal
            //  pull-up resistor makes it high.  Now slave can drive the pin.
            releaseSda();

            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            
            releaseSclWithStretch();

            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);

            // At this point, SCL is high and SDA is valid - so read the bit.
            bool bit = _sda.Read();

            clearScl(); // Pull the serial clock line low ...

            return bit; //  and return.
        }

        private bool writeBit( bool bit )
        {
            if( bit )
            {
                releaseSda(); // Make SDA an input ... so pin is pulled up.
            }
            else
            {
                clearSda(); // Make SDA an output ... so pin is pulled low.
            }

            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);

            releaseSclWithStretch();

            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            
            // SCL is high and SDA is valid ...
            //  Check that nobody else is driving SDA
            if( bit &&
                _isMultimaster &&
                !_sda.Read() )
            {
                return false; 
            }

            clearScl();

            return true; // Success!
        }

        public void WaitForNotBusy()
        {
            long endWait = Utility.GetMachineTime().Ticks + _timeOutTicks;
            //wait for the line to go high

            MakePinInput(_sda);

            while( !_sda.Read() )
            {
                // How long have we been stuck in the while loop?
                if( Utility.GetMachineTime().Ticks>=endWait )
                    throw new TimeOutException(); // Too long, so bail out by throwing an exception.
            }
        }

        public bool SendStart()
        {
            //if not started, SCL & SDL should be high
            if( _isStarted )
            {
                //if doing a reset, set SDA to 1 
                releaseSda();
                releaseSclWithStretch();
                I2Cdelay(I2_C_SPEED);
                I2Cdelay(I2_C_SPEED);
                I2Cdelay(I2_C_SPEED);
                I2Cdelay(I2_C_SPEED);
            }

            if( _isMultimaster &&
                !_sda.Read() )
            {
                //Lost arbitration?
                return false;
            }

            // SCL is high, set SDA from 1 to 0 => that's the start condition
            clearSda();
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            clearScl();

            _isStarted = true;

            return true;
        }

        public bool SendStop()
        {
            // set SDA to 0 
            clearSda();
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            releaseSclWithStretch();
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            releaseSda();
            _isStarted = false;

            I2Cdelay(I2_C_SPEED);
            I2Cdelay(I2_C_SPEED);
            // SCL is high, set SDA from 0 to 1 
            if( _isMultimaster &&
                !_sda.Read() )                 
            {
                return false;
            }

            return true;
        }


        public bool Transmit( byte byteToSend, bool sendFakeAck = false )
        {
            writeBit( ( byteToSend & 0x80 ) != 0 );
            writeBit( ( byteToSend & 0x40 ) != 0 );
            writeBit( ( byteToSend & 0x20 ) != 0 );
            writeBit( ( byteToSend & 0x10 ) != 0 );
            writeBit( ( byteToSend & 0x08 ) != 0 );
            writeBit( ( byteToSend & 0x04 ) != 0 );
            writeBit( ( byteToSend & 0x02 ) != 0 );
            writeBit( ( byteToSend & 0x01 ) != 0 );
            if( sendFakeAck )
            {
                writeBit( false );
                return true;
            }
            bool nack = readBit();
            return !nack;
        }


        public byte Receive( bool sendAcknowledgeBit )
        {
            byte data = 0;
            if( readBit() ) data |= 0x80;
            if( readBit() ) data |= 0x40;
            if( readBit() ) data |= 0x20;
            if( readBit() ) data |= 0x10;
            if( readBit() ) data |= 0x08;
            if( readBit() ) data |= 0x04;
            if( readBit() ) data |= 0x02;
            if( readBit() ) data |= 0x01;
            writeBit( !sendAcknowledgeBit );
            return data;
        }


        public int ErrorCount
        {
            get { return _errorCount; }
        }

        public void SendWakeup()
        {
            SendStart();
            Transmit(0x01, true); //allow CPU to recognise we are active            
        }

        public byte[] ReadRegisters(int address, byte startRegister, int readBytes)
        {
            var buffer = new byte[readBytes];
            for (int i = 0; i < RETRY_MAX; i++)
            {
                try
                {
                    if (i > 0)
                    {
                        if (_isStarted)
                        {
                            SendStop();
                        }
                        Debug.Print("ReadRegisters: i2c error");
                        Thread.Sleep(20);
                        _errorCount++;
                    }

                    WaitForNotBusy();
                    SendStart();
                    if (!Transmit((byte)(address + WRITE))) continue;
                    if (!Transmit(startRegister)) continue;

                    if (UseUltrasonicHack)
                    {
                        //pulse the clock
                        readScl();
                        I2Cdelay(I2_C_SPEED);
                        releaseSclWithStretch();
                    }

                    SendStart();
                    if (!Transmit((byte)(address + READ))) continue;
                    for (int j = 0; j < readBytes; j++)
                    {
                        //the other side will keep sending bytes until we are done
                        buffer[j] = Receive(j + 1 < readBytes);   //send ack until last byte, then send nack
                    }
                    SendStop();
                    return buffer;
                }
                catch (IOException)
                {
                    Debug.Print("IO Timed out");
                }
            }
            throw new IOException("ReadRegisters keep nacking");
        }


        public void WriteRegister( int address, byte register, int value, bool is16 )
        {
            for( int i = 0; i < RETRY_MAX; i++ )
            {
                try
                {
                    if( i > 0 )
                    {
                        if( _isStarted )
                        {
                            SendStop();
                        }
                        Debug.Print( "WriteRegister: i2c error" );
                        Thread.Sleep(20);
                        _errorCount++;
                    }

                    WaitForNotBusy();
                    SendStart();
                    if( !Transmit( (byte)( address + WRITE ) ) ) continue;
                    if( !Transmit( register ) ) continue;
                    if( is16 )
                    {
                        if( !Transmit( (byte)( value >> 8 ) ) ) continue;
                    }
                    if( !Transmit( (byte)( value & 0xFF ) ) ) continue;
                    SendStop();
                    return;
                }
                catch( IOException )
                {
                    Debug.Print( "IO Timed out" );
                }
            }
            throw new IOException( "WriteRegisters keep nacking" );
        }


        public int ReadRegister( int address, byte register, bool is16 )
        {
            for( int i = 0; i < RETRY_MAX; i++ )
            {
                try
                {
                    if( i > 0 )
                    {
                        if( _isStarted )
                        {
                            SendStop();
                        }
                        Debug.Print( "ReadRegister: i2c error" );
                        Thread.Sleep(20);
                        _errorCount++;
                    }
                    WaitForNotBusy();
                    //var sw = Stopwatch.StartNew();

                    SendStart();

                    if( !Transmit( (byte)( address + WRITE ) ) ) continue;
                    if( !Transmit( register ) ) continue;

                    if( UseUltrasonicHack )
                    {
                        //pulse the clock
                        readScl();
                        I2Cdelay( I2_C_SPEED );
                        releaseSclWithStretch();
						
                    }

                    SendStart();
                    if( !Transmit( (byte)( address + READ ) ) ) continue;
                    int value = Receive( is16 ); //send ack if reading another byte
                    if( is16 )
                    {
                        byte lsb = Receive( false ); //we're done, so nack
                        value = ( value << 8 ) + lsb;
                    }
                    SendStop();

                    //var t = sw.ElapsedMilliseconds;
                    //Debug.Print( "ReadRegister= "+t+"ms");

                    return value;
                }
                catch( IOException )
                {
                    Debug.Print( "IO Timed out" );
                }
            }
            throw new IOException( "ReadRegister keep nacking" );
        }
    }
}
