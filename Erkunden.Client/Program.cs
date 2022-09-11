using Erkunden.Core.Util;

namespace Erkunden.Client
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Log.WriteLine("Starting Game");
			using (Game game = new Game(800, 600, "Erkunden"))
			{
				game.Run();
			}
#if DEBUG
			Log.Pause();
#endif
		}
	}
}
