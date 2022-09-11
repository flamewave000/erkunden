namespace Erkunden.Client.Graphics.Data
{
	public class DoubleArrayData : ArrayData<double>
	{
		public DoubleArrayData() : base(sizeof(double)) { }
		public DoubleArrayData(double[] data) : base(sizeof(double), data) { }
	}
}
