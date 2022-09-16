using System;
using System.Collections.Generic;

namespace Erkunden.Client.AssetManagement
{
	public class AssetStore : IDisposable
	{
		private Dictionary<string, Asset> assets = new Dictionary<string, Asset>();

		public bool Has(string name)
		{
			assets.TryGetValue(name, out var assetRef);
			return assetRef != null;
		}
		public bool TryGet(string name, out Asset? asset)
		{
			assets.TryGetValue(name, out asset);
			return asset != null;
		}
		public Asset? Get(string name) => assets.TryGetValue(name, out var assetRef) ? assetRef : null;
		public void Add(in string name, in Asset asset) => assets[name] = asset;
		public bool Remove(in string name) => assets.Remove(name);

		public void Dispose()
		{
			assets.Clear();
		}
	}
}
