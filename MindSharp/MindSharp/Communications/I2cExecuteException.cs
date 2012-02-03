using System;
using Microsoft.SPOT;

namespace MindSharp.Communications
{
	public class I2cExecuteException : Exception
	{
		public int SizeTranferred { get; private set; }

		public I2cExecuteException(int sizetransferred, string msg = null)
			: base(msg)
		{
			SizeTranferred = sizetransferred;
		}
	}
}
