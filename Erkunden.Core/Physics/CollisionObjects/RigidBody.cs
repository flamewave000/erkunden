using Erkunden.Core.Components;
using Erkunden.Core.Physics.Colliders;
using OpenTK.Mathematics;

namespace Erkunden.Core.Physics.CollisionObjects
{
	public class RigidBody : StaticBody
	{
		private float inertia = 0;
		public float Inertia
		{
			get => inertia;
			set
			{
				inertia = value;
				InvInertia = 1 / inertia;
			}
		}
		public float InvInertia { get; private set; } = 0f;
		public Vector3 Force = Vector3.Zero;
		public Vector3 Velocity = Vector3.Zero;
		public Vector3 Torque = Vector3.Zero;
		public Vector3 AngularVelocity = Vector3.Zero;

		public Vector3 NetForce { get; internal set; } = Vector3.Zero;
		public Vector3 NetTorque { get; internal set; } = Vector3.Zero;

		/// <summary>Gravitational Acceleration</summary>
		public Vector3 Gravity = Vector3.Zero;
		/// <summary>If the Rigid Body will take gravity from the world</summary>
		public bool TakesGravity = false;

		/// <summary>Friction coefficient</summary>
		public float Friction = 0.5f;
		/// <summary>Elasticity of collisions (bounciness)</summary>
		public float Restitution = 0.5f;

		public RigidBody(ICollider collider, Transform transform) : base(collider, transform) { }
	}
}
