using System;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.Entities.SkyBoxes;
using Erkunden.Core.Util;
using OpenTK.Mathematics;

namespace Erkunden.Client.Entities.Scenes
{
	public class SpaceScene : Scene
	{
		private Player Player = null!;

		public SpaceScene(Game game) : base(game) { }

		protected override void OnSetup()
		{
			base.OnSetup();

			CreateChild<NebulaSkyBox>();
			CreateChild<Grid>().Transform.Position = new Vector3(0, -10f, 0);
			Player = CreateChild<Player>();

			DefaultCamera.Position = new Vector3(0, 2, 5);

			var rand = new Random();
			for (int i = 0; i < 100; i++)
			{
				CreateChild<Crate>().Transform.Position = new Vector3(
					((float)rand.NextDouble() * 100) - 50f,
					((float)rand.NextDouble() * 100) - 50f,
					((float)rand.NextDouble() * 100) - 50f
				);
			}
		}

		public override void OnPreDraw(Shader shader, in GameTime gameTime)
		{
			base.OnPreDraw(shader, gameTime);
			shader.SetVector3("u_LightPos", Vector3.Zero);
			shader.SetColor4("u_LightColor", Color4.White);
			shader.SetVector3("u_EyePos", CurrentCamera!.Position);
		}

		public override void OnPostUpdate(in GameTime gameTime)
		{
			base.OnPostUpdate(gameTime);
			//DefaultCamera.Direction = (Player.Transform.Position - DefaultCamera.Position).Normalized();
		}
	}
}
