using System;

namespace Erkunden.Client.AssetManagement
{
	public abstract class Asset : IDisposable
	{
		public string Name { get; private set; }
		public abstract bool IsDisposed { get; }
		protected Asset(string name) { Name = name; }
		public abstract void Dispose();
	}
}
