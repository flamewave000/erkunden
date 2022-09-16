using System;
using System.Collections.Generic;
using Erkunden.Core.Util;
using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.Graphics.Objects
{
	public class ShaderProgram : GraphicsObject, IDisposable
	{
		public string? LastError { get; private set; } = null;

		public void Build(IEnumerable<ShaderScript> shaders)
		{
			if (!IsDisposed) return;
			Handle = GL.CreateProgram();

			Log.WriteLine($"@blue;Created Shader Program: @Magenta;{Handle}");
			Log.WriteLine($"@yellow;Compiling Shader Program @Magenta;{Handle}");
			Log.Indent();
			foreach (var shader in shaders)
			{
				shader.Bind();
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

			foreach (var shader in shaders)
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
		}
	}
}
