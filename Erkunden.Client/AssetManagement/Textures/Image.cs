using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.AssetManagement.Textures
{
	public struct ImageData
	{
		public string Name;
		public int Width;
		public int Height;
		public byte[] Pixels;
		public PixelFormat Format;
		public PixelInternalFormat InternalFormat;
	}
}
