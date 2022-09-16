using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Mathematics;

namespace Erkunden.Client.AssetManagement
{
	public interface AssetParser
	{
		public string[] Extensions { get; }
		public void Parse(FileInfo file, AssetStore store);
		public IEnumerable<string> GetNames(FileInfo file);

		public static string ConformName<T>(string assetName) where T : Asset => typeof(T).Name + "_" + assetName;
		public static string ConformName(Type type, string assetName) => type.Name + "_" + assetName;

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
		protected static Color4 ParseColor4(Span<string> tokens)
		{
			var col = new Color4();
			col.R = tokens.Length > 0 ? float.Parse(tokens[0]) : 0;
			col.G = tokens.Length > 1 ? float.Parse(tokens[1]) : 0;
			col.B = tokens.Length > 2 ? float.Parse(tokens[2]) : 0;
			col.A = tokens.Length > 3 ? float.Parse(tokens[3]) : 1;
			return col;
		}
	}
}
