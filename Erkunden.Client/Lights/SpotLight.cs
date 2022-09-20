using OpenTK.Mathematics;

namespace Erkunden.Client.Lights
{
	public class SpotLight : PointLight
	{
		public Vector3 Direction = -Vector3.UnitZ;
		public float CutOff = MathHelper.DegreesToRadians(45f);

		public SpotLight() : base(LightType.Spot) { }

		protected override void FillData(ref LightData lightData)
		{
			base.FillData(ref lightData);
			lightData.Direction = new Vector4(Direction, 1);
			lightData.CutOff = CutOff;
		}
	}
}
