using Erkunden.Core.Ships;

namespace Erkunden.Core.Equipment
{
	public interface IDevice : IConnectable
	{
		public bool IsOnline { get; set; }
		public float PowerDemand { get; }
	}
}
