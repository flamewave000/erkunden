using Erkunden.Client.AssetManagement.Shaders;
using OpenTK.Mathematics;

namespace Erkunden.Client.Lights
{
	public enum LightType
	{
		Point = 0,
		Directional = 1,
		Spot = 2
	}
	public abstract class Light
	{
		public Color4 Color = Color4.White;
		public Color4 AmbientColor = Color4.White;
		public float AmbientStrength = 0.1f;

		public virtual void Bind(Shader shader, int index)
		{
			shader.SetColor4("u_Lights[" + index + "].color", Color);
			shader.SetColor4("u_Lights[" + index + "].ambientColor", AmbientColor);
			shader.SetFloat("u_Lights[" + index + "].ambientStrength", AmbientStrength);
		}
	}
}
