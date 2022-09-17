using System;
using System.Runtime.InteropServices;

namespace Erkunden.Client.AssetManagement.Fonts.Blocks
{
	[Flags]
	public enum CommonBitField : byte
	{
		_Reserved0 = 0b0000_0001,
		_Reserved1 = 0b0000_0010,
		_Reserved2 = 0b0000_0100,
		_Reserved3 = 0b0000_1000,
		_Reserved4 = 0b0001_0000,
		_Reserved5 = 0b0010_0000,
		_Reserved6 = 0b0100_0000,
		Packed = 0b1000_0000
	}

	[StructLayout(LayoutKind.Explicit, Size = 15, Pack = 1)]
	public struct CommonBlock
	{
		[FieldOffset(0)]
		public ushort LineHeight;
		[FieldOffset(2)]
		public ushort Base;
		[FieldOffset(4)]
		public ushort ScaleW;
		[FieldOffset(6)]
		public ushort ScaleH;
		[FieldOffset(8)]
		public ushort Pages;
		[FieldOffset(10)]
		public CommonBitField BitField;
		[FieldOffset(11)]
		public byte AlphaChnl;
		[FieldOffset(12)]
		public byte RedChnl;
		[FieldOffset(13)]
		public byte GreenChnl;
		[FieldOffset(14)]
		public byte BlueChnl;

		public override string ToString() => $"Lh: {LineHeight}, Pgs: {Pages}";
		public static readonly int ByteSize = Marshal.SizeOf<CommonBlock>();
	}
}
