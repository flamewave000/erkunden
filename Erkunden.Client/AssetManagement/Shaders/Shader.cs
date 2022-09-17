using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Erkunden.Client.AssetManagement.Textures;
using Erkunden.Client.Graphics.Data;
using Erkunden.Client.Graphics.Objects;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Erkunden.Client.AssetManagement.Shaders
{
	public class Shader : BaseAsset
	{
		private Dictionary<string, int> locations = new Dictionary<string, int>();

		public readonly ShaderProgram Program;
		public ShaderConfig Config;
		public ShaderScript[] Scripts;

		public override bool IsDisposed => Program.IsDisposed;

		public int AttributeVertex { get; private set; } = -1;
		public int AttributeTexCoord { get; private set; } = -1;
		public int AttributeNormal { get; private set; } = -1;

		public int MatrixModel { get; private set; } = -1;
		public int MatrixView { get; private set; } = -1;
		public int MatrixProjection { get; private set; } = -1;

		public int TextureAmbient { get; private set; } = -1;
		public int TextureDiffuse { get; private set; } = -1;
		public int TextureSpecular { get; private set; } = -1;
		public int TextureNormal { get; private set; } = -1;

		public int ColorAmbient { get; private set; } = -1;
		public int ColorDiffuse { get; private set; } = -1;
		public int ColorSpecular { get; private set; } = -1;

		public Shader(string name, ShaderScript[] scripts, ShaderConfig config) : base(name)
		{
			Program = new ShaderProgram(scripts);
			Scripts = scripts;
			Config = config;
		}

		public override void Dispose() => Program.Dispose();

		public void Use()
		{
			if (IsDisposed)
				Compile();

			Program.Bind();

			GL.PolygonMode(MaterialFace.FrontAndBack, Config.PolygonMode);

			GL.CullFace(Config.CullMode);
			GL.FrontFace(Config.FaceDirection);

			if (Config.DepthTest) GL.Enable(EnableCap.DepthTest); else GL.Disable(EnableCap.DepthTest);
			if (Config.Blend) GL.Enable(EnableCap.Blend); else GL.Disable(EnableCap.Blend);

			GL.BlendFuncSeparate(
				Config.SrcColorBlend,
				Config.DstColorBlend,
				Config.SrcAlphaBlend,
				Config.DstAlphaBlend
			);
		}

		public void Compile()
		{
			Program.Build();
			Program.Bind();

			AttributeVertex = GetAttribLocation(Config.AttributeVertex);
			AttributeTexCoord = GetAttribLocation(Config.AttributeTexCoord);
			AttributeNormal = GetAttribLocation(Config.AttributeNormal);

			MatrixModel = GetUniformLocation(Config.MatrixModel);
			MatrixView = GetUniformLocation(Config.MatrixView);
			MatrixProjection = GetUniformLocation(Config.MatrixProjection);

			TextureAmbient = GetUniformLocation(Config.TextureAmbient);
			TextureDiffuse = GetUniformLocation(Config.TextureDiffuse);
			TextureSpecular = GetUniformLocation(Config.TextureSpecular);
			TextureNormal = GetUniformLocation(Config.TextureNormal);

			ColorAmbient = GetUniformLocation(Config.ColorAmbient);
			ColorDiffuse = GetUniformLocation(Config.ColorDiffuse);
			ColorSpecular = GetUniformLocation(Config.ColorSpecular);
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Toggle(EnableCap cap, bool value) { if (value) GL.Enable(cap); else GL.Disable(cap); }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetColor4(string name, Color4 value) => GL.Uniform4(GetUniformLocation(name), value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetInt(string name, int value) => GL.Uniform1(GetUniformLocation(name), value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetFloat(string name, float value) => GL.Uniform1(GetUniformLocation(name), value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetBool(string name, bool value) => GL.Uniform1(GetUniformLocation(name), value ? 1 : 0);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMat4(string name, ref Matrix4 value) => GL.UniformMatrix4(GetUniformLocation(name), true, ref value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetAttribLocation(string attribName) => GL.GetAttribLocation(Program.Handle, attribName);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetUniformLocation(string uniformName)
		{
			if (!locations.TryGetValue(uniformName, out var location))
				location = locations[uniformName] = GL.GetUniformLocation(Program.Handle, uniformName);
			return location;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetModel(ref Matrix4 value) => GL.UniformMatrix4(MatrixModel, true, ref value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetView(ref Matrix4 value) => GL.UniformMatrix4(MatrixView, true, ref value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetProjection(ref Matrix4 value) => GL.UniformMatrix4(MatrixProjection, true, ref value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetTexture(string name, TextureUnit unit, ref Texture texture) => SetTexture(GetUniformLocation(name), unit, ref texture);
		public void SetTexture(int location, TextureUnit unit, ref Texture texture)
		{
			texture.Bind(unit);
			GL.Uniform1(location, (int)unit - (int)TextureUnit.Texture0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetAmbientTexture(TextureUnit unit, ref Texture texture) => SetTexture(TextureAmbient, unit, ref texture);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetDiffuseTexture(TextureUnit unit, ref Texture texture) => SetTexture(TextureDiffuse, unit, ref texture);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetSpecularTexture(TextureUnit unit, ref Texture texture) => SetTexture(TextureSpecular, unit, ref texture);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetNormalTexture(TextureUnit unit, ref Texture texture) => SetTexture(TextureNormal, unit, ref texture);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetAmbientColor(Color4 value) => GL.Uniform4(ColorAmbient, value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetDiffuseColor(Color4 value) => GL.Uniform4(ColorDiffuse, value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetSpecularColor(Color4 value) => GL.Uniform4(ColorSpecular, value);
	}
}
