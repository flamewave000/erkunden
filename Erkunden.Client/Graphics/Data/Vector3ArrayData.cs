using OpenTK.Mathematics;

namespace Erkunden.Client.Graphics.Data
{
	public class Vector3ArrayData : ArrayData<Vector3>
	{
		public Vector3ArrayData() : base(Vector3.SizeInBytes) { }
		public Vector3ArrayData(Vector3[] data) : base(Vector3.SizeInBytes, data) { }
	}
}
