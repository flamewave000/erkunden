using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.Graphics.Data;
using Erkunden.Client.Graphics.Objects;

namespace Erkunden.Client.AssetManagement.Models
{
	public partial class Model : BaseAsset
	{
		private VertexArrayObject VertexArrayObject;
		private VertexTextureNormalBuffer VertexBufferObject;
		public Part[] Parts;

		public override bool IsDisposed => VertexArrayObject.IsDisposed;

		public Model(string Name, in VertexTextureNormal[] vertices, Part[] parts) : base(Name)
		{
			Parts = parts;
			VertexArrayObject = VertexArrayObject.Create();
			VertexArrayObject.Bind();
			VertexBufferObject = VertexTextureNormalBuffer.Create(in vertices);
			VertexBufferObject.EnableAttribPointers();
			VertexBufferObject.Unbind();
			VertexArrayObject.Unbind();
		}

		public override void Dispose()
		{
			if (IsDisposed) return;
			VertexArrayObject.Dispose();
			VertexBufferObject.Dispose();
			// Dispose all of the face groups
			foreach (var part in Parts) part.Dispose();
		}

		public void Draw(Shader shader)
		{
			if (VertexArrayObject == null) return;
			VertexArrayObject.Bind();
			foreach (var part in Parts) part.Draw(shader);
		}
	}
}
