using Erkunden.Client.AssetManagement;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.Components;
using Erkunden.Client.Lights;
using Erkunden.Core.Util;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Erkunden.Client.Entities.Scenes
{
	public class LightScene : Scene
	{
		public LightScene(Game game) : base(game) { }

		float cameraRotation = 0f;

		public PointLight PointLight = new PointLight()
		{
			Position = new Vector3(4, 4, 2)
		};
		public ModelCollection Models = null!;
		public LightCollection Lights = null!;
		public Controller Controller = null!;
		public Quaternion CamRotation = Quaternion.Identity;

		protected override void OnSetup()
		{
			base.OnSetup();
			DefaultCamera.Position = new Vector3(0, 2, 5);

			Controller = Add<Controller>();
			Models = Add<ModelCollection>();
			Lights = Add<LightCollection>();

			Lights.Add(PointLight);
			Lights.Add(new DirectionalLight()
			{
				Direction = new Vector3(-1, 1, 1),
				AmbientStrength = 0,
				Color = Color4.CornflowerBlue
			});

			Models.AddModel(AssetProvider.Get<Model>("LightBall"));
			Models.AddModel(AssetProvider.Get<Model>("Crate"));
			Models.AddModel(AssetProvider.Get<Model>("Plane"));

			CreateChild<Grid>(1U).Transform.Position = new Vector3(0, -0.5f, 0);
		}

		public override void OnUpdate(in GameTime gameTime)
		{
			base.OnUpdate(gameTime);

			cameraRotation += Controller.GetRotation().Y * gameTime.ellapsed * 2;
			var rot = Quaternion.FromAxisAngle(Vector3.UnitY, cameraRotation);

			PointLight.Position += Vector3.Transform(Controller.GetMovement(), rot) * gameTime.ellapsed * 2;
			DefaultCamera.Position = Vector3.Transform(new Vector3(0, 2, 5), rot);
		}

		public override void OnDraw(Shader shader, in GameTime gameTime)
		{
			base.OnDraw(shader, gameTime);

			Lights.BindLights(shader);

			var model = Matrix4.Identity;
			shader.SetModel(ref model);
			Models["Crate"].Draw(shader);

			Matrix4.CreateTranslation(new Vector3(0, -0.45f, 0), out model);
			shader.SetModel(ref model);
			Models["Plane"].Draw(shader);

			//GL.CullFace(CullFaceMode.Front);
			Matrix4.CreateTranslation((Lights[0] as PointLight)!.Position, out model);
			shader.SetModel(ref model);
			Models["LightBall"].Draw(shader);
			//GL.CullFace(CullFaceMode.Back);
		}
	}
}
