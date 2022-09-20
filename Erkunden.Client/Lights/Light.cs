using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace Erkunden.Client.Lights
{
	public enum LightType
	{
		Point = 1,
		Directional = 2,
		Spot = 3
	}
	[StructLayout(LayoutKind.Sequential, Pack = 16)]
	public struct LightData
	{
		public Color4 Color;
		public Color4 AmbientColor;
		public Vector4 Position;
		public Vector4 Direction;
		public float ColorIntensity;
		public float AmbientStrength;
		public float Linear;
		public float Quadratic;
		public float Constant;
		public float CutOff;
		public int Type;
		private int _Padding0;

		public static readonly int SizeInBytes = Unsafe.SizeOf<LightData>();
	}
	public abstract class Light
	{
		protected LightType Type;
		public Color4 Color = Color4.White;
		public Color4 AmbientColor = Color4.White;
		public float AmbientStrength = 0.1f;
		public float ColorIntensity = 1f;

		protected Light(LightType type) { Type = type; }

		public LightData AsLightData()
		{
			LightData data = new LightData
			{
				Type = (int)Type,
				Color = Color,
				AmbientColor = AmbientColor,
				AmbientStrength = AmbientStrength,
				ColorIntensity = ColorIntensity,
				Position = Vector4.Zero,
				Direction = Vector4.Zero,
				Linear = 0,
				Quadratic = 0,
				Constant = 0,
				CutOff = 0
			};
			FillData(ref data);
			return data;
		}
		protected abstract void FillData(ref LightData lightData);
	}
}
