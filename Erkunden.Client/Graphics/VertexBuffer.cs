using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.Graphics
{
	public class VertexBuffer : GraphicsObject
	{
		public BufferTarget Target { get; private set; }

		private VertexBuffer(int handle, BufferTarget target) { Handle = handle; Target = target; }

		public void SetData<T>(T[] data, int size, BufferUsageHint hint) where T : struct =>
			GL.BufferData(Target, data.Length * size, data, hint);

		public override void Bind() => GL.BindBuffer(Target, Handle);
		public override void Dispose()
		{
			GL.DeleteBuffer(Handle);
			Handle = 0;
		}

		public static VertexBuffer Create(BufferTarget target) =>
			new VertexBuffer(GL.GenBuffer(), target);
		public static VertexBuffer[] Create(BufferTarget[] targets)
		{
			VertexBuffer[] buffers = new VertexBuffer[targets.Length];
			int[] handles = new int[targets.Length];
			GL.GenBuffers(targets.Length, handles);
			for (int c = 0; c < targets.Length; c++)
				buffers[c] = new VertexBuffer(handles[c], targets[c]);
			return buffers;
		}

		public static void Unbind(BufferTarget target) => GL.BindBuffer(target, 0);
	}
}
