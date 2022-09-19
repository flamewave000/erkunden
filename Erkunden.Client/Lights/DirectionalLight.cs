using Erkunden.Client.AssetManagement.Shaders;
using OpenTK.Mathematics;

namespace Erkunden.Client.Lights
{
	public class DirectionalLight : Light
	{
		public Vector3 Direction = new Vector3(-1, 0, 1).Normalized();

		public override void Bind(Shader shader, int index)
		{
			base.Bind(shader, index);
			shader.SetInt("u_Lights[" + index + "].type", (int)LightType.Directional);
			shader.SetVector3("u_Lights[" + index + "].direction", Direction.Normalized());
		}
	}
}
