using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.Graphics.Objects
{
	public class Texture2D : GraphicsObject
	{
		private byte[] Pixels = new byte[0];
		private Texture2D(int handle)
		{
			Handle = handle;
		}

		public override void Bind()
		{
			GL.BindTexture(TextureTarget.Texture2D, Handle);
		}
		public void SetData(PixelFormat format, int width, int height, ref byte[] pixels, bool generateMipMaps = false)
		{
			Pixels = pixels;
			GL.TexImage2D(
				target: TextureTarget.Texture2D,
				level: 0, // For reducing the max number of MipMaps, 0 allows OpenGL to figure it out
				internalformat: PixelInternalFormat.Rgba, // Internal Pixel Format
				width: width, // Width of the image
				height: height, // Height of the image
				border: 0, // Legacy value, always keep at 0
				format: format, // Format of pixel data coming in
				type: PixelType.UnsignedByte, // Type of the array elements
				pixels: Pixels // Pixel data to be copied to the GPU
			);
			if (generateMipMaps)
				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}

		public override void Dispose()
		{
			if (IsDisposed) return;
			GL.DeleteTexture(Handle);
		}

		public static Texture2D Create() => new Texture2D(GL.GenTexture());
	}
}
