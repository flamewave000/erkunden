using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Erkunden.Core.Util;

namespace Erkunden.Client.AssetManagement
{
	#region Custom Exceptions
	public class AssetNotFoundException : Exception
	{
		public AssetNotFoundException(string message) : base(message) { }
	}
	public class DuplicateAssetException : Exception
	{
		public DuplicateAssetException(string assetName, string original, string duplicate)
			: base($"Duplicate: {assetName}\n\t Original: {original}\n\tDuplicate: {duplicate}") { }
	}
	public class DuplicateParserException : Exception
	{
		public DuplicateParserException(string extension, AssetParser original, AssetParser duplicate)
			: base($"Duplicate: {extension}\n\t Original: {original.GetType().FullName}\n\tDuplicate: {duplicate.GetType().FullName}") { }
	}
	#endregion

	public static class AssetProvider
	{
		#region Fields
		private static Dictionary<string, string> assetFiles = new Dictionary<string, string>();
		private static Dictionary<string, AssetParser> assetParsers = new Dictionary<string, AssetParser>();
		private static AssetStore assets = new AssetStore();
		#endregion

		#region Asset Accessors
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(string name) where T : Asset => assetFiles.ContainsKey(name);
		private static Asset Get(Type type, string name)
		{
			string conformedName = AssetParser.ConformName(type, name);
			if (!assetFiles.ContainsKey(conformedName))
				throw new AssetNotFoundException($"{name}: Asset File Not Found");

			if (!assets.TryGet(conformedName, out var assetRef) || assetRef == null)
				ParseAssetFile(conformedName);
			var asset = assets.Get(conformedName);
			if (asset == null)
				throw new AssetNotFoundException($"{name}: Asset Not Found in File\n\tFile: {assetFiles[conformedName]}");
			return asset;
		}
		public static T Get<T>(string name) where T : Asset
		{
			var asset = Get(typeof(T), name);
			if (asset is T) return (T)asset;
			throw new AssetNotFoundException($"{name}: Asset Wrong Type\n\tExpected: {typeof(T).Name}\n\tFound: {asset.GetType().Name}");
		}
		public static bool TryGet<T>(string name, ref T outValue) where T : Asset
		{
			try { outValue = Get<T>(name); return true; }
			catch { return false; }
		}

		public static void LoadAssets(string assetDirectory)
		{
			Queue<DirectoryInfo> dirs = new Queue<DirectoryInfo>();
			dirs.Enqueue(new DirectoryInfo(assetDirectory));

			DirectoryInfo dir;
			while (dirs.Count > 0)
			{
				dir = dirs.Dequeue();
				dir.EnumerateDirectories().ForEach(dirs.Enqueue);
				foreach (FileInfo file in dir.EnumerateFiles())
				{
					LoadAssetFile(file);
				}
			}
		}
		public static IEnumerable<string> LoadAssetFile(string fileName) => LoadAssetFile(new FileInfo(fileName));
		public static IEnumerable<string> LoadAssetFile(FileInfo file)
		{
			string ext = file.Extension;
			if (!assetParsers.TryGetValue(ext, out var parser))
				return new string[0];
			var names = parser.GetNames(file);
			foreach (var name in names)
			{
				// Try to add, if there is a duplicate asset name, we need to throw an exception
				if (assetFiles.TryAdd(name, file.FullName)) continue;
				throw new DuplicateAssetException(name, assetFiles[name], file.FullName);
			}
			return names;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ParseAssetFile(string assetName) => ParseAssetFile(new FileInfo(assetFiles[assetName]));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ParseAssetFile(FileInfo file) => assetParsers[file.Extension].Parse(file, assets);
		#endregion

		public static void RegisterParser(AssetParser parser, HashSet<string> ignoreExtensions = null!)
		{
			ignoreExtensions = ignoreExtensions ?? new HashSet<string>();
			foreach (var ext in parser.Extensions)
			{
				if (ignoreExtensions.Contains(ext)) continue;
				// Try to add, if there is already a parser for the given type we need to throw an exception
				if (assetParsers.TryAdd(ext, parser)) continue;
				throw new DuplicateParserException(ext, assetParsers[ext], parser);
			}
		}

		public static void Dispose()
		{
			assetFiles.Clear();
			assets.Dispose();
		}
	}
}
