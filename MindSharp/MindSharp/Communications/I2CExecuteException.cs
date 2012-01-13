using System;
using Microsoft.SPOT;

namespace MindSharp.Communications
{
	public class I2CExecuteException : Exception
	{
		public int SizeTranferred { get; private set; }

		public I2CExecuteException(int sizetransferred, string msg = null)
			: base(msg)
		{
			SizeTranferred = sizetransferred;
		}
	}
}
