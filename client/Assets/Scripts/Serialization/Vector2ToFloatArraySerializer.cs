using Newtonsoft.Json.Linq;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.Serialization
{
	public class Vector2ToFloatArraySerializer : ISerializer<Vector2>
	{
		public static Vector2ToFloatArraySerializer Instance { get; } = new Vector2ToFloatArraySerializer();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static JToken toJson(Vector2 value) => Instance.serialize(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void fromJson(JToken source, Vector2 target) => Instance.deserialize(source, target);

		public JToken serialize(Vector2 value) => new JArray(value.x, value.y);
		public void deserialize(JToken source, Vector2 target)
		{
			var array = source as JArray;
			if (array?.Count == 2) throw new Exception("JToken is not a 2 float array");
			if (array[0].Type != JTokenType.Float ||
				array[1].Type != JTokenType.Float) throw new Exception("JArray contains element that is not a float");
			target.Set((float)array[0], (float)array[1]);
		}
	}
}
