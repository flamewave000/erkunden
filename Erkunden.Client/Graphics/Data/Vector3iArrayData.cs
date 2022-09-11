using OpenTK.Mathematics;

namespace Erkunden.Client.Graphics.Data
{
	public class Vector3iArrayData : ArrayData<Vector3i>
	{
		public Vector3iArrayData() : base(Vector3i.SizeInBytes) { }
		public Vector3iArrayData(Vector3i[] data) : base(Vector3i.SizeInBytes, data) { }
	}
}
