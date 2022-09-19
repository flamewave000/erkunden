using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.Lights;
using Erkunden.ECS;

namespace Erkunden.Client.Components
{
	public class LightCollection : IComponent, ICollection<Light>
	{
		public const int MaxLights = 10;

		public Light?[] Lights = new Light?[MaxLights] { null, null, null, null, null, null, null, null, null, null };

		public int Count { get; private set; }
		public bool IsReadOnly => false;


		public ref Light this[int index]
		{
			get
			{
				if (index >= Count) throw new IndexOutOfRangeException();
				return ref Lights[index]!;
			}
		}

		public void Add(Light item)
		{
			if (Count == MaxLights)
				throw new OverflowException();
			Lights[Count++] = item;
		}

		public bool Remove(Light item)
		{
			int index = 0;
			for (; index < Count && item != Lights[index]; index++) ;
			if (index == Count) return false;
			Lights[index] = null;
			if (index + 1 < Count && Lights[index] != null)
			{
				for (int i = index + 1; index < Count; index++)
					Lights[index - 1] = Lights[index];
			}
			Count--;
			return true;
		}

		public void Clear()
		{
			Count = 0;
			Array.Fill(Lights, null);
		}

		public bool Contains(Light item) => Lights.Any(x => item.Equals(x));
		public void CopyTo(Light[] array, int arrayIndex) => Array.Copy(Lights, arrayIndex, array, 0, Count);
		IEnumerator IEnumerable.GetEnumerator() => Lights.GetEnumerator();
		public IEnumerator<Light> GetEnumerator() => Lights.Where(x => x != null).Select(x => x!).GetEnumerator();

		public void BindLights(Shader shader)
		{
			shader.SetInt("u_LightCount", Count);
			for (int c = 0; c < Count; c++)
			{
				Lights[c]!.Bind(shader, c);
			}
		}
	}
}
