using System;
using System.Runtime.InteropServices;

namespace Erkunden.Client.AssetManagement.Fonts.Blocks
{
	[StructLayout(LayoutKind.Explicit, Size = 10, Pack = 1)]
	public struct KerningBlock
	{
		[FieldOffset(0)]
		public uint First;
		[FieldOffset(4)]
		public uint Second;
		[FieldOffset(8)]
		public short Amount;

		public override string ToString() => $"{Convert.ToChar(First)}:{Convert.ToChar(Second)} -> {Amount}";
		public static readonly int ByteSize = Marshal.SizeOf<KerningBlock>();
	}
}
