using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Erkunden.Client.AssetManagement.Fonts
{
	public static class CString
	{
		public static string Dereference(IntPtr ptr) => Marshal.PtrToStringAnsi(ptr)!;

		public static string Convert(byte[] bytes)
		{
			var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			try
			{
				return Marshal.PtrToStringAnsi(handle.AddrOfPinnedObject())!;
			}
			finally { handle.Free(); }
		}

		public static string ReadString(BinaryReader binary)
		{
			List<byte> bytes = new List<byte>();
			byte c;
			while ((c = binary.ReadByte()) != 0)
				bytes.Add(c);
			return Convert(bytes.ToArray());
		}

		public static int Length(byte[] bytes, int offset = 0)
		{
			int count = 0;
			while (bytes[offset + count++] != 0) ;
			return count;
		}

		public static int Length(BinaryReader binary, bool includeTerminal = false)
		{
			long position = binary.BaseStream.Position;
			int count = 0;
			while (binary.ReadByte() != 0) count++;
			binary.BaseStream.Position = position;
			return includeTerminal ? 1 + count : count;
		}
	}
}
