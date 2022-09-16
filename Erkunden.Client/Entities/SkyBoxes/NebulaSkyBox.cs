using Erkunden.Client.AssetManagement;
using Erkunden.Client.AssetManagement.Textures;

namespace Erkunden.Client.Entities.SkyBoxes
{
	/// <summary>
	/// 
	/// </summary>
	/// <seealso cref="https://tools.wwwtyro.net/space-3d"/>
	public class NebulaSkyBox : SkyBox
	{
		protected override void OnSetup()
		{
			base.OnSetup();
			AssignCubeMap(AssetProvider.Get<Texture>("Nebula"));
		}
	}
}
