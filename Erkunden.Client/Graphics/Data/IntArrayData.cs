namespace Erkunden.Client.Graphics.Data
{
	public class IntArrayData : ArrayData<int>
	{
		public IntArrayData() : base(sizeof(int)) { }
		public IntArrayData(int[] data) : base(sizeof(int), data) { }
	}
}
