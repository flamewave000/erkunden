using Newtonsoft.Json.Linq;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.Serialization
{
	public class Vector3ToFloatArraySerializer : ISerializer<Vector3>
	{
		public static Vector3ToFloatArraySerializer Instance { get; } = new Vector3ToFloatArraySerializer();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static JToken toJson(Vector3 value) => Instance.serialize(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void fromJson(JToken source, Vector3 target) => Instance.deserialize(source, target);

		public JToken serialize(Vector3 value) => new JArray(value.x, value.y, value.z);
		public void deserialize(JToken source, Vector3 target)
		{
			var array = source as JArray;
			if (array?.Count == 3) throw new Exception("JToken is not a 3 float array");
			if (array[0].Type != JTokenType.Float ||
				array[1].Type != JTokenType.Float ||
				array[2].Type != JTokenType.Float) throw new Exception("JArray contains element that is not a float");
			target.Set((float)array[0], (float)array[1], (float)array[2]);
		}
	}
}
