using Erkunden.Client.AssetManagement;
using Erkunden.Client.AssetManagement.Materials;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.AssetManagement.Textures;
using Erkunden.Client.Entities.Scenes;
using Erkunden.Client.Graphics.Data;
using Erkunden.Core.Utils;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Erkunden.Client.Entities
{
	public class SkyBox : ClientGameObject
	{
		public float Size
		{
			get => Transform.Scale.X;
			set => Transform.Scale = new Vector3(value, value, value);
		}
		public Model SkyboxModel = null!;

		public SkyBox()
		{
			Transform.Scale = Vector3.One * 100f;
			Level = RenderLevel.SkyBox;
		}

		protected override void OnSetup()
		{
			base.OnSetup();
			SkyboxModel = AssetProvider.Get<Model>("Skybox");
			AssignCubeMap(SkyboxModel.Parts[0].Material.DiffuseMap ?? AssetProvider.Get<Texture>("SkyboxPlaceholder"));
		}

		public void AssignCubeMap(string assetName) => AssignCubeMap(AssetProvider.Get<Texture>(assetName));
		public void AssignCubeMap(Texture texture)
		{
			ref Material material = ref SkyboxModel.Parts[0].Material;
			material.DiffuseMap = texture;
			material.DiffuseMap!.TextureWrapS = TextureWrapMode.ClampToEdge;
			material.DiffuseMap!.TextureWrapT = TextureWrapMode.ClampToEdge;
		}

		public override void OnPostUpdate(in GameTime gameTime)
		{
			// Note: noop function as we calculate our matrix in Pre-Draw
		}

		public override void OnPreDraw(Shader shader, in GameTime gameTime)
		{
			base.OnPreDraw(shader, gameTime);
			Transform.Position = GetParent<Scene>().CurrentOrDefaultCamera.Position;
			Transform.UpdateMatrix();
		}

		public override void OnDraw(Shader shader, in GameTime gameTime)
		{
			base.OnDraw(shader, gameTime);
			SkyboxModel.Draw(shader);
		}
	}
}
