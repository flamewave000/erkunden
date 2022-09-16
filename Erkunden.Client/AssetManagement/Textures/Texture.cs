using System.Runtime.CompilerServices;
using Erkunden.Client.Graphics.Objects;
using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.AssetManagement.Textures
{
	public class Texture : BaseAsset
	{
		public Texture2D Texture2D { get; } = Texture2D.Create();
		public TextureMinFilter DownscaleTextureFilter = TextureMinFilter.Linear;
		public TextureMagFilter UpscaleTextureFilter = TextureMagFilter.Linear;
		public TextureWrapMode TextureWrapS = TextureWrapMode.Repeat;
		public TextureWrapMode TextureWrapT = TextureWrapMode.Repeat;

		private Texture(string name) : base(name) { }

		public override bool IsDisposed => Texture2D.IsDisposed;
		public override void Dispose() { Texture2D.Dispose(); }

		public void Bind(TextureUnit textureUnit)
		{
			GL.ActiveTexture(textureUnit);
			Texture2D.Bind();
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapS);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapT);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)DownscaleTextureFilter);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)UpscaleTextureFilter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Texture Create(ImageData image, PixelInternalFormat internalFormat, bool generateMipMaps) =>
			CopyImageToTexture(new Texture(image.Name), image, internalFormat, generateMipMaps);

		public static Texture CopyImageToTexture(Texture texture, ImageData image, PixelInternalFormat internalFormat, bool generateMipMaps)
		{
			texture.Texture2D.Bind();
			texture.Texture2D.SetData(image.Format, internalFormat, image.Width, image.Height, generateMipMaps, ref image.Pixels);
			return texture;
		}
	}
}
