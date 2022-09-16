using System;
using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.Graphics.Objects
{
	public class VertexBuffer : GraphicsObject
	{
		public readonly BufferTarget Target;
		private VertexBuffer(int handle, BufferTarget target) { Handle = handle; Target = target; }

		public void SetData<T>(T[] data, int elementByteSize, BufferUsageHint hint) where T : struct =>
			SetData(data, data.Length, elementByteSize, hint);
		public void SetData<T>(T[] data, int count, int elementByteSize, BufferUsageHint hint) where T : struct =>
			GL.BufferData(Target, count * elementByteSize, data, hint);

		public void SetSubData<T>(T[] data, int count, int elementByteSize) where T : struct =>
			GL.BufferSubData(Target, (IntPtr)0, count * elementByteSize, data);

		public void Unbind() => GL.BindBuffer(Target, 0);
		public override void Bind() => GL.BindBuffer(Target, Handle);
		public override void Dispose()
		{
			GL.DeleteBuffer(Handle);
			Handle = 0;
		}

		public static VertexBuffer Create(BufferTarget target) => new VertexBuffer(GL.GenBuffer(), target);
	}
}
