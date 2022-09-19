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

		public override void Bind(Shader shader)
		{
			base.Bind(shader);
			shader.SetInt("u_Light.type", (int)LightType.Point);
			shader.SetVector3("u_Light.position", Position);
			shader.SetFloat("u_Light.constant", Constant);
			shader.SetFloat("u_Light.linear", LinearFalloff);
			shader.SetFloat("u_Light.quadratic", QuadraticFalloff);
		}
	}
}
