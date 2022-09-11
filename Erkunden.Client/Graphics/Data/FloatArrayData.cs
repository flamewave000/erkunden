namespace Erkunden.Client.Graphics.Data
{
	public class FloatArrayData : ArrayData<float>
	{
		public FloatArrayData() : base(sizeof(float)) { }
		public FloatArrayData(float[] data) : base(sizeof(float), data) { }
	}
}
