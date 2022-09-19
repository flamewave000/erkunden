using Erkunden.Client.AssetManagement.Shaders;
using OpenTK.Mathematics;

namespace Erkunden.Client.Lights
{
	public class DirectionalLight : Light
	{
		public Vector3 Direction = new Vector3(-1, 0, 1).Normalized();

		public override void Bind(Shader shader)
		{
			base.Bind(shader);
			shader.SetVector3("u_Light.vector", Direction);
		}
	}
}
