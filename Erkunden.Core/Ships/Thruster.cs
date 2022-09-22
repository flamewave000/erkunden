namespace Erkunden.Core.Ships
{
	public enum ThrusterType
	{
		Primary,
		Control
	}
	public struct Thruster
	{
		public ThrusterType Type;
		/// <summary>Measured in Km/s</summary>
		public float MaximumSpeed { get; }
		/// <summary>Measured in Km/s^2</summary>
		public float Acceleration { get; }
		/// <summary>Measured in Mega Watts</summary>
		public float PowerConsumption { get; }
	}
}
