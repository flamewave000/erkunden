using Newtonsoft.Json.Linq;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.Serialization
{
	public class Vector4ToFloatArraySerializer : ISerializer<Vector4>
	{
		public static Vector4ToFloatArraySerializer Instance { get; } = new Vector4ToFloatArraySerializer();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static JToken toJson(Vector4 value) => Instance.serialize(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void fromJson(JToken source, Vector4 target) => Instance.deserialize(source, target);

		public JToken serialize(Vector4 value) => new JArray(value.x, value.y, value.z, value.w);
		public void deserialize(JToken source, Vector4 target)
		{
			var array = source as JArray;
			if (array?.Count == 4) throw new Exception("JToken is not a 4 float array");
			if (array[0].Type != JTokenType.Float ||
				array[1].Type != JTokenType.Float ||
				array[2].Type != JTokenType.Float ||
				array[3].Type != JTokenType.Float) throw new Exception("JArray contains element that is not a float");
			target.Set((float)array[0], (float)array[1], (float)array[2], (float)array[3]);
		}
	}
}
