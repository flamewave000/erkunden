using System;
using System.IO;
using System.Text;
using Erkunden.Core.Util;
using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.Graphics.Objects
{
	public partial class ShaderScript : GraphicsObject, IDisposable
	{
		public string Path { get; private set; }
		public string? Source { get; private set; } = null;
		public string? LastError { get; private set; } = null;
		public ShaderType Type { get; private set; }

		public ShaderScript(string path, ShaderType type)
		{
			Path = path;
			Type = type;
		}

		public void Build()
		{
			if (!IsDisposed) return;
			Log.WriteLine($"@yellow;Compiling @Magenta;{Type}:{Handle}");
			Log.WriteLine($"Path: {Path}", indent: true);

			// Create the shader object and bind the source code to it
			Handle = GL.CreateShader(Type);
			using (StreamReader reader = new StreamReader(Path, Encoding.UTF8))
			{
				Source = reader.ReadToEnd();
				GL.ShaderSource(Handle, Source);
			}
			// Compile the source code
			GL.CompileShader(Handle);
			// Check compilation result
			GL.GetShader(Handle, ShaderParameter.CompileStatus, out int success);
			if (success == 0)
			{
				LastError = GL.GetShaderInfoLog(Handle);
				Log.WriteLine("@red;Failed!", false, indent: true);
				Log.WriteLine(LastError, indent: true);
			}
			else
				Log.WriteLine("@green;Done.", indent: true);
		}
		public override void Bind() { }
		public override void Dispose()
		{
			if (IsDisposed) return;
			Log.WriteLine($"@red;Destroyed @Magenta;{Type}:{Handle}");
			Log.WriteLine(Path, indent: true);
			GL.DeleteShader(Handle);
			Handle = 0;
		}
	}
}
