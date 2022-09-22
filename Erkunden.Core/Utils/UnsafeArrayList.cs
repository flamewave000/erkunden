using System;
using System.Collections;
using System.Collections.Generic;

namespace Erkunden.Core.Utils
{
	public class UnsafeArrayList<T> : ICollection<T>
	{
		public const int DEFAULT_CAPACITY = 8;

		private T[] data;
		private int count;
		private int resizeLength;

		public ref T[] UnsafeData => ref data;
		public int Count => count;
		public int Capacity
		{
			get => data.Length;
			set => Resize(value);
		}
		public bool IsReadOnly => false;

		public ref T this[int index] => ref data[index];

		public UnsafeArrayList() : this(DEFAULT_CAPACITY, DEFAULT_CAPACITY) { }
		public UnsafeArrayList(int startCapacity, int resizeLength)
		{
			count = 0;
			this.resizeLength = resizeLength;
			data = new T[startCapacity];
		}

		public T[] GetData()
		{
			T[] values = new T[count];
			Array.Copy(data, 0, values, 0, count);
			return values;
		}

		public void Add(T item)
		{
			if (data.Length == count)
				Resize(data.Length + resizeLength);
			data[count++] = item;
		}

		public void AddAll(IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				if (data.Length == count)
					Resize(data.Length + resizeLength);
				data[count++] = item;
			}
		}

		public bool Remove(T item)
		{
			int index = Array.IndexOf(data, item);
			if (index < 0) return false;
			Array.Clear(data, index, 1);
			int count = this.count--;
			// Shift the data down by one element
			for (int i = index + 1; i < count; i++)
				data[index - 1] = data[index];
			// If our array is too big, leniantly reduce its size
			if (data.Length - count > resizeLength * 2)
			{
				Resize(((count / resizeLength) + 1) * resizeLength);
			}
			return true;
		}

		public void Clear()
		{
			Array.Clear(data, 0, data.Length);
			count = 0;
			Resize(resizeLength);
		}

		public bool Contains(T item)
		{
			for (int c = 0; c < count; c++)
				if (Equals(item, data[c]))
					return true;
			return false;
		}

		public void CopyTo(T[] array, int arrayIndex) => Array.Copy(data, 0, array, arrayIndex, count);

		private void Resize(int capacity)
		{
			if (capacity == data.Length) return;
			if (capacity < count) throw new ArgumentOutOfRangeException("capacity", "Cannot resize to fewer elements than list already contains.");
			Array.Resize(ref data, capacity);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<T> GetEnumerator() => new Enumerator(this);
		private class Enumerator : IEnumerator<T>
		{
			private int index = 0;
			private UnsafeArrayList<T> list;
			public T Current => list[index];
			object? IEnumerator.Current => Current;

			public Enumerator(UnsafeArrayList<T> list) { this.list = list; }

			public void Dispose() => list = null!;
			public bool MoveNext() => ++index < list.Count;
			public void Reset() => index = 0;
		}
	}
}
