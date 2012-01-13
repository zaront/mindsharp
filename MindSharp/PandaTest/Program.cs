using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using MindSharp;
using MindSharp.Peripherals;

namespace PandaTest
{
    public class Program
    {
        public static void Main()
        {
			Debug.EnableGCMessages(false);

            var shield = new NxShield(new PandaPinMap());

			PSControllerTest(shield);
            //FlashLights(shield);
            //I2CTest(shield);
        }

		private static void PSControllerTest(NxShield shield)
		{
			//setup
			var controller = new PspNx(shield, SensorPlug.BAS1);

			//output sensor data that has changed
			PspState state = new PspState();
			PspState prevState = new PspState();
			while (true)
			{
				if (controller.UpdateButtonState(state))
				{
					//compare values output diffrent ones
					if (state.A != prevState.A) Debug.Print("A : " + state.A.ToString());
					if (state.B != prevState.B) Debug.Print("B : " + state.B.ToString());
					if (state.B != prevState.C) Debug.Print("C : " + state.C.ToString());
					if (state.D != prevState.D) Debug.Print("D : " + state.D.ToString());
					if (state.Circle != prevState.Circle) Debug.Print("Circle : " + state.Circle.ToString());
					if (state.Cross != prevState.Cross) Debug.Print("Cross : " + state.Cross.ToString());
					if (state.Square != prevState.Square) Debug.Print("Square : " + state.Square.ToString());
					if (state.Triangle != prevState.Triangle) Debug.Print("Triangle : " + state.Triangle.ToString());
					if (state.L1 != prevState.L1) Debug.Print("L1 : " + state.L1.ToString());
					if (state.L2 != prevState.L2) Debug.Print("L2 : " + state.L2.ToString());
					if (state.R1 != prevState.R1) Debug.Print("R1 : " + state.R1.ToString());
					if (state.R2 != prevState.R2) Debug.Print("R2 : " + state.R2.ToString());
					if (state.LeftJoystickButton != prevState.LeftJoystickButton) Debug.Print("LeftJoystickButton : " + state.LeftJoystickButton.ToString());
					if (state.RightJoystickButton != prevState.RightJoystickButton) Debug.Print("RightJoystickButton : " + state.RightJoystickButton.ToString());
					if (state.LeftJoystick != prevState.LeftJoystick) Debug.Print("LeftJoystick : " + state.LeftJoystick.ToString());
					if (state.RightJoystick != prevState.RightJoystick) Debug.Print("RightJoystick : " + state.RightJoystick.ToString());

					//copy over values
					prevState.A = state.A;
					prevState.B = state.B;
					prevState.C = state.C;
					prevState.D = state.D;
					prevState.Circle = state.Circle;
					prevState.Cross = state.Cross;
					prevState.Square = state.Square;
					prevState.Triangle = state.Triangle;
					prevState.L1 = state.L1;
					prevState.L2 = state.L2;
					prevState.R1 = state.R1;
					prevState.R2 = state.R2;
					prevState.LeftJoystickButton = state.LeftJoystickButton;
					prevState.RightJoystickButton = state.RightJoystickButton;
					prevState.LeftJoystick = state.LeftJoystick;
					prevState.RightJoystick = state.RightJoystick;

				}
				else
					Debug.Print("no data");
				//Thread.Sleep(50);
			}
		}

        private static void I2CTest(NxShield shield)
        {
            var controller = new PspNx(shield, SensorPlug.BAS1);
            Thread.Sleep(1000);
            for (int i = 0; i < 10000000; i++)
            {
                PspState buttons = new PspState();
                controller.UpdateButtonState(buttons);
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
