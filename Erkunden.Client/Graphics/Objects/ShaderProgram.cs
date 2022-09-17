using System;
using Erkunden.Core.Util;
using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.Graphics.Objects
{
	public class ShaderProgram : GraphicsObject, IDisposable
	{
		public readonly ShaderScript[] Scripts;
		public string? LastError { get; private set; } = null;

		public ShaderProgram(ShaderScript[] scripts)
		{
			Scripts = scripts;
		}

		public void Build()
		{
			if (!IsDisposed) return;

			Handle = GL.CreateProgram();

			Log.WriteLine($"@blue;Created Shader Program: @Magenta;{Handle}");
			Log.WriteLine($"@yellow;Compiling Shader Program @Magenta;{Handle}");
			Log.Indent();
			foreach (var script in Scripts)
			{
				script.Bind();
				GL.AttachShader(Handle, script.Handle);
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

			Log.Dedent();
		}

		public override void Bind()
		{
			GL.UseProgram(Handle);
		}
		public override void Dispose()
		{
			if (IsDisposed) return;
			foreach (var shader in Scripts)
			{
				GL.DetachShader(Handle, shader.Handle);
				shader.Dispose();
			}
			GL.DeleteProgram(Handle);
			Handle = 0;
		}
	}
}
