using Erkunden.Client.Graphics.Objects;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Erkunden.Client.AssetManagement.Textures
{
	public class Texture : Asset
	{
		public Image<Rgba32> Image;
		public Texture2D Texture2D;
		public TextureMinFilter DownscaleTextureFilter = TextureMinFilter.Linear;
		public TextureMagFilter UpscaleTextureFilter = TextureMagFilter.Linear;

		public Texture(string name, Image<Rgba32> image) : base(name)
		{
			Image = image;
			Texture2D = Texture2D.Create();
			byte[] pixels = new byte[4 * Image.Width * Image.Height];
			Image.CopyPixelDataTo(pixels);
			Texture2D.Bind();
			Texture2D.SetData(PixelFormat.Rgba, Image.Width, Image.Height, ref pixels);
		}

		public override bool IsDisposed => Texture2D == null;
		public override void Dispose()
		{
			Image.Dispose();
			Texture2D.Dispose();
		}

		public void Bind(TextureUnit texture)
		{
			GL.ActiveTexture(texture);
			Texture2D.Bind();
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)DownscaleTextureFilter);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)UpscaleTextureFilter);
		}
	}
}
