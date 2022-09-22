using Erkunden.Client.AssetManagement;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.AssetManagement.Textures;
using Erkunden.Core.Utils;

namespace Erkunden.Client.Entities
{
	public class Planet : ClientGameObject
	{
		public Model Sphere = null!;
		public int PlanetType = 0;

		public Planet(int planetType) { PlanetType = planetType; }

		protected override void OnSetup()
		{
			base.OnSetup();
			Sphere = AssetProvider.Get<Model>("Planet");
			Sphere.Parts[0].Material.DiffuseMap = AssetProvider.Get<Texture>("planet_" + PlanetType);
			Sphere.Parts[0].Material.NormalMap = AssetProvider.Get<Texture>("planet_" + PlanetType + "_norm");
		}

		public override void OnDraw(Shader shader, in GameTime gameTime)
		{
			base.OnDraw(shader, gameTime);
			Sphere.Draw(shader);
		}
	}
}
