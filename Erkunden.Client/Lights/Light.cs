using Erkunden.Client.AssetManagement.Shaders;
using OpenTK.Mathematics;

namespace Erkunden.Client.Lights
{
	public abstract class Light
	{
		public Color4 Color = Color4.White;
		public Color4 AmbientColor = Color4.White;
		public float AmbientStrength = 0.1f;

		public virtual void Bind(Shader shader)
		{
			shader.SetColor4("u_Light.color", Color);
			shader.SetColor4("u_Light.ambientColor", AmbientColor);
			shader.SetFloat("u_Light.ambientStrength", AmbientStrength);
		}
	}
}
