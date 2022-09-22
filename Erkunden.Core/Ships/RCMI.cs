using System;

namespace Erkunden.Core.Ships
{
	public struct RCMI
	{
		public ulong Value; //0x000000_00_00_000000;

		public char[] Registrar => new char[]
		{
			Convert.ToChar((Value & 0xFF0000_00_00_000000U) >> 56),
			Convert.ToChar((Value & 0x00FF00_00_00_000000U) >> 48),
			Convert.ToChar((Value & 0x0000FF_00_00_000000U) >> 40)
		};

		public string RegistrarStr => "" + Registrar;

		public char Class => Convert.ToChar((Value & 0x000000_FF_00_000000U) >> 32);
		public char Model => Convert.ToChar((Value & 0x000000_00_FF_000000U) >> 24);
		public char Identifier => Convert.ToChar(Value & 0x000000_00_00_FFFFFFU);

		public static implicit operator ulong(RCMI rcmi) => rcmi.Value;
		public static implicit operator RCMI(ulong rcmi) => new RCMI { Value = rcmi };
	}
}
