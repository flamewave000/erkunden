using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Erkunden.Client.Graphics
{
	public class VertexArrayObject : GraphicsObject
	{
		private VertexArrayObject(int handle) { Handle = handle; }

		public override void Bind() => GL.BindVertexArray(Handle);
		public override void Dispose()
		{
			GL.DeleteVertexArray(Handle);
			Handle = 0;
		}

		#region Attribute Pointer Definitions
		public void EnableVector2iAttribPointer(int attributeIndex)
		{
			GL.VertexAttribPointer(attributeIndex, 2, VertexAttribPointerType.Int, false, Vector2i.SizeInBytes, 0);
			GL.EnableVertexAttribArray(attributeIndex);
		}
		public void EnableVector3iAttribPointer(int attributeIndex)
		{
			GL.VertexAttribPointer(attributeIndex, 3, VertexAttribPointerType.Int, false, Vector3i.SizeInBytes, 0);
			GL.EnableVertexAttribArray(attributeIndex);
		}
		public void EnableVector4iAttribPointer(int attributeIndex)
		{
			GL.VertexAttribPointer(attributeIndex, 4, VertexAttribPointerType.Int, false, Vector4i.SizeInBytes, 0);
			GL.EnableVertexAttribArray(attributeIndex);
		}

		public void EnableVector2AttribPointer(int attributeIndex)
		{
			GL.VertexAttribPointer(attributeIndex, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
			GL.EnableVertexAttribArray(attributeIndex);
		}
		public void EnableVector3AttribPointer(int attributeIndex)
		{
			GL.VertexAttribPointer(attributeIndex, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
			GL.EnableVertexAttribArray(attributeIndex);
		}
		public void EnableVector4AttribPointer(int attributeIndex)
		{
			GL.VertexAttribPointer(attributeIndex, 4, VertexAttribPointerType.Float, false, Vector4.SizeInBytes, 0);
			GL.EnableVertexAttribArray(attributeIndex);
		}
		#endregion

		public static VertexArrayObject Create() => new VertexArrayObject(GL.GenVertexArray());
		public static void Unbind() => GL.BindVertexArray(0);
	}
}
