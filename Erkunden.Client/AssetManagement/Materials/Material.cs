using System.Runtime.CompilerServices;
using Erkunden.Client.AssetManagement.Textures;
using Erkunden.Client.Graphics.Objects;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Erkunden.Client.AssetManagement.Materials
{
	public class Material : Asset
	{
		public float Shininess;

		public Color4? AmbientColor;
		public Color4? DiffuseColor;
		public Color4? SpecularColor;

		/** <summary>Ambient Map</summary> */
		public Texture? AmbientMap;
		/** <summary>Diffuse Map</summary> */
		public Texture? DiffuseMap;
		/** <summary>Specular Map</summary> */
		public Texture? SpecularMap;
		/** <summary>Normal Map</summary> */
		public Texture? NormalMap;

		public Material(string name) : base(name) { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void BindColour(Shader shader, string name, Color4? colour)
		{
			if (!colour.HasValue) return;
			shader.SetColor4(name, colour.Value);
		}

		public void Bind(Shader shader)
		{
			// Set the colors
			BindColour(shader, "u_AmbientColor", AmbientColor);
			BindColour(shader, "u_DiffuseColor", DiffuseColor);
			BindColour(shader, "u_SpecularColor", SpecularColor);
			// Bind the textures
			AmbientMap?.Bind(TextureUnit.Texture0);
			DiffuseMap?.Bind(TextureUnit.Texture1);
			SpecularMap?.Bind(TextureUnit.Texture2);
			NormalMap?.Bind(TextureUnit.Texture3);
			// Set the sampler2D indices
			shader.SetInt("u_AmbientTexture", 0);
			shader.SetInt("u_DiffuseTexture", 1);
			shader.SetInt("u_SpecularTexture", 2);
			shader.SetInt("u_NormalTexture", 3);
			// Set additional fields
			shader.SetFloat("u_Shininess", Shininess);
		}

		public override void Dispose() { }
		public override bool IsDisposed => false;
	}
}
