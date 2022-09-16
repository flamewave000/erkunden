using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Erkunden.Core.Util
{
	public static class EnumerableExt
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> selector)
		{
			foreach (var item in source) selector(item);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> selector)
		{
			int __count = 0;
			foreach (var item in source) selector(item, __count++);
		}
	}
}
