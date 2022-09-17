using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Erkunden.Client.Graphics.Objects
{
	public class Texture2D : GraphicsObject
	{
		public PixelInternalFormat Format { get; private set; } = PixelInternalFormat.Rgba;
		public int Width { get; private set; } = 0;
		public int Height { get; private set; } = 0;
		public Vector2 Size { get; private set; } = Vector2.Zero;

		private Texture2D(int handle)
		{
			Handle = handle;
		}

		public override void Bind()
		{
			GL.BindTexture(TextureTarget.Texture2D, Handle);
		}
		public void SetData(PixelFormat format, PixelInternalFormat internalFormat, int width, int height, bool generateMipMaps, ref byte[] pixels)
		{
			Width = width;
			Height = height;
			Size = new Vector2(width, height);
			Format = internalFormat;
			GL.TexImage2D(
				target: TextureTarget.Texture2D,
				level: 0, // For reducing the max number of MipMaps, 0 allows OpenGL to figure it out
				internalformat: internalFormat, // Internal Pixel Format
				width: width, // Width of the image
				height: height, // Height of the image
				border: 0, // Legacy value, always keep at 0
				format: format, // Format of pixel data coming in
				type: PixelType.UnsignedByte, // Type of the array elements
				pixels: pixels // Pixel data to be copied to the GPU
			);
			if (generateMipMaps)
				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}

		public override void Dispose()
		{
			if (IsDisposed) return;
			GL.DeleteTexture(Handle);
			Handle = 0;
		}

		public static Texture2D Create() => new Texture2D(GL.GenTexture());
	}
}
