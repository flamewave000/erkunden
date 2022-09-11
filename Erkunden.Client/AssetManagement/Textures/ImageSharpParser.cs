using System.IO;
using System.Runtime.CompilerServices;
using Erkunden.Core.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Erkunden.Client.AssetManagement.Textures
{
	public class ImageSharpParser : TextureParser
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Register(AssetProvider provider) => provider.RegisterParser<TextureParser, Texture>(new ImageSharpParser());

		public string[] Extensions => new string[] { "gif", "jpg", "jpeg", "bmp", "pbm", "png", "tga", "tiff", "webp" };

		public void Parse(string assetPath, AssetStore<Texture> store, AssetProvider provider)
		{
			Image<Rgba32> image;
			// Load the image
			using (FileStream stream = new FileStream(assetPath, FileMode.Open, FileAccess.Read))
			{
				image = Image.Load<Rgba32>(stream);

			}
			// Flip the data of the image
			// ImageSharp data starts top-left, but OpenGL starts Bottom-Left
			image.Mutate(x => x.Flip(FlipMode.Vertical));
			var name = Path.GetFileNameWithoutExtension(assetPath);
			store.Register(name, new Texture(name, image));
			Log.WriteLine("@green;Loaded Texture:@magenta;  " + name, indent: true);
		}
	}
}
