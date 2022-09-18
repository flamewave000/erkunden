using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Erkunden.Client.AssetManagement.Materials;
using Erkunden.Client.Graphics.Data;
using Erkunden.Core.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Erkunden.Client.AssetManagement.Models
{
	public class InvalidPrimitiveTypeException : Exception { }
	/**
	 * <summary>Parses the Wavefront Model file format <b>.OBJ</b></summary>
	 * <see cref="https://en.wikipedia.org/wiki/Wavefront_.obj_file"/>
	 */
	public class ObjParser : AssetParser
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Register() => AssetProvider.RegisterParser(new ObjParser());

		public string[] Extensions { get; } = new string[] { ".obj" };
		public void Parse(FileInfo file, AssetStore store)
		{
			string[] tokens;

			string? modelName = null;
			List<Model.Part> parts = new List<Model.Part>();
			List<VertexTextureNormal> vertexTextureNormals = new List<VertexTextureNormal>();
			List<Vector3> verts = new List<Vector3>();
			List<Vector2> texcs = new List<Vector2>();
			List<Vector3> norms = new List<Vector3>();

			// Model Face Data
			string? partName = null;
			Material material = Material.BasicMaterial.Value;
			int indexOffset = 0;
			int count = 0;
			PrimitiveType primitiveType = PrimitiveType.Triangles;

			Action<string?> pushPart = name =>
			{
				if (partName != null)
				{
					parts.Add(Model.Part.CreateWithOffset(partName, count, indexOffset, material, primitiveType));
					indexOffset += count;
					partName = null;
					material = Material.BasicMaterial.Value;
					count = 0;
				}
				if (name != null)
				{
					partName = name;
					AssetProvider.TryGet(name, ref material);
				}
			};
			Action<string?> pushModel = name =>
			{
				pushPart(null);
				if (modelName != null)
				{
					Log.WriteLine($"@green;Loaded Model:@magenta;    {modelName}@clr - {parts.Count} Parts" + modelName, indent: true);
					store.Add(AssetParser.ConformName<Model>(modelName),
						new Model(modelName, vertexTextureNormals.ToArray(), parts.ToArray()));
					vertexTextureNormals.Clear();
					parts.Clear();
					verts.Clear();
					texcs.Clear();
					norms.Clear();
				}
				modelName = name;
			};

			using (StreamReader reader = new StreamReader(file.OpenRead()))
			{
				while (!reader.EndOfStream)
				{
					tokens = (reader.ReadLine() ?? "").Trim().ReduceSpaces().Split(' ');
					if (tokens.Length == 0) continue;

					switch (tokens[0])
					{
						// Declare material dependency
						case "mtllib":
							// ! Materials should automatically be detected !
							continue;
						// Declare current face group will use this material by name
						case "usemtl":
							pushPart(tokens[1]);
							partName = tokens[1];
							material = AssetProvider.Get<Material>(tokens[1]);
							continue;
						// Declare new Object or Group
						case "g":
						case "o":
							pushModel(string.Join(' ', tokens, 1, tokens.Length - 1));
							continue;
						// Verticies
						case "v":
							verts.Add(AssetParser.ParseVector3(tokens.AsSpan(1)));
							continue;
						// Texture Coordinates
						case "vt":
							texcs.Add(AssetParser.ParseVector2(tokens.AsSpan(1)));
							continue;
						// Normals
						case "vn":
							norms.Add(AssetParser.ParseVector3(tokens.AsSpan(1)));
							continue;
						case "s":
							//if (model == null) pushModel(tokens[1]);
							//currPart!.Smooth = tokens[1].ToLower().Trim() != "off";
							continue;
						// Polygon Indicies
						case "f":
							if (partName == null)
								pushPart("part" + parts.Count);
							// f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3 v#/vt#/vn#
							// Split the indexes
							VertexTextureNormal[] vertexes = new VertexTextureNormal[tokens.Length - 1];
							int[] vertex;
							count++;
							for (int c = 0; c < vertexes.Length; c++)
							{
								vertex = tokens[c + 1]
									.Split('/')
									.Select(AsIndex)
									.ToArray();
								vertexes[c] = new VertexTextureNormal(
									verts[vertex[0]],
									texcs[vertex[1]],
									norms[vertex[2]]
								);
							};
							// Determine what kind of primitive is being used
							switch (tokens.Length - 1)
							{
								case 3:
									primitiveType = PrimitiveType.Triangles;
									vertexTextureNormals.AddRange(vertexes);
									count += 6;
									break;
								case 4:
									primitiveType = PrimitiveType.Triangles;
									vertexTextureNormals.AddRange(new VertexTextureNormal[]
									{
										vertexes[0], vertexes[1], vertexes[2],
										vertexes[2], vertexes[3], vertexes[0]
									});
									count += 6;
									break;
								default:
									primitiveType = PrimitiveType.TriangleFan;
									vertexTextureNormals.AddRange(vertexes);
									count += vertexes.Length;
									break;
							}
							continue;
						default: continue;
					}
				}
				pushModel(null);
			}
		}
		private static int AsIndex(string value) => int.Parse(value) - 1;

		public IEnumerable<string> GetNames(FileInfo file)
		{
			string? line;
			using (StreamReader stream = new StreamReader(file.OpenRead()))
			{
				while (!stream.EndOfStream)
				{
					line = stream.ReadLine()?.Trim();
					if (line == null) continue;
					if (!line.StartsWith('o')) continue;
					yield return AssetParser.ConformName<Model>(line.ReduceSpaces().Substring(line.IndexOf(' ') + 1));
				}
			}
		}
	}
}
