using System;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.AssetManagement.Textures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Erkunden.Client.AssetManagement.Materials
{
	public struct Material : Asset
	{
		public string Name { get; set; }
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

		private int? ShininessLocation;

		public void Bind(Shader shader)
		{
			if (ShininessLocation == null)
				ShininessLocation = shader.GetUniformLocation("u_Shininess");
			// Set the colors
			if (AmbientColor.HasValue) shader.SetAmbientColor(AmbientColor.Value);
			if (DiffuseColor.HasValue) shader.SetDiffuseColor(DiffuseColor.Value);
			if (SpecularColor.HasValue) shader.SetSpecularColor(SpecularColor.Value);
			// Bind the textures
			shader.SetAmbientTexture(TextureUnit.Texture0, AmbientMap);
			shader.SetDiffuseTexture(TextureUnit.Texture1, DiffuseMap);
			shader.SetSpecularTexture(TextureUnit.Texture2, SpecularMap);
			shader.SetNormalTexture(TextureUnit.Texture3, NormalMap);
			// Set additional fields
			if (ShininessLocation >= 0)
				shader.SetFloat("u_Shininess", Shininess);
		}

		public void Dispose() { }
		public bool IsDisposed => false;

		public static Lazy<Material> BasicMaterial = new Lazy<Material>(() => AssetProvider.Get<Material>("Basic"));
	}
}
