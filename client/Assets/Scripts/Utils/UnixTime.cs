using System;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.Utils
{
	public static class UnixTime
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long now() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}
}
