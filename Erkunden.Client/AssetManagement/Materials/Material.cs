using System;
using System.Runtime.CompilerServices;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.AssetManagement.Textures;
using Erkunden.Client.Graphics.Objects;
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
			if (AmbientMap != null) shader.SetAmbientTexture(TextureUnit.Texture0, ref AmbientMap);
			if (DiffuseMap != null) shader.SetDiffuseTexture(TextureUnit.Texture1, ref DiffuseMap);
			if (SpecularMap != null) shader.SetSpecularTexture(TextureUnit.Texture2, ref SpecularMap);
			if (NormalMap != null) shader.SetNormalTexture(TextureUnit.Texture3, ref NormalMap);
			// Set additional fields
			if (ShininessLocation >= 0)
				shader.SetFloat("u_Shininess", Shininess);
		}

		public void Dispose() { }
		public bool IsDisposed => false;

		public static Lazy<Material> BasicMaterial = new Lazy<Material>(() => AssetProvider.Get<Material>("Basic"));
	}
}
