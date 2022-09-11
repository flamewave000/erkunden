namespace Erkunden.Client.AssetManagement.Textures
{
	public class Texture : Asset
	{
		public Texture(string name) : base(name) { }

		public override bool IsDisposed => false;
		public override void Dispose() { }
	}
}
