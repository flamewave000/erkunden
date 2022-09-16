using System;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.Entities.Cameras;
using Erkunden.Client.Entities.Scenes;
using Erkunden.Client.Graphics.Data;
using Erkunden.Client.Utils;
using Erkunden.Core.Components;
using Erkunden.Core.Util;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Erkunden.Client.Entities
{
	public class Player : ClientGameObject
	{
		private int currentZoomLevel = 3;
		private float[] zoomLevels = new float[]
		{
			0.25f, 0.5f, 0.75f, 1.0f, 2.0f, 4.0f, 8.0f
		};

		public Momentum Momentum { get; private set; } = null!;
		public ThirdPersonCamera Camera = null!;
		public Model Ship = null!;

		public float Acceleration = 100;
		public float Speed => Momentum.Linear.LengthFast;

		protected override void OnSetup()
		{
			base.OnSetup();

			Transform.Scale *= 100f;

			Ship = AssetManagement.AssetProvider.Get<Model>("Fighter_Ship");

			Camera = Add<ThirdPersonCamera>();
			Camera.Target = Transform;
			Camera.LookAtOffset = new Vector3(0, 0, 0);
			Camera.RelativePosition = ThirdPersonCamera.RelativePositionFromRadiusRotation(80f,
				Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(-15), 0, 0));

			GetParent<Scene>().CurrentCamera = Camera;


			Momentum = Add<Momentum>();
			Momentum.LinearDrag = Acceleration / 2;
		}

		public override void OnUpdate(in GameTime gameTime)
		{
			base.OnUpdate(gameTime);
			Vector3 velocity = Vector3.Zero;
			Vector3 angular = Vector3.Zero;
			Vector3 forward = Vector4.Transform(-Vector4.UnitZ, Transform.Rotation).Xyz;
			Vector3 right = Vector4.Transform(Vector4.UnitX, Transform.Rotation).Xyz;

			if (InputManager.Keyboard.IsKeyDown(Keys.W))
				velocity += forward;
			if (InputManager.Keyboard.IsKeyDown(Keys.S))
				velocity -= forward;
			if (InputManager.Keyboard.IsKeyDown(Keys.D))
				velocity += right;
			if (InputManager.Keyboard.IsKeyDown(Keys.A))
				velocity -= right;

			if (InputManager.Keyboard.IsKeyDown(Keys.Up))
				angular -= Vector3.UnitX;
			if (InputManager.Keyboard.IsKeyDown(Keys.Down))
				angular += Vector3.UnitX;
			if (InputManager.Keyboard.IsKeyDown(Keys.Left))
				angular += Vector3.UnitY;
			if (InputManager.Keyboard.IsKeyDown(Keys.Right))
				angular -= Vector3.UnitY;

			if (InputManager.Keyboard.IsKeyPressed(Keys.D1))
				Level = RenderLevel.Default;
			if (InputManager.Keyboard.IsKeyPressed(Keys.D2))
				Level = RenderLevel.Default | RenderLevel.WireFrame;
			if (InputManager.Keyboard.IsKeyPressed(Keys.D3))
				Level = RenderLevel.WireFrame;

			if (InputManager.Keyboard.IsKeyReleased(Keys.Space))
			{
				Transform.Position = Vector3.Zero;
				Transform.Rotation = Quaternion.Identity;
			}
			if (InputManager.Keyboard.IsKeyDown(Keys.Escape))
				GetParent<Scene>().Game.Close();

			currentZoomLevel = Math.Clamp(currentZoomLevel + (int)InputManager.Mouse.ScrollDelta.Y, 0, zoomLevels.Length - 1);
			Camera.Zoom = zoomLevels[currentZoomLevel];

			velocity.NormalizeFast();
			Momentum.Linear += velocity * Acceleration * (float)gameTime.ellapsed;
			Momentum.Angular = angular * MathHelper.DegreesToRadians(10) * (float)Math.PI;
		}

		public override void OnDraw(Shader shader, in GameTime gameTime)
		{
			base.OnDraw(shader, gameTime);
			Ship.Draw(shader);

			//SpriteFont.DrawText("Arial", 24, new Vector2(20, 20), "Hello World", Color4.Red);
		}
	}
}
