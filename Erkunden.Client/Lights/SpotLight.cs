using Erkunden.Client.AssetManagement.Shaders;
using OpenTK.Mathematics;

namespace Erkunden.Client.Lights
{
	public class SpotLight : PointLight
	{
		public Vector3 Direction = -Vector3.UnitZ;
		public float CutOff = MathHelper.DegreesToRadians(45f);

		public override void Bind(Shader shader)
		{
			base.Bind(shader);
			shader.SetInt("u_Light.type", (int)LightType.Spot);
			shader.SetVector3("u_Light.direction", Direction.Normalized());
			shader.SetFloat("u_Light.cutoff", CutOff);
		}
	}
}
