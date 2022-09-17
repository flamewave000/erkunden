using System;
using System.Runtime.InteropServices;

namespace Erkunden.Client.AssetManagement.Fonts.Blocks
{
	[Flags]
	public enum InfoBitField : byte
	{
		Smooth = 0b0000_0001,
		Unicode = 0b0000_0010,
		Italic = 0b0000_0100,
		Bold = 0b0000_1000,
		FixedHeigth = 0b0001_0000,
		_Reserved5 = 0b0010_0000,
		_Reserved6 = 0b0100_0000,
		_Reserved7 = 0b1000_0000
	}

	[StructLayout(LayoutKind.Explicit, Size = 14, Pack = 1)]
	public struct InfoBlock
	{
		[FieldOffset(0)]
		public ushort FontSize;
		[FieldOffset(2)]
		public InfoBitField BitField;
		[FieldOffset(3)]
		public byte CharSet;
		[FieldOffset(4)]
		public ushort StretchH;
		[FieldOffset(6)]
		public byte AA;
		[FieldOffset(7)]
		public byte PaddingUp;
		[FieldOffset(8)]
		public byte PaddingRight;
		[FieldOffset(9)]
		public byte PaddingDown;
		[FieldOffset(10)]
		public byte PaddingLeft;
		[FieldOffset(11)]
		public byte SpacingHoriz;
		[FieldOffset(12)]
		public byte SpecingVert;
		[FieldOffset(13)]
		public byte Outline;

		public override string ToString() => $"FontSize: {FontSize}";
		public static readonly int ByteSize = Marshal.SizeOf<InfoBlock>();
	}
}
