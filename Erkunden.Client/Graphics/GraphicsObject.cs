using System;

namespace Erkunden.Client.Graphics
{
	public abstract class GraphicsObject : IDisposable
	{
		public int Handle { get; protected set; } = 0;
		public bool IsDisposed => Handle <= 0;
		public abstract void Bind();
		public abstract void Dispose();

		~GraphicsObject()
		{
			try
			{
				Dispose();
			}
			catch { }
		}
	}
}
