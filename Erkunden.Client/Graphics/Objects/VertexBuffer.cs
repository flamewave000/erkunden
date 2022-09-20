using System;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.Graphics.Objects
{
	public class VertexBuffer : GraphicsObject
	{
		public readonly BufferTarget Target;
		public int Capacity { get; private set; } = 0;
		private VertexBuffer(int handle, BufferTarget target) { Handle = handle; Target = target; }

		public void SetData<T>(T[] data, BufferUsageHint hint) where T : struct =>
			SetData(data, data.Length * Unsafe.SizeOf<T>(), hint);
		public void SetData<T>(T[] data, int count, int elementByteSize, BufferUsageHint hint) where T : struct =>
			SetData(data, count * elementByteSize, hint);
		public void SetData<T>(T[] data, int size, BufferUsageHint hint) where T : struct =>
			GL.BufferData(Target, Capacity = size, data, hint);

		public void SetSubData<T>(T[] data, int count, int elementByteSize) where T : struct => SetSubData(data, count * elementByteSize);
		public void SetSubData<T>(T[] data, int size) where T : struct
		{
			if (size >= Capacity) throw new ArgumentOutOfRangeException("count", "Cannot sub data to larger than buffer's capacity.");
			GL.BufferSubData(Target, (IntPtr)0, size, data);
		}

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
