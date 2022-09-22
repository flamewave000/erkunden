using System;
using Erkunden.Client.Utils;
using Erkunden.ECS;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Erkunden.Client.Components
{
	public class Controller : IComponent
	{
		// For fast vectors, pre-calc sqrts of unit vectors.
		private float sqrt2 = (float)(1.0 / Math.Sqrt(2.0));
		private float sqrt3 = (float)(1.0 / Math.Sqrt(3.0));

		public Vector3 GetMovement()
		{
			int X = 0, Y = 0, Z = 0;
			if (InputManager.Keyboard.IsKeyDown(Keys.W) || InputManager.Keyboard.IsKeyDown(Keys.Up)) Z--;
			if (InputManager.Keyboard.IsKeyDown(Keys.S) || InputManager.Keyboard.IsKeyDown(Keys.Down)) Z++;
			if (InputManager.Keyboard.IsKeyDown(Keys.D) || InputManager.Keyboard.IsKeyDown(Keys.Right)) X++;
			if (InputManager.Keyboard.IsKeyDown(Keys.A) || InputManager.Keyboard.IsKeyDown(Keys.Left)) X--;
			if (InputManager.Keyboard.IsKeyDown(Keys.Space)) Y++;
			if (InputManager.Keyboard.IsKeyDown(Keys.LeftControl)) Y--;
			int total = Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
			Vector3 result = new Vector3(X, Y, Z);
			return total < 2 ? result : total < 3 ? result * sqrt2 : result * sqrt3;
		}

		public Vector3 GetRotation(bool invertedY = true)
		{
			int X = 0, Y = 0, Z = 0;
			if (InputManager.Keyboard.IsKeyDown(Keys.KeyPad8)) X++;
			if (InputManager.Keyboard.IsKeyDown(Keys.KeyPad5)) X--;
			if (InputManager.Keyboard.IsKeyDown(Keys.KeyPad6)) if (invertedY) Y++; else Y--;
			if (InputManager.Keyboard.IsKeyDown(Keys.KeyPad4)) if (invertedY) Y--; else Y++;
			if (InputManager.Keyboard.IsKeyDown(Keys.KeyPad9)) Z++;
			if (InputManager.Keyboard.IsKeyDown(Keys.KeyPad7)) Z--;
			int total = Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
			Vector3 result = new Vector3(X, Y, Z);
			return total < 2 ? result : total < 3 ? result * sqrt2 : result * sqrt3;
		}
	}
}
