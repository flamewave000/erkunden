using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Assets.Scripts.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Scripts.Net
{
	/// <summary>
	/// General Network Message
	/// 
	/// <code>
	/// // Format
	/// (header)[:r(req_id)][:(type)[(value)]]
	/// // Example
	/// 1234:r1234:i1234
	/// </code>
	/// </summary>
	public struct Message
	{
		private static Dictionary<char, Type> _types = new Dictionary<char, Type>
		{
			{ '#', Type.NoData },
			{ 'y', Type.True },
			{ 'n', Type.False },
			{ 'i', Type.Int },
			{ 'f', Type.Double },
			{ 's', Type.String },
			{ 'o', Type.Object }
		};
		public enum Type
		{
			NoData = '#',
			True = 'y',
			False = 'n',
			Int = 'i',
			Double = 'f',
			String = 's',
			Object = 'o'
		}

		private static GenericNetSyncSerializer serializer = new GenericNetSyncSerializer();

		#region Fields
		private string packed;
		public Header header { get; private set; }
		public Type type { get; private set; }
		public uint? requestID { get; private set; }
		public string data { get; private set; }
		#endregion

		#region Metadata Properties
		public bool hasData => type != Type.NoData;
		public bool isError => header.HasFlags(Header.ERROR);
		public bool isRequest => requestID.HasValue;

		public bool isBoolean => type == Type.True || type == Type.False;
		public bool isInteger => type == Type.Int;
		public bool isDouble => type == Type.Double;
		public bool isString => type == Type.String;
		public bool isObject => type == Type.Object;
		#endregion

		#region Data Conversion
		public bool AsBool() => type == Type.True;
		public int AsInteger() => int.Parse(data);
		public double AsDouble() => double.Parse(data);
		public float AsFloat() => (float)AsDouble();
		public string AsString() => data;
		public T AsObject<T>() where T : class => JsonUtility.FromJson<T>(data);
		public JObject AsObject() => JObject.Parse(data);
		#endregion

		#region Constructors
		public Message(string message) : this()
		{
			Unpack(message);
		}
		#endregion

		#region Helpers
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Is(Header header) => this.header == header;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(Header header) => this.header.HasFlags((ushort)header);
		#endregion

		#region Packing/Unpacking Messages
		public string Pack()
		{
			if (packed != null) return packed;
			string message = ((ushort)header).ToString("X");
			if (requestID.HasValue)
				message += ":r" + requestID.Value.ToString("X");
			if (hasData)
			{
				message += ':' + (char)type;
				if (data != null) message += data;
			}
			return packed = message;
		}
		public void Unpack(string message)
		{
			string[] tokens = message.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
			if (tokens.Length == 0) throw new FormatException("Malformed message data");
			packed = message;
			type = Type.NoData;
			requestID = null;
			data = null;
			header = ushort.Parse(tokens[0], NumberStyles.HexNumber);
			if (tokens.Length > 1)
			{
				if (tokens[1][0] == 'r')
				{
					tokens = message.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
					requestID = uint.Parse(tokens[0].Substring(1), NumberStyles.HexNumber);
				}
				type = _types[tokens[1][0]];
				data = tokens[1].Substring(1);
			}
		}
		#endregion

		#region Builder Definitions
		public static IBuilder Create(Header header) => new Builder().Header(header);
		public abstract class IBuilder
		{
			public abstract IBuilder Header(Header value);
			public abstract IBuilder Request(uint? id);
			public abstract IBuilder Data(bool value);
			public abstract IBuilder Data(int value);
			public abstract IBuilder Data(long value);
			public abstract IBuilder Data(double value);
			public abstract IBuilder Data(INetSynced value);
			public abstract IBuilder Data(string value);
			public abstract Message Message();

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static implicit operator Message(IBuilder builder) => builder.Message();
		}
		private class Builder : IBuilder
		{
			#region Fields
			private Message message = new Message
			{
				header = Net.Header.INVALID,
				type = Type.NoData,
				requestID = null,
				data = null,
				packed = null
			};
			#endregion

			#region Flow Functions
			public override IBuilder Header(Header value)
			{
				message.header = value;
				return this;
			}
			public override IBuilder Request(uint? id)
			{
				message.requestID = id;
				return this;
			}
			public override IBuilder Data(bool value)
			{
				message.type = value ? Type.True : Type.False;
				message.data = null;
				return this;
			}
			public override IBuilder Data(int value)
			{
				message.type = Type.Int;
				message.data = value.ToString();
				return this;
			}
			public override IBuilder Data(long value)
			{
				message.type = Type.Double;
				message.data = ((double)value).ToString();
				return this;
			}
			public override IBuilder Data(double value)
			{
				message.type = Type.Double;
				message.data = value.ToString();
				return this;
			}
			public override IBuilder Data(INetSynced value)
			{
				message.type = Type.Object;
				message.data = serializer.serialize((INetSynced)value).ToString(Formatting.None);
				return this;
			}
			public override IBuilder Data(string value)
			{
				message.type = Type.String;
				message.data = value;
				return this;
			}
			#endregion

			public override Message Message() => message;
		}
		#endregion
	}
}
