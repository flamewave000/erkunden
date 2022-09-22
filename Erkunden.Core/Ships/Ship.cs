namespace Erkunden.Core.Ships
{
	public class Ship
	{
		/// <summary>
		/// Unique Identifier for the ship
		/// </summary>
		public RCMI RCMI = 0x000000_00_00_000000;

		public string Name = "";

		/// <summary>
		/// A value of 0-1 as a percentage of acceleration output by the <see cref="PrimaryThruster"/>.
		/// </summary>
		public float Throttle = 0;
		/// <summary>
		/// Provides the primary "GO" force for the ship.
		/// </summary>
		public Thruster? PrimaryThruster;
		/// <summary>
		/// Determines how quickly the ship can move laterally and turn.
		/// </summary>
		public Thruster? ControlThruster;
		/// <summary>
		/// Provides the ship with its power.
		/// </summary>
		public PowerPlant? PowerPlant;
	}
}
