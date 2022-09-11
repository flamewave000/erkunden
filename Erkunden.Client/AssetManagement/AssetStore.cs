using System;
using System.Collections.Generic;

namespace Erkunden.Client.AssetManagement
{
	public class AssetStore<T> : IDisposable where T : Asset
	{
		private Dictionary<string, T> Assets = new Dictionary<string, T>();
		public bool Has(string name) => Assets.ContainsKey(name);
		public T Get(string name) => Assets[name];
		public void Register(string name, T asset) => Assets[name] = asset;

		public void Dispose()
		{
			foreach (var asset in Assets.Values) asset.Dispose();
			Assets.Clear();
		}
	}
}
