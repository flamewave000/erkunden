using System;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.Components;
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
		public Momentum Momentum { get; private set; } = null!;
		public ThirdPersonCamera Camera = null!;
		public Controller Controller = null!;
		public Model Ship = null!;

		private int currentZoomLevel = 3;
		private float[] zoomLevels = new float[]
		{
			0.25f, 0.5f, 0.75f, 1.0f, 2.0f, 4.0f, 8.0f
		};

		public float Acceleration = 100;

		public Vector2 TiltDirection = Vector2.Zero;
		public Vector2 TiltDirectionLerp = Vector2.Zero;

		public float Speed => Momentum.Linear.LengthFast;

		protected override void OnSetup()
		{
			base.OnSetup();
			Camera = Add<ThirdPersonCamera>();
			Camera.Target = Transform;
			Controller = Add<Controller>();
			Transform.Scale *= 100f;
			Ship = AssetManagement.AssetProvider.Get<Model>("Fighter_Ship");
			Momentum = Add<Momentum>();
			Momentum.LinearDrag = Acceleration / 2;
			Momentum.AngularDrag = MathHelper.DegreesToRadians(30) / 2;
			GetParent<Scene>().CurrentCamera = Camera;
			Camera.RelativePosition = ThirdPersonCamera.RelativePositionFromRadiusRotation(80f,
				Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(-15), 0, 0));
		}

		public override void OnUpdate(in GameTime gameTime)
		{
			base.OnUpdate(gameTime);
			HandleInput();

			var deltaRot = -Controller.GetRotation();
			var deltaPos = Vector3.Transform(Controller.GetMovement(), Transform.Rotation);

			TiltDirection = deltaPos.Zx;

			TiltDirectionLerp += (TiltDirection - TiltDirectionLerp) * 0.25f;

			Momentum.LinearAccel = deltaPos * Acceleration;
			Momentum.AngularAccel = deltaRot * MathHelper.DegreesToRadians(30);
		}

		public override void OnPostUpdate(in GameTime gameTime)
		{
			var velocity = new Vector3(TiltDirectionLerp.X, 0, -TiltDirectionLerp.Y);
			Transform.Tilt = Quaternion.FromEulerAngles(velocity * MathHelper.DegreesToRadians(5f));
			base.OnPostUpdate(gameTime);
		}

		public override void OnDraw(Shader shader, in GameTime gameTime)
		{
			base.OnDraw(shader, gameTime);
			Ship.Draw(shader);
			SpriteFont.DrawText("Consolas", 20, new Vector2(20, 0), @$"
        Position: {Round(Transform.Position)}
        Rotation: {Round(Transform.Rotation.ToEulerAngles())}
           Scale: {Round(Transform.Scale)}
 Linear Velocity: {Round(Momentum.Linear)}
Angular Velocity: {Round(Momentum.Angular)}
", Color4.White);
		}

		private Vector3 Round(Vector3 vec) => new Vector3((float)Math.Round(vec.X, 2), (float)Math.Round(vec.Y, 2), (float)Math.Round(vec.Z, 2));

		private void HandleInput()
		{
			if (InputManager.Keyboard.IsKeyPressed(Keys.D1))
				Level = RenderLevel.Default;
			if (InputManager.Keyboard.IsKeyPressed(Keys.D2))
				Level = RenderLevel.Default | RenderLevel.WireFrame;
			if (InputManager.Keyboard.IsKeyPressed(Keys.D3))
				Level = RenderLevel.WireFrame;
			if (InputManager.Keyboard.IsKeyReleased(Keys.R))
			{
				Transform.Reset(scale: false);
				Momentum.Reset();
			}
			if (InputManager.Keyboard.IsKeyDown(Keys.Escape))
				GetParent<Scene>().Game.Close();

			currentZoomLevel = Math.Clamp(currentZoomLevel + (int)InputManager.Mouse.ScrollDelta.Y, 0, zoomLevels.Length - 1);
			Camera.Zoom = zoomLevels[currentZoomLevel];
		}
	}
}
