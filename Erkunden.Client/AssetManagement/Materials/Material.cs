using Erkunden.Client.AssetManagement.Textures;
using OpenTK.Mathematics;

namespace Erkunden.Client.AssetManagement.Materials
{
	public class Material : Asset
	{
		public float Shininess;
		public float Transparency;

		public Vector3? AmbientColor;
		public Vector3? DiffuseColor;
		public Vector3? SpecularColor;

		/** <summary>Diffuse Map</summary> */
		public Texture? DiffuseMap;
		/** <summary>Ambient Map</summary> */
		public Texture? AmbientMap;
		/** <summary>Normal Map</summary> */
		public Texture? NormalMap;
		/** <summary>Specular Map</summary> */
		public Texture? SpecularMap;

		public Material(string name) : base(name) { }

		public override void Dispose() { }
		public override bool IsDisposed => false;
	}
}
