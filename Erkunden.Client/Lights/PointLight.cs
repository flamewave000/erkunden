using OpenTK.Mathematics;

namespace Erkunden.Client.Lights
{
	public class PointLight : Light
	{
		public float Constant = 1.0f;
		public float LinearFalloff = 0.22f;
		public float QuadraticFalloff = 0.2f;
		public Vector3 Position = Vector3.Zero;

		protected PointLight(LightType type) : base(type) { }
		public PointLight() : base(LightType.Point) { }

		protected override void FillData(ref LightData lightData)
		{
			lightData.Linear = LinearFalloff;
			lightData.Quadratic = QuadraticFalloff;
			lightData.Constant = Constant;
			lightData.Position = new Vector4(Position, 1);
		}
	}
}
