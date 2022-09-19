using Erkunden.Client.AssetManagement.Shaders;
using OpenTK.Mathematics;

namespace Erkunden.Client.Lights
{
	public class PointLight : Light
	{
		public float Constant = 1.0f;
		public float LinearFalloff = 0.22f;
		public float QuadraticFalloff = 0.2f;
		public Vector3 Position = Vector3.Zero;

		public override void Bind(Shader shader, int index)
		{
			base.Bind(shader, index);
			shader.SetInt("u_Lights[" + index + "].type", (int)LightType.Point);
			shader.SetVector3("u_Lights[" + index + "].position", Position);
			shader.SetFloat("u_Lights[" + index + "].constant", Constant);
			shader.SetFloat("u_Lights[" + index + "].linear", LinearFalloff);
			shader.SetFloat("u_Lights[" + index + "].quadratic", QuadraticFalloff);
		}
	}
}
