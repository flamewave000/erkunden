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
			List<Vector2> texcs = new List<Vector2>();
			List<Vector3> normsTemp = new List<Vector3>();
			List<Vector2> texcsTemp = new List<Vector2>();

			// Model Face Data
			List<Model.Part> parts = new List<Model.Part>();
			Model.Part? currPart = null;
			string[] indexA;
			string[] indexB;
			string[] indexC;
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
				norms.Resize(verts.Count);
				texcs.Resize(verts.Count);
			};
			Action<string?> pushModel = name =>
			{
				pushPart(null);
				if (model != null)
				{
					Log.WriteLine("@green;Loaded Model:@magenta;    " + model.Name, indent: true);
					model.Vertices.Data = verts.ToArray();
					model.Normals.Data = norms.ToArray();
					model.TexCoords.Data = texcs.ToArray();
					model.Parts = parts.ToArray();
					store.Register(model.Name, model);
				}
				verts.Clear();
				norms.Clear();
				texcs.Clear();
				normsTemp.Clear();
				texcsTemp.Clear();
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
							provider.LoadAsset(FileUtil.PlatformPath(tokens[1]), FileUtil.GetParentDirectory(path));
							continue;
						// Declare current face group will use this material by name
						case "usemtl":
							if (currPart == null)
								pushPart("");
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
							texcsTemp.Add(ModelParser.ParseVector2(tokens.AsSpan(1)));
							continue;
						// Normals
						case "vn":
							normsTemp.Add(ModelParser.ParseVector3(tokens.AsSpan(1)));
							continue;
						case "s":
							if (model == null) pushModel(tokens[1]);
							currPart!.Smooth = tokens[1].ToLower().Trim() != "off";
							continue;
						// Polygon Indicies
						case "f":
							// f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3
							// Split the indexes
							indexA = tokens[1].Split('/');
							indexB = tokens[2].Split('/');
							indexC = tokens[3].Split('/');
							// Generate the Vertex Indices
							var index = new Vector3i(AsIndex(indexA[0]), AsIndex(indexB[0]), AsIndex(indexC[0]));
							indexes.Add((uint)index.X);
							indexes.Add((uint)index.Y);
							indexes.Add((uint)index.Z);
							// Generate the TexCoords
							texcs[index.X] = texcsTemp[AsIndex(indexA[1])];
							texcs[index.Y] = texcsTemp[AsIndex(indexB[1])];
							texcs[index.Z] = texcsTemp[AsIndex(indexC[1])];
							// Generate the normals
							norms[index.X] = normsTemp[AsIndex(indexA[2])];
							norms[index.Y] = normsTemp[AsIndex(indexB[2])];
							norms[index.Z] = normsTemp[AsIndex(indexC[2])];
							continue;
						default: continue;
					}
				}
				pushModel(null);
			}
		}
		private static int AsIndex(string value) => int.Parse(value) - 1;
	}
}
