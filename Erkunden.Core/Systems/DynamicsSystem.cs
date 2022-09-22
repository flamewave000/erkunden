using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Erkunden.Core.Physics.CollisionObjects;
using Erkunden.Core.Utils;
using Erkunden.ECS;
using OpenTK.Mathematics;

namespace Erkunden.Core.Systems
{
	public class DynamicsSystem : ISystem<GameTime>
	{
		public Vector3 GlobalGravity = Vector3.Zero;

		public void Process(Entity[] entities, GameTime gameTime)
		{
			var bodies = entities.Where(x => x.Has<RigidBody>()).Select(x => x.Get<RigidBody>()!).ToArray();
			ApplyGravity(bodies);
			MoveObject(bodies, gameTime.ellapsed);
		}

		private void ApplyGravity(RigidBody[] bodies)
		{
			// Maybe we can calulate how much gravity is being applied to an object here based on the mass of other objects?
			foreach (var body in bodies)
			{
				if (body.TakesGravity)
					body.NetForce = body.NetForce + (GlobalGravity + body.Gravity);
				else
					body.NetForce = body.NetForce + body.Gravity;
			}
		}

		private void MoveObject(RigidBody[] bodies, float time)
		{
			foreach (var rigidBody in bodies)
			{
				rigidBody.NetForce = rigidBody.Force + (-rigidBody.Velocity * rigidBody.Friction);
				rigidBody.NetTorque = rigidBody.Torque + (-rigidBody.Torque * rigidBody.Friction);

				rigidBody.Velocity += rigidBody.NetForce * rigidBody.InvMass;
				rigidBody.AngularVelocity += rigidBody.NetTorque * rigidBody.InvInertia * ((float)Math.PI / 180f);

				rigidBody.Transform.Position += rigidBody.Velocity * time;
				rigidBody.Transform.Rotation *= Quaternion.FromEulerAngles(rigidBody.AngularVelocity * time);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float Reduce(float value, float amount) =>
			value < 0 ? Math.Min(value + amount, 0) : Math.Max(value - amount, 0);
	}
}
