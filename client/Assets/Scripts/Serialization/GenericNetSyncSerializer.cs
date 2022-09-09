using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Net;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Scripts.Serialization
{
	public class GenericNetSyncSerializer : ISerializer<INetSynced>
	{
		private static readonly Dictionary<Type, object> serializerCache = new Dictionary<Type, object>();

		#region Type Caches
		private static readonly Type SerializeAttributeType = typeof(SerializeAttribute);
		private static readonly Type INetSyncedType = typeof(INetSynced);
		private static readonly Type ISerializerType = typeof(ISerializer<>);
		#endregion

		#region Known Serializers
		private static readonly Type Vector2Type = typeof(Vector2);
		private static readonly Type Vector3Type = typeof(Vector3);
		private static readonly Type Vector4Type = typeof(Vector4);
		private static readonly Type Vector2ToFloatArraySerializerType = typeof(Vector2ToFloatArraySerializer);
		private static readonly Type Vector3ToFloatArraySerializerType = typeof(Vector3ToFloatArraySerializer);
		private static readonly Type Vector4ToFloatArraySerializerType = typeof(Vector4ToFloatArraySerializer);
		#endregion

		private static Type getSerializerType(Type serializer, Type propertyType)
		{
			if (!serializerCache.ContainsKey(serializer))
				serializerCache.Add(serializer, Activator.CreateInstance(serializer));
			return ISerializerType.MakeGenericType(propertyType);
		}
		private static JToken invokeSerializer(Type serializer, PropertyInfo prop, object value) =>
			(JToken)getSerializerType(serializer, prop.PropertyType)
					.GetMethod(nameof(serialize))
					.Invoke(serializerCache[serializer], new object[] { value });
		private static void invokeDeserializer(Type serializer, PropertyInfo prop, object parent, JToken source)
		{
			getSerializerType(serializer, prop.PropertyType)
				.GetMethod(nameof(deserialize))
				.Invoke(serializerCache[serializer], new object[] { source, prop.GetValue(parent) });
		}

		public static TSerializer GetSerializer<TSerializer, TType>() where TSerializer : class, ISerializer<TType> =>
			serializerCache[getSerializerType(typeof(TSerializer), typeof(TType))] as TSerializer;

		public void deserialize(JToken source, INetSynced target)
		{
			// Fetch all properties that contain the SerializeAttribute for automatic serialization
			var props = source.GetType().GetProperties()
				.Select(prop => Tuple.Create(prop, (SerializeAttribute)Attribute.GetCustomAttribute(prop, SerializeAttributeType)))
				.Where(prop => prop.Item2 != null);

			JToken token;
			foreach (var (prop, attr) in props)
			{
				token = (source as JObject).GetValue(attr.name ?? prop.Name);
				// If we have a custom serializer, invoke it
				if (attr.serializer != null)
					invokeDeserializer(attr.serializer, prop, target, token);
				// If the property is an INetSync object, recursively invoke it's deserialization
				else if (INetSyncedType.IsAssignableFrom(prop.PropertyType))
					deserialize(token, (INetSynced)prop.GetValue(target));
				// If the type is known to have a custom serializer
				else if (Vector2Type.IsAssignableFrom(prop.PropertyType))
					invokeDeserializer(Vector2ToFloatArraySerializerType, prop, target, token);
				else if (Vector3Type.IsAssignableFrom(prop.PropertyType))
					invokeDeserializer(Vector3ToFloatArraySerializerType, prop, target, token);
				else if (Vector4Type.IsAssignableFrom(prop.PropertyType))
					invokeDeserializer(Vector4ToFloatArraySerializerType, prop, target, token);
				// The default serialization for the field
				else
					prop.SetValue(target, token.ToObject(prop.PropertyType));
			}
			target.LoadData((JObject)source);
		}

		public JToken serialize(INetSynced source)
		{
			var target = new JObject();
			// Fetch all properties that contain the SerializeAttribute for automatic serialization
			var props = source.GetType().GetProperties()
				.Select(prop => Tuple.Create(prop, (SerializeAttribute)Attribute.GetCustomAttribute(prop, SerializeAttributeType)))
				.Where(prop => prop.Item2 != null);

			JToken token;
			object value;
			foreach (var (prop, attr) in props)
			{
				value = prop.GetValue(source);
				// If there is a custom serializer defined, invoke it
				if (attr.serializer != null)
					token = invokeSerializer(attr.serializer, prop, value);
				// If the property is an INetSync object, recursively invoke it's serialization
				else if (INetSyncedType.IsAssignableFrom(prop.PropertyType))
					token = serialize(value as INetSynced);
				// If the type is known to have a custom serializer
				else if (value is Vector2)
					token = invokeSerializer(Vector2ToFloatArraySerializerType, prop, value);
				else if (value is Vector3)
					token = invokeSerializer(Vector3ToFloatArraySerializerType, prop, value);
				else if (value is Vector4)
					token = invokeSerializer(Vector4ToFloatArraySerializerType, prop, value);
				// The default serialization for the field
				else
					token = JToken.FromObject(value);

				target.Add(attr?.name ?? prop.Name, token);
			}
			// Invoke this 
			source.SaveData(target);
			return target;
		}
	}
}
