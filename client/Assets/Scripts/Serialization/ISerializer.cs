using Newtonsoft.Json.Linq;
using System;

namespace Assets.Scripts.Serialization
{
	public interface ISerializer<T>
	{
		/// <summary>
		/// Serializes the source value to a JToken object.
		/// </summary>
		/// <param name="value">Value to be serialized.</param>
		/// <returns>JToken containing the serialized value.</returns>
		JToken serialize(T value);

		/// <summary>
		/// Deserializes a JToken source to a target object.
		/// </summary>
		/// <param name="source">JSON source token.</param>
		/// <param name="target">Target object for JSON data to be bound to.</param>
		void deserialize(JToken source, T target);
	}
}
