using Erkunden.Client.AssetManagement.Materials;
using Erkunden.Client.Graphics;
using Erkunden.Client.Graphics.Data;
using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.AssetManagement.Models
{
	public partial class Model : Asset
	{
		public class Part : Asset
		{
			private VertexBuffer? ElementBuffer = null;

			public Material? Material;
			public UIntArrayData Indexes = new UIntArrayData();

			public override bool IsDisposed => ElementBuffer?.IsDisposed ?? true;

			public Part(string name) : base(name) { }
			~Part() { Dispose(); }

			public void Initialize()
			{
				if (!IsDisposed) return;
				ElementBuffer = VertexBuffer.Create(BufferTarget.ElementArrayBuffer);
				ElementBuffer.Bind();
				ElementBuffer.SetData(Indexes.Data, Indexes.TotalByteSize, BufferUsageHint.StaticDraw);
				VertexBuffer.Unbind(BufferTarget.ElementArrayBuffer);
			}

			public override void Dispose()
			{
				ElementBuffer?.Dispose();
			}

			public void Draw()
			{
				if (ElementBuffer == null) return;
				ElementBuffer.Bind();
				GL.DrawElements(PrimitiveType.Triangles, Indexes.Data.Length, DrawElementsType.UnsignedInt, 0);
			}
		}
	}
}
