using System;
using Erkunden.Client.AssetManagement;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Core.Components;
using Erkunden.Core.Utils;
using OpenTK.Mathematics;

namespace Erkunden.Client.Entities
{
	public class Crate : ClientGameObject
	{
		private Random Rand = new Random();
		public Model CrateModel { get; private set; } = null!;

		float RandomAngle(float from, float to)
		{
			if (new Random().NextDouble() > 0.5)
				return (float)Rand.NextDouble() * (to - from) + from;
			else
				return -((float)Rand.NextDouble() * (to - from) + from);
		}

		protected override void OnSetup()
		{
			base.OnSetup();
			Transform.Scale *= 5;
			CrateModel = AssetProvider.Get<Model>("Crate");
			//Momentum = Add<Momentum>();
			//Momentum.Angular.X = MathHelper.DegreesToRadians(RandomAngle(20f, 90f));
			//Momentum.Angular.Z = MathHelper.DegreesToRadians(RandomAngle(20f, 90f));
			//Momentum.Angular.Y = MathHelper.DegreesToRadians(RandomAngle(20f, 90f));
		}

		public override void OnDraw(Shader shader, in GameTime gameTime)
		{
			base.OnDraw(shader, gameTime);
			CrateModel.Draw(shader);
		}
	}
}
