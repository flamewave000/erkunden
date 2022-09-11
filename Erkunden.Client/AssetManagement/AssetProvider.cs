using System;
using System.Collections.Generic;
using System.IO;
using Erkunden.Client.AssetManagement.Materials;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.AssetManagement.Textures;
using Erkunden.Core.Util;

namespace Erkunden.Client.AssetManagement
{
	public partial class AssetProvider : IDisposable
	{
		private static Type ModelType = typeof(Model);
		private static Type MaterialType = typeof(Material);
		private static Type TextureType = typeof(Texture);

		#region Track all files that have already been loaded
		private HashSet<string> LoadedAssets = new HashSet<string>();
		#endregion

		#region Hold dictionaries of all available parsers
		private Dictionary<string, ModelParser> ModelParsers = new Dictionary<string, ModelParser>();
		private Dictionary<string, MaterialParser> MaterialParsers = new Dictionary<string, MaterialParser>();
		private Dictionary<string, TextureParser> TextureParsers = new Dictionary<string, TextureParser>();
		#endregion

		#region Store all models, materials, and textures
		private AssetStore<Model> Models = new AssetStore<Model>();
		private AssetStore<Material> Materials = new AssetStore<Material>();
		private AssetStore<Texture> Textures = new AssetStore<Texture>();
		#endregion

		#region Asset Accessors
		public bool Has<T>(string name) where T : Asset
		{
			var type = typeof(T);
			if (type == ModelType) return Models.Has(name);
			if (type == MaterialType) return Materials.Has(name);
			if (type == TextureType) return Textures.Has(name);
			return false;
		}
		public T Get<T>(string name) where T : Asset
		{
			var type = typeof(T);
			if (type == ModelType) return (T)(object)Models.Get(name);
			if (type == MaterialType) return (T)(object)Materials.Get(name);
			if (type == TextureType) return (T)(object)Textures.Get(name);
			throw new KeyNotFoundException();
		}
		#endregion

		public void RegisterParser<TParser, TAsset>(TParser parser) where TAsset : Asset where TParser : AssetParser<TAsset>
		{
			if (parser is ModelParser)
				foreach (var ext in parser.Extensions) ModelParsers[ext] = (ModelParser)parser;
			else if (parser is MaterialParser)
				foreach (var ext in parser.Extensions) MaterialParsers[ext] = (MaterialParser)parser;
			else if (parser is TextureParser)
				foreach (var ext in parser.Extensions) TextureParsers[ext] = (TextureParser)parser;
		}

		public void LoadAsset(string path, string? relativeTo = null)
		{
			path = FileUtil.PlatformPath(path);
			// Ignore asset files that have already been loaded
			if (LoadedAssets.Contains(path)) return;

			Log.WriteLine($"@blue;Loading Assets from:@clr; {path}");

			// Otherwise track the new file path and proceed
			LoadedAssets.Add(path);

			// Attempt to load all dependencies
			if (relativeTo != null)
				path = Path.Combine(relativeTo, path);

			// Attempt to find and invoke the appropriate parser based on the file extension
			string ext = Path.GetExtension(path).Substring(1).ToLower();
			if (ModelParsers.ContainsKey(ext))
				ModelParsers[ext].Parse(path, Models, this);
			else if (MaterialParsers.ContainsKey(ext))
				MaterialParsers[ext].Parse(path, Materials, this);
			else if (TextureParsers.ContainsKey(ext))
				TextureParsers[ext].Parse(path, Textures, this);
			else
				throw new Exception("Unsupported File Type: " + path);
		}

		public void Dispose()
		{
			LoadedAssets.Clear();
			Models.Dispose();
			Materials.Dispose();
			Textures.Dispose();
		}
	}
}
