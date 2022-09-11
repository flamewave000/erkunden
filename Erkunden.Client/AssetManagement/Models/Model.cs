using Erkunden.Client.Graphics;
using Erkunden.Client.Graphics.Data;
using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.AssetManagement.Models
{
	public partial class Model : Asset
	{
		private bool Ready => VertexArrayObject != null;
		private VertexArrayObject? VertexArrayObject;
		private VertexBuffer? VertexBuffer = null;
		private VertexBuffer? NormalBuffer = null;
		private VertexBuffer? TexCoordBuffer = null;

		public Vector3ArrayData Vertices = new Vector3ArrayData();
		public Vector3ArrayData Normals = new Vector3ArrayData();
		public Vector3ArrayData TexCoords = new Vector3ArrayData();
		public Part[] Parts;

		public override bool IsDisposed => VertexArrayObject?.IsDisposed ?? false;

		public Model(string Name) : base(Name) { Parts = new Part[0]; }
		public Model(string Name, Part[] parts) : base(Name) { Parts = parts; }
		~Model() { Dispose(); }

		public void Initialize(Shader.Program program)
		{
			if (Ready) return;
			VertexArrayObject = VertexArrayObject.Create();
			VertexArrayObject.Bind();

			VertexBuffer = VertexBuffer.Create(BufferTarget.ArrayBuffer);
			VertexBuffer.Bind();
			VertexBuffer.SetData(Vertices.Data, Vertices.TotalByteSize, BufferUsageHint.StaticDraw);
			VertexArrayObject.EnableVector3AttribPointer(program.GetAttribLocation("aPosition"));
			// If there are normals, create the buffer for them
			if (Normals.Data.Length > 0)
			{
				NormalBuffer = VertexBuffer.Create(BufferTarget.ArrayBuffer);
				NormalBuffer.Bind();
				NormalBuffer.SetData(Normals.Data, Normals.TotalByteSize, BufferUsageHint.StaticDraw);
				VertexArrayObject.EnableVector3AttribPointer(program.GetAttribLocation("aNormal"));
			}

			if (TexCoords.Data.Length > 0)
			{
				TexCoordBuffer = VertexBuffer.Create(BufferTarget.ArrayBuffer);
				TexCoordBuffer.Bind();
				TexCoordBuffer.SetData(TexCoords.Data, TexCoords.TotalByteSize, BufferUsageHint.StaticDraw);
				VertexArrayObject.EnableVector3AttribPointer(program.GetAttribLocation("aTexCoord"));
			}

			// Unbind the vertex buffers and object
			VertexBuffer.Unbind(BufferTarget.ArrayBuffer);
			VertexArrayObject.Unbind();
			// Initialize all of the face groups
			foreach (var part in Parts) part.Initialize();
		}

		public override void Dispose()
		{
			if (IsDisposed) return;
			VertexArrayObject?.Dispose();
			VertexBuffer?.Dispose();
			NormalBuffer?.Dispose();
			TexCoordBuffer?.Dispose();
			// Dispose all of the face groups
			foreach (var part in Parts) part.Dispose();
		}

		public void Draw()
		{
			if (VertexArrayObject == null) return;
			VertexArrayObject.Bind();
			foreach (var part in Parts) part.Draw();
		}

	}
}
