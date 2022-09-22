namespace Erkunden.Core.Rooms
{
	public abstract class Room
	{
		public string Name = "";
		public int Size = 0;
		public abstract int MinimumSize { get; }
	}
}
