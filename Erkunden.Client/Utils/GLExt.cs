using Erkunden.Core.Utils;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Erkunden.Client.Utils
{
	public static class GLExt
	{
		public static ErrorCode CheckErrors()
		{
			var error = GL.GetError();
			if (error != ErrorCode.NoError)
				Log.WriteLine("@red;Error! " + error.ToString());
			return error;
		}

		public static Color4 Mult(this Color4 self, float scalar) =>
			new Color4(self.R * scalar, self.G * scalar, self.B * scalar, self.A * scalar);
	}
}
