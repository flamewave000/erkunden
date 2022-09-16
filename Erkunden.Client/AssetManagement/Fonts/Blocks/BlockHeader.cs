using System.Runtime.InteropServices;

namespace Erkunden.Client.AssetManagement.Fonts.Blocks
{
	public enum BlockType : byte
	{
		Info = 1,
		Common = 2,
		Pages = 3,
		Chars = 4,
		KerningPairs = 5
	}

	[StructLayout(LayoutKind.Explicit, Size = 5, Pack = 1)]
	public struct BlockHeader
	{
		[FieldOffset(0)]
		public BlockType Type;
		[FieldOffset(1)]
		public int Size;
		public static readonly int ByteSize = Marshal.SizeOf<BlockHeader>();
	}
}
