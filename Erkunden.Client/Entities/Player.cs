using System;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.Components;
using Erkunden.Client.Entities.Cameras;
using Erkunden.Client.Entities.Scenes;
using Erkunden.Client.Graphics.Data;
using Erkunden.Client.Utils;
using Erkunden.Core.Components;
using Erkunden.Core.Physics.Colliders;
using Erkunden.Core.Physics.CollisionObjects;
using Erkunden.Core.Utils;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Erkunden.Client.Entities
{
	public class Player : ClientGameObject
	{
		public RigidBody RigidBody { get; private set; } = null!;
		public ThirdPersonCamera Camera = null!;
		public Controller Controller = null!;
		public Model Ship = null!;

		private int currentZoomLevel = 0;
		private float[] zoomLevels = new float[]
		{
			0.25f, 0.5f, 0.75f, 1.0f, 2.0f, 4.0f, 8.0f
		};

		public float PrimaryThruster = 100_000;
		public float OmniThruster = 10_000;
		public float GyroThruster = 1;

		protected override void OnSetup()
		{
			base.OnSetup();
			Camera = Add<ThirdPersonCamera>();
			Camera.Target = Transform;
			Controller = Add<Controller>();
			Transform.Scale *= 100f;
			Ship = AssetManagement.AssetProvider.Get<Model>("Fighter_Ship");
			GetParent<Scene>().CurrentCamera = Camera;
			Camera.RelativePosition = ThirdPersonCamera.RelativePositionFromRadiusRotation(80f,
				Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(-15), 0, 0));

			RigidBody = Add(new RigidBody(new SphereCollider(), Transform));
			RigidBody.Mass = 10_000;
			RigidBody.Inertia = 1_000;
			RigidBody.Friction = 100;
		}

		public override void OnUpdate(in GameTime gameTime)
		{
			base.OnUpdate(gameTime);
			HandleInput();

			var deltaRot = Controller.GetRotation();
			RigidBody.Torque = deltaRot * GyroThruster;
			//var rotation = Transform.Rotation.Normalized();

			var deltaPos = Controller.GetMovement();
			var forward = Vector3.Transform(Vector3.UnitZ, Transform.Rotation);
			var right = Vector3.Transform(Vector3.UnitX, Transform.Rotation);
			var up = Vector3.Transform(Vector3.UnitY, Transform.Rotation);
			RigidBody.Force =
				forward * deltaPos.Z * (InputManager.Keyboard.IsKeyDown(Keys.LeftShift) ? PrimaryThruster : OmniThruster) +
				right * deltaPos.X * OmniThruster +
				up * deltaPos.Y * OmniThruster;

			if (deltaPos.Length != 0)
				SpriteFont.DrawText("Consolas", 24, new Vector2(20, 150), deltaPos.Length.ToString(), Color4.White);
		}

		public override void OnDraw(Shader shader, in GameTime gameTime)
		{
			base.OnDraw(shader, gameTime);
			Ship.Draw(shader);
			SpriteFont.DrawText("Consolas", 20, new Vector2(20, 0), @$"
 Position: {Round(Transform.Position)}
 Rotation: {Round(Transform.Rotation.ToEulerAngles())}
	Scale: {Round(Transform.Scale)}
 Velocity: {Round(RigidBody.Velocity)}
	Force: {Round(RigidBody.Force)}
Net Force: {Round(RigidBody.NetForce)}
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
				RigidBody.Velocity = Vector3.Zero;
				RigidBody.AngularVelocity = Vector3.Zero;
			}
			if (InputManager.Keyboard.IsKeyDown(Keys.Escape))
				GetParent<Scene>().Game.Close();

			currentZoomLevel = Math.Clamp(currentZoomLevel + (int)InputManager.Mouse.ScrollDelta.Y, 0, zoomLevels.Length - 1);
			Camera.Zoom = zoomLevels[currentZoomLevel];
		}
	}
}
