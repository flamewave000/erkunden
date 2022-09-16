using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Erkunden.Client.Utils
{
	public static class InputManager
	{
		public static KeyboardState Keyboard { get; private set; } = null!;
		public static MouseState Mouse { get; private set; } = null!;

		public static void Initialize(KeyboardState kbd, MouseState mouse)
		{
			Keyboard = kbd;
			Mouse = mouse;
		}
	}
}
