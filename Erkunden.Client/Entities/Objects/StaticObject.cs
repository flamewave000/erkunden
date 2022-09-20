using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Core.Components;
using Erkunden.Core.Util;

namespace Erkunden.Client.Entities.Objects
{
	public class StaticObject : ClientGameObject
	{
		public Momentum Momentum = null!;
		public Model? Model = null;

		protected override void OnSetup()
		{
			base.OnSetup();
			Momentum = Add<Momentum>();
		}

		public override void OnDraw(Shader shader, in GameTime gameTime)
		{
			base.OnDraw(shader, gameTime);
			Model?.Draw(shader);
		}
	}
}
