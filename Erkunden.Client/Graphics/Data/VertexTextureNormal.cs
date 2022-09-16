using System.Runtime.CompilerServices;
using OpenTK.Mathematics;

namespace Erkunden.Client.Graphics.Data
{
	public struct VertexTextureNormal
	{
		public Vector3 Vertex;
		public Vector2 Texture;
		public Vector3 Normal;

		public VertexTextureNormal(Vector3 vertex, Vector2 texture, Vector3 normal)
		{
			Vertex = vertex;
			Texture = texture;
			Normal = normal;
		}

		public readonly static int VertexOffset = 0;
		public readonly static int TextureOffset = Vector3.SizeInBytes;
		public readonly static int NormalOffset = Vector3.SizeInBytes + Vector2.SizeInBytes;
		public readonly static int SizeInBytes = Unsafe.SizeOf<VertexTextureNormal>();
		public readonly static int Stride = SizeInBytes;
	}
}
