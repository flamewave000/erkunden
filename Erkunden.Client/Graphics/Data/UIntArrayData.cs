namespace Erkunden.Client.Graphics.Data
{
	public class UIntArrayData : ArrayData<uint>
	{
		public UIntArrayData() : base(sizeof(uint)) { }
		public UIntArrayData(uint[] data) : base(sizeof(uint), data) { }
	}
}
