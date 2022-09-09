using System.Runtime.CompilerServices;
using Assets.Scripts.Net;
using Assets.Scripts.Serialization;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects
{
	public class WorldObject : MonoBehaviour, INetSynced
	{
		public bool isDirty { get; private set; } = false;
		public string id { get; set; }

		public virtual void SaveData(JObject jobj)
		{
			jobj.Add("pos", Vector3ToFloatArraySerializer.toJson(transform.position));
			jobj.Add("rot", Vector3ToFloatArraySerializer.toJson(transform.eulerAngles));
			jobj.Add("scl", Vector3ToFloatArraySerializer.toJson(transform.localScale));
			jobj.Add(nameof(id), JValue.CreateString(id));
		}
		public virtual void LoadData(JObject jobj)
		{
			Vector3ToFloatArraySerializer.fromJson(jobj.GetValue("pos"), transform.position);
			Vector3ToFloatArraySerializer.fromJson(jobj.GetValue("rot"), transform.eulerAngles);
			Vector3ToFloatArraySerializer.fromJson(jobj.GetValue("scl"), transform.localScale);
			id = (string)jobj.GetValue(nameof(id));
		}

		protected virtual void Start() { }
		protected virtual void Update() { }
		protected virtual void OnDestroy() { }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invalidate() { if (!isDirty) isDirty = true; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void Set<T>(ref T target, T value)
		{
			target = value;
			Invalidate();
		}

		public void Publish(IChannel channel)
		{
			if (!isDirty) return;
			channel.Send(
				Message.Create(Header.ObjectUpdate)
					   .Data((INetSynced)this)
			);
		}
	}
}
