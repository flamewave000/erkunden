using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace Erkunden.Client.AssetManagement.Fonts.Blocks
{
	[StructLayout(LayoutKind.Explicit, Size = 20, Pack = 1)]
	public struct CharBlock
	{
		[FieldOffset(0)]
		public uint Id;
		[FieldOffset(4)]
		public ushort X;
		[FieldOffset(6)]
		public ushort Y;
		[FieldOffset(8)]
		public ushort Width;
		[FieldOffset(10)]
		public ushort Height;
		[FieldOffset(12)]
		public short XOffset;
		[FieldOffset(14)]
		public short YOffset;
		[FieldOffset(16)]
		public short XAdvance;
		[FieldOffset(18)]
		public byte Page;
		[FieldOffset(19)]
		public byte Chnl;
		public static readonly int ByteSize = Marshal.SizeOf<CharBlock>();
	}

	public struct ScaledCharBlock
	{
		public Vector2 GlyphOffset;
		public Vector2 GlyphSize;
		public Vector2 DrawSize;
		public Vector2 DrawOffset;
		public float XAdvance;
		public int Page;
		public int Chnl;
	}
}
