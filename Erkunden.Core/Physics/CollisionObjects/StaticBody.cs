using System;
using Erkunden.Core.Components;
using Erkunden.Core.Physics.Colliders;
using Erkunden.Core.Utils;

namespace Erkunden.Core.Physics.CollisionObjects
{
	public class StaticBody : ICollidable
	{
		private float mass = 0;
		public ICollider Collider { get; }
		public Transform Transform { get; }
		public float Mass
		{
			get => mass;
			set
			{
				mass = value;
				InvMass = 1 / value;
			}
		}
		public float InvMass { get; private set; }

		public event Action<Collision, GameTime> OnCollision = null!;

		public StaticBody(ICollider collider, Transform transform)
		{
			Collider = collider;
			Transform = transform;
		}
		public void Collided(in Collision collision, in GameTime gameTime) => OnCollision?.Invoke(collision, gameTime);
	}
}
