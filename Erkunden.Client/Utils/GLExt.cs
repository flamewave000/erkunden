using Erkunden.Core.Util;
using OpenTK.Graphics.OpenGL4;

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
	}
}
