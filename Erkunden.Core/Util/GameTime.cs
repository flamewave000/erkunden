namespace Erkunden.Core.Util
{
	public struct GameTime
	{
		public float ellapsed;
		public float ellapsedSquared;
		public float total;
		public double ellapsedLong;
		public double ellapsedSquaredLong;
		public double totalLong;

		public void Update(double deltaTime)
		{
			totalLong += deltaTime;
			ellapsedLong = deltaTime;
			ellapsedSquaredLong = deltaTime * deltaTime;
			total = (float)totalLong;
			ellapsed = (float)ellapsedLong;
			ellapsedSquared = (float)ellapsedSquaredLong;
		}
	}
}
