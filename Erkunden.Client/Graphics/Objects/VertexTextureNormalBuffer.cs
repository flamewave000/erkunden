using Erkunden.Client.Graphics.Data;
using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.Graphics.Objects
{
	public class VertexTextureNormalBuffer : GraphicsObject
	{
		#region Fields
		private VertexBuffer internalBuffer;
		public int Length { get; private set; }
		#endregion

		#region Constructors
		private VertexTextureNormalBuffer(VertexBuffer internalBuffer)
		{
			this.internalBuffer = internalBuffer;
			Handle = internalBuffer.Handle;
			internalBuffer.Bind();
		}
		private VertexTextureNormalBuffer(VertexBuffer internalBuffer, in VertexTextureNormal[] data) : this(internalBuffer) => SetData(in data);
		#endregion

		/// <summary>
		/// Defines and enables the attribute pointers for the currently bound <see cref="VertexArrayObject"/>.
		/// </summary>
		/// <param name="vertAttribLocation">Vertex/Position Shader Attribute Location.<code>/* Default */ layout(location = 0) in vec3 a_Position</code></param>
		/// <param name="texcAttribLocation">Texture Coordinate Shader Attribute Location.<code>/* Default */ layout(location = 1) in vec2 a_TexCoord</code></param>
		/// <param name="normAttribLocation">Normal Shader Attribute Location.<code>/* Default */ layout(location = 2) in vec3 a_Normal</code></param>
		public void EnableAttribPointers(
			in int vertAttribLocation = 0,
			in int texcAttribLocation = 1,
			in int normAttribLocation = 2)
		{
			// Define the properties of each array
			GL.VertexAttribPointer(vertAttribLocation, 3, VertexAttribPointerType.Float, false, VertexTextureNormal.Stride, VertexTextureNormal.VertexOffset);
			GL.VertexAttribPointer(texcAttribLocation, 2, VertexAttribPointerType.Float, false, VertexTextureNormal.Stride, VertexTextureNormal.TextureOffset);
			GL.VertexAttribPointer(normAttribLocation, 3, VertexAttribPointerType.Float, false, VertexTextureNormal.Stride, VertexTextureNormal.NormalOffset);
			// Enable each attribute
			GL.EnableVertexAttribArray(vertAttribLocation);
			GL.EnableVertexAttribArray(texcAttribLocation);
			GL.EnableVertexAttribArray(normAttribLocation);
		}
		public void SetData(in VertexTextureNormal[] data)
		{
			Length = data.Length;
			internalBuffer.SetData(data, BufferUsageHint.StaticDraw);
		}


		#region GraphicsObject Implementation
		public void Unbind() => internalBuffer.Unbind();
		public override void Bind() => internalBuffer.Bind();
		public override void Dispose() { internalBuffer.Dispose(); Handle = 0; }
		#endregion

		#region Factory Methods
		/// <summary>Creates and Binds a new empty <see cref="VertexTextureNormalBuffer"/>.</summary>
		public static VertexTextureNormalBuffer Create() =>
			new VertexTextureNormalBuffer(VertexBuffer.Create(BufferTarget.ArrayBuffer));
		/// <summary>Creates and Binds a new <see cref="VertexTextureNormalBuffer"/> and prefills it with the provided data.</summary>
		/// <param name="data">Data to be copied to the internal Vertex Buffer Object.</param>
		public static VertexTextureNormalBuffer Create(in VertexTextureNormal[] data) =>
			new VertexTextureNormalBuffer(VertexBuffer.Create(BufferTarget.ArrayBuffer), in data);
		#endregion
	}
}
