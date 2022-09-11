using OpenTK.Mathematics;

namespace Erkunden.Client.Graphics.Data
{
	public class Vector2ArrayData : ArrayData<Vector2>
	{
		public Vector2ArrayData() : base(Vector2.SizeInBytes) { }
		public Vector2ArrayData(Vector2[] data) : base(Vector2.SizeInBytes, data) { }
	}
}
