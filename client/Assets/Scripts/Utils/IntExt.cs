namespace Assets.Scripts.Utils
{
	public static class IntExt
	{
		public static bool HasFlags(this int value, int flags) => (value & flags) != 0;
		public static bool HasFlags(this int value, uint flags) => (value & flags) != 0;
		public static bool HasFlags(this int value, long flags) => (value & flags) != 0;
		public static bool HasFlags(this int? value, int flags) => value != null && (value & flags) != 0;
		public static bool HasFlags(this int? value, uint flags) => value != null && (value & flags) != 0;
		public static bool HasFlags(this int? value, long flags) => value != null && (value & flags) != 0;

		public static bool HasFlags(this uint value, int flags) => (value & flags) != 0;
		public static bool HasFlags(this uint value, uint flags) => (value & flags) != 0;
		public static bool HasFlags(this uint value, long flags) => (value & flags) != 0;
		public static bool HasFlags(this uint value, ulong flags) => (value & flags) != 0;
		public static bool HasFlags(this uint? value, int flags) => value != null && (value & flags) != 0;
		public static bool HasFlags(this uint? value, uint flags) => value != null && (value & flags) != 0;
		public static bool HasFlags(this uint? value, long flags) => value != null && (value & flags) != 0;
		public static bool HasFlags(this uint? value, ulong flags) => value != null && (value & flags) != 0;

		public static bool HasFlags(this long value, int flags) => (value & flags) != 0;
		public static bool HasFlags(this long value, uint flags) => (value & flags) != 0;
		public static bool HasFlags(this long value, long flags) => (value & flags) != 0;
		public static bool HasFlags(this long? value, int flags) => value != null && (value & flags) != 0;
		public static bool HasFlags(this long? value, uint flags) => value != null && (value & flags) != 0;
		public static bool HasFlags(this long? value, long flags) => value != null && (value & flags) != 0;

		public static bool HasFlags(this ulong value, uint flags) => (value & flags) != 0;
		public static bool HasFlags(this ulong value, ulong flags) => (value & flags) != 0;
		public static bool HasFlags(this ulong? value, uint flags) => value != null && (value & flags) != 0;
		public static bool HasFlags(this ulong? value, ulong flags) => value != null && (value & flags) != 0;
	}
}
