using System;
using System.Runtime.CompilerServices;
using Erkunden.Core.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Erkunden.Client.Graphics.Objects
{
	public class Shader : GraphicsObject, IDisposable
	{
		public ShaderScript[] Shaders { get; private set; }
		public string? LastError { get; private set; } = null;

		public Shader(ShaderScript[] shaders) { Shaders = shaders; }

		public void Build()
		{
			if (!IsDisposed) return;
			Handle = GL.CreateProgram();
			Log.WriteLine($"@blue;Created Shader Program: @Magenta;{Handle}");
			Log.WriteLine($"@yellow;Compiling Shader Program @Magenta;{Handle}");
			Log.Indent();
			foreach (var shader in Shaders)
			{
				shader.Build();
				GL.AttachShader(Handle, shader.Handle);
			}

			GL.LinkProgram(Handle);
			GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
			if (success == 0)
			{
				LastError = GL.GetProgramInfoLog(Handle);
				Log.WriteLine("@red;Failed!", false);
				Log.WriteLine(LastError, true);
			}
			else
				Log.WriteLine($"@yellow;Linked Program @Magenta;{Handle}");

			foreach (var shader in Shaders)
			{
				GL.DetachShader(Handle, shader.Handle);
				shader.Dispose();
			}
			Log.Dedent();
		}

		public override void Bind()
		{
			GL.UseProgram(Handle);
		}
		public override void Dispose()
		{
			if (IsDisposed) return;
			GL.DeleteProgram(Handle);
			Handle = 0;
			foreach (var shader in Shaders)
			{
				shader.Dispose();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetColor4(string name, Color4 value) => GL.Uniform4(GetUniformLocation(name), value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetInt(string name, int value) => GL.Uniform1(GetUniformLocation(name), value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetFloat(string name, float value) => GL.Uniform1(GetUniformLocation(name), value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetBool(string name, bool value) => GL.Uniform1(GetUniformLocation(name), value ? 1 : 0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetAttribLocation(string attribName) => GL.GetAttribLocation(Handle, attribName);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetUniformLocation(string uniformName) => GL.GetUniformLocation(Handle, uniformName);
	}
}
