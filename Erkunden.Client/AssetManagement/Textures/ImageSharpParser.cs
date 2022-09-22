using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Erkunden.Core.Utils;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Erkunden.Client.AssetManagement.Textures
{
	public class UnsupportedPixelSizeException : Exception { }
	public class ImageSharpParser : AssetParser
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Register() => AssetProvider.RegisterParser(new ImageSharpParser());

		public string[] Extensions { get; } = new string[] { ".gif", ".jpg", ".jpeg", ".bmp", ".pbm", ".png", ".tga", ".tiff", ".webp" };

		public void Parse(FileInfo file, AssetStore store)
		{
			// Load the image
			ImageData imageData = LoadImage(file);
			store.Add(
				AssetParser.ConformName<Texture>(imageData.Name),
				Texture.Create(imageData, false)
			);
			Log.WriteLine("@green;Loaded Texture:@magenta;  " + imageData.Name, indent: true);
		}

		public IEnumerable<string> GetNames(FileInfo file) => new string[] {
			AssetParser.ConformName<Texture>(Path.GetFileNameWithoutExtension(file.Name))
		};

		private void GetImagePixelData<TPixel>(Stream stream, ref ImageData imageData)
			where TPixel : unmanaged, IPixel<TPixel>
		{
			Image<TPixel> image = null!;
			try
			{
				image = Image.Load<TPixel>(stream);
				// Flip the data of the image
				// ImageSharp data starts image arrays at the top-left, but OpenGL starts Bottom-Left
				image.Mutate(x => x.Flip(FlipMode.Vertical));

				imageData.Width = image.Width;
				imageData.Height = image.Height;
				imageData.Pixels = new byte[(image.PixelType.BitsPerPixel / 8) * (image.Width * image.Height)];
				image.CopyPixelDataTo(imageData.Pixels);
			}
			finally
			{
				if (image != null)
					image.Dispose();
			}
		}

		private ImageData LoadImage(FileInfo file)
		{
			ImageData imageData = new ImageData();
			imageData.Name = Path.GetFileNameWithoutExtension(file.Name);

			using (FileStream stream = file.OpenRead())
			{
				var imageInfo = Image.Identify(stream);
				stream.Seek(0, SeekOrigin.Begin);
				switch (imageInfo.PixelType.BitsPerPixel)
				{
					case 8:
						imageData.Format = PixelFormat.Red;
						imageData.InternalFormat = PixelInternalFormat.R8;
						GetImagePixelData<L8>(stream, ref imageData);
						break;
					case 24:
						imageData.Format = PixelFormat.Rgb;
						imageData.InternalFormat = PixelInternalFormat.Rgb;
						GetImagePixelData<Rgb24>(stream, ref imageData);
						break;
					case 32:
						imageData.Format = PixelFormat.Rgba;
						imageData.InternalFormat = PixelInternalFormat.Rgba;
						GetImagePixelData<Rgba32>(stream, ref imageData);
						break;
					default: throw new UnsupportedPixelSizeException();
				}
			}
			return imageData;
		}
	}
}
