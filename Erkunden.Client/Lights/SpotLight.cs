using Erkunden.Client.AssetManagement.Shaders;
using OpenTK.Mathematics;

namespace Erkunden.Client.Lights
{
	public class SpotLight : PointLight
	{
		public Vector3 Direction = -Vector3.UnitZ;
		public float CutOff = MathHelper.DegreesToRadians(45f);

		public override void Bind(Shader shader, int index)
		{
			base.Bind(shader, index);
			shader.SetInt("u_Lights[" + index + "].type", (int)LightType.Spot);
			shader.SetVector3("u_Lights[" + index + "].direction", Direction.Normalized());
			shader.SetFloat("u_Lights[" + index + "].cutoff", CutOff);
		}
	}
}
