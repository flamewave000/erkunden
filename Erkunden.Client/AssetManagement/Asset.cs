using System;

namespace Erkunden.Client.AssetManagement
{
	public interface Asset : IDisposable
	{
		string Name { get; }
		bool IsDisposed { get; }
	}

	public abstract class BaseAsset : Asset
	{
		public string Name { get; private set; }
		public abstract bool IsDisposed { get; }

		protected BaseAsset(string name) { Name = name; }
		~BaseAsset() { Dispose(); }

		public abstract void Dispose();
	}
}
