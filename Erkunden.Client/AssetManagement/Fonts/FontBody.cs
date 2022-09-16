using System.Runtime.InteropServices;
using Erkunden.Client.AssetManagement.Fonts.Blocks;

namespace Erkunden.Client.AssetManagement.Fonts
{
	[StructLayout(LayoutKind.Sequential)]
	public struct FontBody
	{
		public CommonBlock Common;
		public string[] Pages;
		public CharBlock[] Chars;
		public KerningBlock[] Kernings;
		public static readonly int ByteSize = Marshal.SizeOf<FontBody>();
	}
}
