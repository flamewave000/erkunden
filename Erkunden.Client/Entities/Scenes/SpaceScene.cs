using System;
using Erkunden.Client.Entities.SkyBoxes;
using Erkunden.Client.Lights;
using OpenTK.Mathematics;

namespace Erkunden.Client.Entities.Scenes
{
	public class SpaceScene : Scene
	{
		public SpaceScene(Game game) : base(game) { }

		protected override void OnSetup()
		{
			base.OnSetup();
			CreateChild<NebulaSkyBox>();
			CreateChild<Grid>().Transform.Position = new Vector3(0, -10f, 0);
			CreateChild<Player>();

			var planet = CreateChild<Planet>(0);
			planet.Transform.Scale *= 1000;
			planet.Transform.Position = new Vector3(0, 0, -1000);
			planet.Transform.Rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, (float)Math.PI / 4);
			//planet.Momentum.Angular = new Vector3(0, (float)Math.PI / 4, 0);

			DefaultCamera.Position = new Vector3(0, 2, 5);

			Lights.Add(new DirectionalLight()
			{
				AmbientColor = Color4.White,
				AmbientStrength = 0.5f,
				Color = Color4.White,
				ColorIntensity = 1f,
				Direction = new Vector3(1, -1, -1)
			});

			var rand = new Random();
			for (int i = 0; i < 1000; i++)
			{
				CreateChild<Crate>().Transform.Position = new Vector3(
					((float)rand.NextDouble() * 1000) - 500f,
					((float)rand.NextDouble() * 1000) - 500f,
					((float)rand.NextDouble() * 1000) - 500f
				);
			}
		}
	}
}
