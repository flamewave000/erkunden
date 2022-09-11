namespace Erkunden.Client.Graphics.Data
{
	public abstract class ArrayData<T> where T : struct
	{
		public T[] Data;
		public int ElementSize { get; protected set; }
		public int TotalByteSize => Data.Length * ElementSize;

		protected ArrayData(int dataByteSize)
		{
			Data = new T[0];
			ElementSize = dataByteSize;
		}
		protected ArrayData(int dataByteSize, T[] data)
		{
			Data = data;
			ElementSize = dataByteSize;
		}
		public override string ToString() => $"{typeof(T).Name}[{Data.Length}]";
	}
}
