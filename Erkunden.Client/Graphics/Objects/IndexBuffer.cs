using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.Graphics.Objects
{
	public class IndexBuffer : GraphicsObject
	{
		#region Fields
		private VertexBuffer internalBuffer;
		public int Length { get; private set; }
		#endregion

		#region Constructors
		private IndexBuffer(VertexBuffer internalBuffer)
		{
			this.internalBuffer = internalBuffer;
			Handle = internalBuffer.Handle;
			internalBuffer.Bind();
		}
		private IndexBuffer(VertexBuffer internalBuffer, in uint[] data) : this(internalBuffer) => SetData(in data);
		#endregion
		/// <summary>Binds the provided data to the internal Vertex Buffer Object.</summary>
		/// <param name="data">Data to be copied to the internal Vertex Buffer Object.</param>
		public void SetData(in uint[] data)
		{
			Length = data.Length;
			internalBuffer.SetData(data, sizeof(uint), BufferUsageHint.StaticDraw);
		}


		#region GraphicsObject Implementation
		public void Unbind() => internalBuffer.Unbind();
		public override void Bind() => internalBuffer.Bind();
		public override void Dispose() { internalBuffer.Dispose(); Handle = 0; }
		#endregion

		#region Factory Methods
		/// <summary>Creates and Binds a new empty <see cref="IndexBuffer"/>.</summary>
		public static IndexBuffer Create() => new IndexBuffer(VertexBuffer.Create(BufferTarget.ElementArrayBuffer));
		/// <summary>Creates and Binds a new <see cref="IndexBuffer"/> and prefills it with the provided data.</summary>
		/// <param name="data">Data to be copied to the internal Vertex Buffer Object.</param>
		public static IndexBuffer Create(in uint[] data) => new IndexBuffer(VertexBuffer.Create(BufferTarget.ElementArrayBuffer), in data);
		#endregion
	}
}
