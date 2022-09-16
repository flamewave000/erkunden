using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Erkunden.Client.AssetManagement.Fonts.Blocks;
using Erkunden.Client.AssetManagement.Textures;

namespace Erkunden.Client.AssetManagement.Fonts
{
	public class BMFParser : AssetParser
	{
		public static void Register() => AssetProvider.RegisterParser(new BMFParser());
		public string[] Extensions { get; } = new string[] { ".fnt" };

		public IEnumerable<string> GetNames(FileInfo file)
		{
			using (BinaryReader reader = new BinaryReader(file.OpenRead()))
			{
				reader.BaseStream.Position = FileHeaderBlock.ByteSize;
				BlockHeader block;
				ParseStruct(reader, out block, BlockHeader.ByteSize);
				InfoBlock info;
				ParseStruct(reader, out info, InfoBlock.ByteSize);
				yield return AssetParser.ConformName<Font>(
					CString.Convert(reader.ReadBytes(block.Size - InfoBlock.ByteSize)) + info.FontSize
				);
			}
		}

		public void Parse(FileInfo file, AssetStore store)
		{
			string name;
			FontMeta meta;
			FontBody body;
			BlockHeader block;
			using (BinaryReader reader = new BinaryReader(file.OpenRead()))
			{
				// Parse the File Header and Info Block
				ParseStruct(reader, out meta.FileHeader, FileHeaderBlock.ByteSize);

				/** INFO BLOCK **/
				// Read the Info Block header
				ParseStruct(reader, out block, BlockHeader.ByteSize);
				// Read the Info Block
				ParseStruct(reader, out meta.Info, InfoBlock.ByteSize);
				// Read the Font Name
				name = CString.Convert(reader.ReadBytes(block.Size - InfoBlock.ByteSize)) + meta.Info.FontSize;

				/** COMMON BLOCK **/
				// Skip Common Block header (block is fixed size)
				reader.BaseStream.Position += BlockHeader.ByteSize;
				// Read the Common Block
				ParseStruct(reader, out body.Common, CommonBlock.ByteSize);

				/** PAGE BLOCKS **/
				// Read the Page Block header
				ParseStruct(reader, out block, BlockHeader.ByteSize);
				// Determine the Uniform size of the Page names
				int pageLength = CString.Length(reader);
				// Calculate the number of pages (Rarely more than 1)
				int pageCount = block.Size / pageLength;
				// Initialize the pages array to the number of pages
				body.Pages = new string[pageCount];
				// Read all of the page names
				for (int c = 0; c < pageCount; c++)
					body.Pages[c] = CString.Convert(reader.ReadBytes(pageLength));

				/** CHARACTER BLOCKS **/
				// Read the Character Block array header
				ParseStruct(reader, out block, BlockHeader.ByteSize);
				// Read the array of Character Blocks
				ParseStructArray(reader, out body.Chars, block.Size / CharBlock.ByteSize, CharBlock.ByteSize);

				/** KERNING BLOCKS **/
				// Read the Kerning Block array header
				ParseStruct(reader, out block, BlockHeader.ByteSize);
				// Read the array of Kerning Blocks
				ParseStructArray(reader, out body.Kernings, block.Size / KerningBlock.ByteSize, KerningBlock.ByteSize);
			}
			Texture[] sprites = new Texture[body.Pages.Length];
			// Load each sprite image and add it to the array of textures
			for (int c = 0; c < sprites.Length; c++)
				sprites[c] = AssetProvider.Get<Texture>(Path.GetFileNameWithoutExtension(body.Pages[c]));
			// Create the Font object and add it to the asset store
			store.Add(AssetParser.ConformName<Font>(name), new Font(name, meta, body, sprites));
		}

		#region Marshal Parsing
		private void ParseStruct<T>(BinaryReader reader, out T value, int byteSize = -1) where T : struct
		{
			if (byteSize < 0) byteSize = Marshal.SizeOf<T>();
			byte[] bytes = reader.ReadBytes(byteSize);
			GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			try
			{
				value = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T))!;
			}
			finally { if (handle.IsAllocated) handle.Free(); }
		}
		private void ParseStructArray<T>(BinaryReader reader, out T[] values, int count, int byteSize = -1) where T : struct
		{
			if (byteSize < 0) byteSize = Marshal.SizeOf<T>();
			values = new T[count];
			byte[] data = reader.ReadBytes(count * byteSize);
			GCHandle handle = GCHandle.Alloc(values, GCHandleType.Pinned);
			try
			{
				Marshal.Copy(data, 0, handle.AddrOfPinnedObject(), data.Length);
			}
			finally { if (handle.IsAllocated) handle.Free(); }
		}
		#endregion
	}
}
