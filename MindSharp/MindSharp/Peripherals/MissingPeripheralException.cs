using System;
using Microsoft.SPOT;

namespace MindSharp.Peripherals
{
	public class MissingPeripheralException : ApplicationException
	{
		public MissingPeripheralException()
			: base("Peripheral not found")
		{
		}

		public MissingPeripheralException(string message)
			: base(message)
		{
		}
	}
}
