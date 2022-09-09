using Assets.Scripts.Serialization;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects.Mobiles
{
	[RequireComponent(typeof(Rigidbody))]
	public class Mobile : WorldObject
	{
		public Rigidbody rigidBody;

		protected override void Start()
		{
			base.Start();
		}

		protected override void Update()
		{
			Invalidate();
			base.Update();
		}

		public override void SaveData(JObject jobj)
		{
			base.SaveData(jobj);
			jobj.Add("vel", Vector3ToFloatArraySerializer.toJson(rigidBody.velocity));
			jobj.Add("ang", Vector3ToFloatArraySerializer.toJson(rigidBody.angularVelocity));
		}

		public override void LoadData(JObject jobj)
		{
			base.LoadData(jobj);
			Vector3ToFloatArraySerializer.fromJson(jobj.GetValue("vel"), rigidBody.velocity);
			Vector3ToFloatArraySerializer.fromJson(jobj.GetValue("ang"), rigidBody.angularVelocity);
		}
	}
}
