using System;
using Erkunden.Core.Util;
using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.Graphics
{
	public partial class Shader
	{
		public class Program : GraphicsObject, IDisposable
		{
			public Shader[] Shaders { get; private set; }
			public string? LastError { get; private set; } = null;

			public Program(Shader[] shaders) { Shaders = shaders; }
			~Program() { Dispose(); }

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

			public override void Bind() => GL.UseProgram(Handle);
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

			public int GetAttribLocation(string attribName) => GL.GetAttribLocation(Handle, attribName);
		}
	}
}
