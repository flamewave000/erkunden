using Erkunden.Client.AssetManagement;
using Erkunden.Client.AssetManagement.Materials;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.Graphics;
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
		Shader.Program Program;

		AssetProvider Assets = new AssetProvider();

		public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
		{
			Title = title,
			Size = new Vector2i(width, height)
		})
		{
			Program = new Shader.Program(new Shader[]
			{
				new Shader("Assets/Shaders/basic_vert.glsl", ShaderType.VertexShader),
				new Shader("Assets/Shaders/basic_frag.glsl", ShaderType.FragmentShader)
			});

			ObjParser.Register(Assets);
			MtlParser.Register(Assets);
		}

		protected override void OnLoad()
		{
			Log.WriteLine("@blue;Game::OnLoad()");
			Log.Indent();
			GL.ClearColor(Color4.CornflowerBlue);

			Assets.LoadAsset("Assets/Models/triangle.obj");

			Triangle = Assets.Get<Model>("Triangle");

			Program.Build();

			Triangle.Initialize(Program);

			base.OnLoad();
			Log.Dedent();
		}

		protected override void OnUnload()
		{
			Log.WriteLine("@blue;Game::OnUnload()");
			Log.Indent();

			GL.UseProgram(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			Triangle?.Dispose();
			Program.Dispose();

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

			Program.Bind();
			Triangle?.Draw();

			Context.SwapBuffers();
			base.OnRenderFrame(args);
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			base.OnUpdateFrame(args);
		}
	}
}
