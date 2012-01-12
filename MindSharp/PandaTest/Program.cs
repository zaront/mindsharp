using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using MindSharp;

namespace PandaTest
{
    public class Program
    {
        public static void Main()
        {
            var shield = new NxShield(new PandaPinMap());

            //FlashLights(shield);
            I2CTest(shield);
        }

        private static void I2CTest(NxShield shield)
        {
            var controller = new PspNx(shield, SensorPlug.BAS1);
            Thread.Sleep(1000);
            for (int i = 0; i < 10000000; i++)
            {
                controller.UpdateButtonState();
            }
            
            //NxtMotor motor = new NxtMotor(shield, MotorPlug.BankA_M1);
        }

        private static void FlashLights(NxShield shield)
        {
            var leds = new OnboardLeds(shield);
            var light = new NxtLight(shield, SensorPlug.BAS2);

            while (true)
            {
                leds.Blue = false;
                leds.Red = true;
                light.Light = true;

                Thread.Sleep(500);


                leds.Red = false;
                light.Light = false;
                leds.Green = true;

                
                Thread.Sleep(500);

                Debug.Print(light.GetBrightness().ToString());

                leds.Green = false;
                leds.Blue = true;
                Thread.Sleep(500);
            }
        }

    }
}
