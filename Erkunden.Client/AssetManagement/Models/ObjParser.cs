using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Erkunden.Client.AssetManagement.Materials;
using Erkunden.Core.Util;
using OpenTK.Mathematics;

namespace Erkunden.Client.AssetManagement.Models
{
	/**
	 * <summary>Parses the Wavefront Model file format <b>.OBJ</b></summary>
	 * <see cref="https://en.wikipedia.org/wiki/Wavefront_.obj_file"/>
	 */
	public class ObjParser : ModelParser
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Register(AssetProvider provider) => provider.RegisterParser<ModelParser, Model>(new ObjParser());

		public string[] Extensions => new string[] { "obj", "object" };
		public void Parse(string path, AssetStore<Model> store, AssetProvider provider)
		{
			string[] tokens;

			// Model Data
			Model? model = null;
			List<Vector3> verts = new List<Vector3>();
			List<Vector3> norms = new List<Vector3>();
			List<Vector3> texcs = new List<Vector3>();

			// Model Face Data
			List<Model.Part> parts = new List<Model.Part>();
			Model.Part? currPart = null;
			bool? hasSlash = null;
			List<uint> indexes = new List<uint>();

			Action<string?> pushPart = name =>
			{
				if (currPart != null)
				{
					currPart.Indexes.Data = indexes.ToArray();
					parts.Add(currPart);
				}
				currPart = name != null ? new Model.Part(name) : null;
				indexes.Clear();
				hasSlash = null;
			};
			Action<string?> pushModel = name =>
			{
				pushPart(null);
				if (model != null)
				{
					Log.WriteLine("@green;Loaded Model:@magenta; " + model.Name, indent: true);
					model.Vertices.Data = verts.ToArray();
					model.Normals.Data = norms.ToArray();
					model.TexCoords.Data = texcs.ToArray();
					model.Parts = parts.ToArray();
					store.Register(model.Name, model);
				}
				verts.Clear();
				norms.Clear();
				texcs.Clear();
				model = name != null ? new Model(name) : null;
			};

			using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
			{
				while (!reader.EndOfStream)
				{
					tokens = (reader.ReadLine() ?? "").Trim().ReduceSpaces().Split(' ');
					if (tokens.Length == 0) continue;

					switch (tokens[0])
					{
						// Declare material dependency
						case "mtllib":
							provider.LoadAsset(tokens[1], ModelParser.GetParentDirectory(path));
							continue;
						// Declare current face group will use this material by name
						case "usemtl":
							currPart!.Material = provider.Get<Material>(tokens[1]);
							continue;
						// Declare new Object
						case "o":
							pushModel(tokens[1]);
							continue;
						// Declare new Face Group
						case "g":
							if (model == null) pushModel(tokens[1]);
							pushPart(tokens[1]);
							continue;

						// Verticies
						case "v":
							verts.Add(ModelParser.ParseVector3(tokens.AsSpan(1)));
							continue;
						// Texture Coordinates
						case "vt":
							texcs.Add(ModelParser.ParseVector3(tokens.AsSpan(1)));
							continue;
						// Normals
						case "vn":
							norms.Add(ModelParser.ParseVector3(tokens.AsSpan(1)));
							continue;
						// Polygon Indicies
						case "f":
							if (hasSlash == null)
								hasSlash = tokens[1].Contains('/');
							if (hasSlash == true)
							{
								indexes.Add(uint.Parse(tokens[1].Substring(0, tokens[1].IndexOf('/'))));
								indexes.Add(uint.Parse(tokens[2].Substring(0, tokens[2].IndexOf('/'))));
								indexes.Add(uint.Parse(tokens[3].Substring(0, tokens[3].IndexOf('/'))));
							}
							else
							{
								indexes.Add(uint.Parse(tokens[1]));
								indexes.Add(uint.Parse(tokens[2]));
								indexes.Add(uint.Parse(tokens[3]));
							}
							continue;
						default: continue;
					}
				}
				pushModel(null);
			}
		}
	}
}
