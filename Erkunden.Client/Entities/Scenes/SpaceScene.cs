using System;
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
			Player = CreateChild<Player>();

			DefaultCamera.Position = new Vector3(0, 2, 5);
			DefaultCamera.Direction = -DefaultCamera.Position;

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

		public override void OnPostUpdate(in GameTime gameTime)
		{
			base.OnPostUpdate(gameTime);
			//DefaultCamera.Direction = (Player.Transform.Position - DefaultCamera.Position).Normalized();
		}
	}
}
