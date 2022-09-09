namespace Assets.Scripts.Utils
{
	public class Optional<T>
	{
		public T Value { get; private set; }
		public bool HasValue { get; private set; }
		private Optional() { }
		public static Optional<T> Of(T value) => new Optional<T> { Value = value, HasValue = true };
		public static Optional<T> Empty() => new Optional<T> { HasValue = false };

		public static explicit operator Optional<T>(T value) => Of(value);
		public static implicit operator T(Optional<T> value) => value.Value;
	}
}
