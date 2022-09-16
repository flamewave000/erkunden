using System.Runtime.InteropServices;
using System.Text;

namespace Erkunden.Client.AssetManagement.Fonts.Blocks
{
	public enum VersionCode : byte
	{
		Version1 = 1,
		Version2 = 2,
		Version3 = 3
	}

	[StructLayout(LayoutKind.Explicit, Size = 4, Pack = 1, CharSet = CharSet.Ansi)]
	public struct FileHeaderBlock
	{
		[FieldOffset(0)]
		private byte ID0;
		[FieldOffset(0)]
		private byte ID1;
		[FieldOffset(0)]
		private byte ID2;
		[FieldOffset(3)]
		public VersionCode Version;
		public string Identifier => Encoding.ASCII.GetString(new byte[] { ID0, ID1, ID2 });
		public static readonly int ByteSize = Marshal.SizeOf<FileHeaderBlock>();
	}
}
