using System;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.Utils
{
	public static class Objects
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void RequireNotNull<T>(T arg) where T : class
		{
			if (arg == null) throw new ArgumentNullException();
		}
	}
}
