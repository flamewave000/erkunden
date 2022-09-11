using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Erkunden.Client.AssetManagement.Textures;
using Erkunden.Core.Util;

namespace Erkunden.Client.AssetManagement.Materials
{
	/**
	 * <summary>Parses the Wavefront Material file format <b>.MTL</b></summary>
	 * <see cref="https://en.wikipedia.org/wiki/Wavefront_.obj_file"/>
	 */
	public class MtlParser : MaterialParser
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Register(AssetProvider provider) => provider.RegisterParser<MaterialParser, Material>(new MtlParser());

		public string[] Extensions => new string[] { "mtl", "mtls", "mat", "mats" };
		public void Parse(string path, AssetStore<Material> assets, AssetProvider provider)
		{
			Material? material = null;
			string texPath;

			Action<string?> pushMaterial = name =>
			{
				if (material != null)
				{
					Log.WriteLine("@green;Loaded Material:@magenta; " + material.Name, indent: true);
					assets.Register(material.Name, material);
				}
				material = name != null ? new Material(name) : null;
			};

			using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
			{
				while (!reader.EndOfStream)
				{
					string[] tokens = (reader.ReadLine() ?? "").Trim().ReduceSpaces().Split(' ');
					switch (tokens[0])
					{
						// New Material
						case "newmtl":
							pushMaterial(tokens[1]);
							continue;
						// Ambient Colour
						case "Ka":
							material!.AmbientColor = MaterialParser.ParseColor4(tokens.AsSpan(1));
							continue;
						// Diffuse Colour
						case "Kd":
							material!.DiffuseColor = MaterialParser.ParseColor4(tokens.AsSpan(1));
							continue;
						// Specular Colour
						case "Ks":
							material!.SpecularColor = MaterialParser.ParseColor4(tokens.AsSpan(1));
							continue;
						// Shininess Factor
						case "Ns":
							material!.Shininess = float.Parse(tokens[1]);
							continue;
						// Ambient Occlusion Map
						case "map_Ka":
							texPath = FileUtil.PlatformPath(tokens[1]);
							provider.LoadAsset(texPath, FileUtil.GetParentDirectory(path));
							material!.AmbientMap = provider.Get<Texture>(Path.GetFileNameWithoutExtension(texPath));
							continue;
						// Diffuse Map
						case "map_Kd":
							texPath = FileUtil.PlatformPath(tokens[1]);
							provider.LoadAsset(texPath, FileUtil.GetParentDirectory(path));
							material!.DiffuseMap = provider.Get<Texture>(Path.GetFileNameWithoutExtension(texPath));
							continue;
						// Specular Map
						case "map_Ks":
							texPath = FileUtil.PlatformPath(tokens[1]);
							provider.LoadAsset(texPath, FileUtil.GetParentDirectory(path));
							material!.SpecularMap = provider.Get<Texture>(Path.GetFileNameWithoutExtension(texPath));
							continue;
						// Normal Map
						case "bump":
							texPath = FileUtil.PlatformPath(tokens[1]);
							provider.LoadAsset(texPath, FileUtil.GetParentDirectory(path));
							material!.NormalMap = provider.Get<Texture>(Path.GetFileNameWithoutExtension(texPath));
							continue;
					}
				}
				pushMaterial(null);
			}
		}
	}
}
