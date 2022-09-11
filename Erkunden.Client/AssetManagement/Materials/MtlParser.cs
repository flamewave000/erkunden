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
							material!.AmbientColor = MaterialParser.ParseVector3(tokens.AsSpan(1));
							continue;
						// Diffuse Colour
						case "Kd":
							material!.DiffuseColor = MaterialParser.ParseVector3(tokens.AsSpan(1));
							continue;
						// Specular Colour
						case "Ks":
							material!.SpecularColor = MaterialParser.ParseVector3(tokens.AsSpan(1));
							continue;
						// Shininess Factor
						case "Ns":
							material!.Shininess = float.Parse(tokens[1]);
							continue;
						// Ambient Occlusion Map
						case "map_Ka":
							provider.LoadAsset(tokens[1], MaterialParser.GetParentDirectory(path));
							material!.AmbientMap = provider.Get<Texture>(tokens[1]);
							continue;
						// Diffuse Map
						case "map_Kd":
							provider.LoadAsset(tokens[1], MaterialParser.GetParentDirectory(path));
							material!.DiffuseMap = provider.Get<Texture>(tokens[1]);
							continue;
						// Specular Map
						case "map_Ks":
							provider.LoadAsset(tokens[1], MaterialParser.GetParentDirectory(path));
							material!.SpecularMap = provider.Get<Texture>(tokens[1]);
							continue;
						// Normal Map
						case "bump":
							provider.LoadAsset(tokens[1], MaterialParser.GetParentDirectory(path));
							material!.NormalMap = provider.Get<Texture>(tokens[1]);
							continue;
					}
				}
				pushMaterial(null);
			}
		}
	}
}
