using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Erkunden.Client.AssetManagement.Textures;
using Erkunden.Core.Utils;

namespace Erkunden.Client.AssetManagement.Materials
{
	/**
	 * <summary>Parses the Wavefront Material file format <b>.MTL</b></summary>
	 * <see cref="https://en.wikipedia.org/wiki/Wavefront_.obj_file"/>
	 */
	public class MtlParser : AssetParser
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Register() => AssetProvider.RegisterParser(new MtlParser());

		public string[] Extensions { get; } = new string[] { ".mtl" };
		public void Parse(FileInfo file, AssetStore store)
		{
			Material material = new Material();
			string texPath;

			Action<string?> pushMaterial = name =>
			{
				if (material.Name != null)
				{
					Log.WriteLine("@green;Loaded Material:@magenta; " + material.Name, indent: true);
					store.Add(AssetParser.ConformName<Material>(material.Name), material);
				}
				material = new Material();
				material.Name = name!;
			};

			using (StreamReader reader = new StreamReader(file.OpenRead()))
			{
				while (!reader.EndOfStream)
				{
					string[] tokens = (reader.ReadLine() ?? "").Trim().ReduceSpaces().Split(' ');
					switch (tokens[0])
					{
						// New Material
						case "newmtl":
							pushMaterial(string.Join(' ', tokens, 1, tokens.Length - 1));
							continue;
						// Ambient Colour
						case "Ka":
							material.AmbientColor = AssetParser.ParseColor4(tokens.AsSpan(1));
							continue;
						// Diffuse Colour
						case "Kd":
							material.DiffuseColor = AssetParser.ParseColor4(tokens.AsSpan(1));
							continue;
						// Specular Colour
						case "Ks":
							material.SpecularColor = AssetParser.ParseColor4(tokens.AsSpan(1));
							continue;
						// Shininess Factor
						case "Ns":
							material.Shininess = float.Parse(tokens[1]);
							continue;
						// Ambient Occlusion Map
						case "map_Ka":
							texPath = FileUtil.PlatformPath(tokens[1]);
							material!.AmbientMap = AssetProvider.Get<Texture>(Path.GetFileNameWithoutExtension(texPath));
							continue;
						// Diffuse Map
						case "map_Kd":
							texPath = FileUtil.PlatformPath(tokens[1]);
							material!.DiffuseMap = AssetProvider.Get<Texture>(Path.GetFileNameWithoutExtension(texPath));
							continue;
						// Specular Map
						case "map_Ks":
							texPath = FileUtil.PlatformPath(tokens[1]);
							material!.SpecularMap = AssetProvider.Get<Texture>(Path.GetFileNameWithoutExtension(texPath));
							continue;
						// Normal Map
						case "bump":
							texPath = FileUtil.PlatformPath(tokens[1]);
							material!.NormalMap = AssetProvider.Get<Texture>(Path.GetFileNameWithoutExtension(texPath));
							continue;
					}
				}
				pushMaterial(null);
			}
		}

		public IEnumerable<string> GetNames(FileInfo file)
		{
			using (StreamReader stream = new StreamReader(file.OpenRead()))
			{
				string? line;
				while (!stream.EndOfStream)
				{
					line = stream.ReadLine()?.Trim();
					if (line == null || !line.StartsWith("newmtl")) continue;
					yield return AssetParser.ConformName<Material>(line.ReduceSpaces().Substring(line.IndexOf(' ') + 1));
				}
			}
		}
	}
}
