using Erkunden.Core.Utils;

namespace Erkunden.Client
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Log.WriteLine("Starting Game");
			using (Game game = new Game())
			{
				game.Run();
			}
#if DEBUG
			//Log.Pause();
#endif
		}
	}
}
