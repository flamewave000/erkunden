using System;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.Utils
{
	public static class SerializableExt
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSerializable(this object instance) =>
			Attribute.GetCustomAttribute(instance.GetType(), typeof(SerializableAttribute)) != null;
	}
}
