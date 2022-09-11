using System;
using System.Collections.Generic;
using System.IO;
using Erkunden.Client.AssetManagement.Materials;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.AssetManagement.Textures;
using OpenTK.Mathematics;

namespace Erkunden.Client.AssetManagement
{
	public interface AssetParser<T> where T : Asset
	{
		public string[] Extensions { get; }
		public void Parse(string assetPath, AssetStore<T> store, AssetProvider provider);

		protected static Vector2 ParseVector2(Span<string> tokens)
		{
			var vec = new Vector2();
			vec.X = tokens.Length > 0 ? float.Parse(tokens[0]) : 0;
			vec.Y = tokens.Length > 1 ? float.Parse(tokens[1]) : 0;
			return vec;
		}
		protected static Vector3 ParseVector3(Span<string> tokens)
		{
			var vec = new Vector3();
			vec.X = tokens.Length > 0 ? float.Parse(tokens[0]) : 0;
			vec.Y = tokens.Length > 1 ? float.Parse(tokens[1]) : 0;
			vec.Z = tokens.Length > 2 ? float.Parse(tokens[2]) : 0;
			return vec;
		}
		protected static Vector3i ParseVector3i(Span<string> tokens)
		{
			var vec = new Vector3i();
			vec.X = tokens.Length > 0 ? int.Parse(tokens[0]) : 0;
			vec.Y = tokens.Length > 1 ? int.Parse(tokens[1]) : 0;
			vec.Z = tokens.Length > 2 ? int.Parse(tokens[2]) : 0;
			return vec;
		}
		protected static string GetParentDirectory(string path) => new FileInfo(path).Directory.FullName;
	}

	public interface ModelParser : AssetParser<Model> { }
	public interface MaterialParser : AssetParser<Material> { }
	public interface TextureParser : AssetParser<Texture> { }
}
