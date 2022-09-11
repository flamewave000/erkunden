using System;
using System.Runtime.InteropServices;
using Erkunden.Client.AssetManagement;
using Erkunden.Client.AssetManagement.Materials;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.AssetManagement.Textures;
using Erkunden.Client.Graphics.Objects;
using Erkunden.Core.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Erkunden.Client
{
	internal class Game : GameWindow
	{
		Model? Triangle = null;
		Shader Shader;

		AssetProvider Assets = new AssetProvider();

		public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
		{
			Title = title,
			Size = new Vector2i(width, height)
		})
		{
			Shader = new Shader(new ShaderScript[]
			{
				new ShaderScript("Assets/Shaders/basic_vert.glsl", ShaderType.VertexShader),
				new ShaderScript("Assets/Shaders/basic_frag.glsl", ShaderType.FragmentShader)
			});

			ObjParser.Register(Assets);
			MtlParser.Register(Assets);
			ImageSharpParser.Register(Assets);
		}

		protected override void OnLoad()
		{
			Log.WriteLine("@blue;Game::OnLoad()");
			Log.Indent();

			GL.Enable(EnableCap.DebugOutput);
			GL.DebugMessageCallback(DebugMessageHandler, IntPtr.Zero);

			GL.ClearColor(new Color4(32, 32, 32, 255));

			Assets.LoadAsset("Assets/Models/Plane.obj");

			Triangle = Assets.Get<Model>("Plane");

			Shader.Build();

			Triangle.Initialize(Shader);

			base.OnLoad();
			Log.Dedent();
		}

		private void DebugMessageHandler(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
		{
			switch (severity)
			{
				case DebugSeverity.DebugSeverityHigh:
					Log.WriteLine($"@red;{source}:{type}:{id}:{severity}");
					break;
				case DebugSeverity.DebugSeverityMedium:
					Log.WriteLine($"@yellow;{source}:{type}:{id}:{severity}");
					break;
				case DebugSeverity.DebugSeverityLow:
					Log.WriteLine($"{source}:{type}:{id}:{severity}");
					break;
				case DebugSeverity.DontCare:
				case DebugSeverity.DebugSeverityNotification:
				default:
					return;
			}
			Log.WriteLine(Marshal.PtrToStringAnsi(message, length), indent: true);
		}

		protected override void OnUnload()
		{
			Log.WriteLine("@blue;Game::OnUnload()");
			Log.Indent();

			GL.UseProgram(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			Triangle?.Dispose();
			Shader.Dispose();
			Assets.Dispose();

			base.OnUnload();
			Log.Dedent();
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			GL.Viewport(0, 0, e.Width, e.Height);
			base.OnResize(e);
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			Shader.Bind();
			Triangle?.Draw(Shader);

			Context.SwapBuffers();
			base.OnRenderFrame(args);
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			base.OnUpdateFrame(args);
			GLExt.CheckErrors();
		}
	}
}
