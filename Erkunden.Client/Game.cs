using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Erkunden.Client.AssetManagement;
using Erkunden.Client.AssetManagement.Fonts;
using Erkunden.Client.AssetManagement.Materials;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.AssetManagement.Textures;
using Erkunden.Client.Entities.Scenes;
using Erkunden.Client.Graphics.Data;
using Erkunden.Client.Utils;
using Erkunden.Core;
using Erkunden.Core.Systems;
using Erkunden.Core.Util;
using Erkunden.ECS;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Erkunden.Client
{
	public class Game : GameWindow
	{
		GameTime updateTime = new GameTime { ellapsed = 0, total = 0 };
		GameTime renderTime = new GameTime { ellapsed = 0, total = 0 };

		PhysicsSystem PhysicsSystem = new PhysicsSystem();

		RenderLevel[] Levels;
		Dictionary<RenderLevel, Shader> Layers = new Dictionary<RenderLevel, Shader>();
		List<Scene> Scenes = new List<Scene>();

		public Game() : base(GameWindowSettings.Default, new NativeWindowSettings()
		{
			Title = "Erkunden",
			Size = new Vector2i(800, 600)
		})
		{
			Levels = Enum.GetValues(typeof(RenderLevel)).Cast<RenderLevel>().ToArray();
			Array.Sort(Levels);
		}

		#region Load/Unload
		protected override void OnLoad()
		{
			Log.WriteLine("@blue;Game::OnLoad()");
			Log.Indent();

			InputManager.Initialize(KeyboardState, MouseState);

			GL.Enable(EnableCap.DebugOutput);
			GL.DebugMessageCallback(DebugMessageHandler, IntPtr.Zero);

			GL.ClearColor(new Color4(32, 32, 32, 255));
			GL.ClearDepth(float.MaxValue);

			ObjParser.Register();
			MtlParser.Register();
			ImageSharpParser.Register();
			GLSLParser.Register();
			BMFParser.Register();
			AssetProvider.LoadAssets("Assets/");

			Layers[RenderLevel.SkyBox] = AssetProvider.Get<Shader>("SkyBox");
			Layers[RenderLevel.Default] = AssetProvider.Get<Shader>("Default");
			Layers[RenderLevel.WireFrame] = AssetProvider.Get<Shader>("WireFrame");

			SpriteBatch.Initialize();

			Scenes.Add(EntityFactory.Create<SpaceScene>(this));

			base.OnLoad();
			Log.Dedent();
		}
		protected override void OnUnload()
		{
			Log.WriteLine("@blue;Game::OnUnload()");
			Log.Indent();

			GL.UseProgram(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			EntityFactory.Save();
			EntityFactory.DestroyAll();
			Layers.ForEach(x => x.Value.Dispose());
			AssetProvider.Dispose();

			base.OnUnload();
			Log.Dedent();
		}
		#endregion

		#region Game Loop
		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			RefreshTime(args, ref updateTime);

			var gameObjects = EntityFactory.Entities.FilterAsType<ClientGameObject>();
			// Perform Pre-Update
			foreach (var gameObject in gameObjects)
				gameObject.OnPreUpdate(updateTime);

			// Perform Update and System Processing
			foreach (var entity in EntityFactory.Entities)
			{
				(entity as IUpdate)?.OnUpdate(updateTime);
				PhysicsSystem.Process(entity, updateTime);
			}

			// Perform Post-Update
			foreach (var gameObject in gameObjects)
				gameObject.OnPostUpdate(updateTime);

			base.OnUpdateFrame(args);
		}
		protected override void OnRenderFrame(FrameEventArgs args)
		{
			RefreshTime(args, ref renderTime);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			foreach (var level in Levels)
			{
				if (!Layers.TryGetValue(level, out var shader)) continue;

				shader.Use();
				foreach (var scene in Scenes)
				{
					scene.OnPreDraw(shader, renderTime);

					var sceneObjects = scene
						.GetAllChildren()
						.FilterAsType<ClientGameObject>()
						.Where(x => x.IsVisible && x.Level.HasFlag(level))
						.ToList();
					foreach (var gameObject in sceneObjects)
					{
						gameObject.OnPreDraw(shader, renderTime);
					}
					foreach (var gameObject in sceneObjects)
					{
						gameObject.OnDraw(shader, renderTime);
					}
					foreach (var gameObject in sceneObjects)
					{
						gameObject.OnPostDraw(shader, renderTime);
					}
				}
			}

			SpriteBatch.DrawBatchedSprites();

			Context.SwapBuffers();
			base.OnRenderFrame(args);
		}
		#endregion


		private void RefreshTime(in FrameEventArgs args, ref GameTime gameTime)
		{
			gameTime.total += args.Time;
			gameTime.ellapsed = args.Time;
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

	}
}
