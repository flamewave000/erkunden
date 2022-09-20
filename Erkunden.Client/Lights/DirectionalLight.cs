using OpenTK.Mathematics;

namespace Erkunden.Client.Lights
{
	public class DirectionalLight : Light
	{
		public Vector3 Direction = new Vector3(-1, 0, 1);

		public DirectionalLight() : base(LightType.Directional) { }

		protected override void FillData(ref LightData lightData)
		{
			lightData.Direction = new Vector4(Direction.Normalized(), 0);
		}
	}
}
