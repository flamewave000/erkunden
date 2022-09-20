using System;
using Erkunden.Client.AssetManagement;
using Erkunden.Core.Util;
using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.Graphics.Objects
{
	public partial class ShaderScript : GraphicsObject, IDisposable, Asset
	{
		public string Source { get; private set; } = null!;
		public string? LastError { get; private set; } = null;
		public ShaderType Type { get; private set; }
		public string Name { get; }

		public ShaderScript(string name, string source, ShaderType type)
		{
			Source = source;
			Type = type;
			Name = name;
		}

		public override void Bind()
		{
			if (!IsDisposed) return;
			Log.WriteLine($"@yellow;Compiling {Type} @Magenta;{Name}:{Handle}");
			// Create the shader object and bind the source code to it
			Handle = GL.CreateShader(Type);
			GL.ShaderSource(Handle, Source);
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
		public override void Dispose()
		{
			if (IsDisposed) return;
			Log.WriteLine($"@red;Destroyed {Type} @Magenta;{Name}:{Handle}");
			GL.DeleteShader(Handle);
			Handle = 0;
		}

		public override string ToString() => Name;
	}
}
