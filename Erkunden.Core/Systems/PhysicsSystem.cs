using System;
using System.Runtime.CompilerServices;
using Erkunden.Core.Components;
using Erkunden.Core.Util;
using Erkunden.ECS;
using OpenTK.Mathematics;

namespace Erkunden.Core.Systems
{
	public class PhysicsSystem : System<GameTime>
	{
		private Type MomentumType = typeof(Momentum);

		public void Process(Entity entity, GameTime gameTime)
		{
			var gameObject = entity as GameObject;
			if (gameObject == null) return;
			var momentum = gameObject.Get<Momentum>(MomentumType);
			if (momentum == null) return;

			// Reduce momentum based on drag values
			ReduceToZero(ref momentum.Linear, momentum.LinearDrag * gameTime.ellapsed);
			ReduceToZero(ref momentum.Scalar, momentum.ScalarDrag * gameTime.ellapsed);
			ReduceToZero(ref momentum.Angular, momentum.AngularDrag * gameTime.ellapsed);

			Vector3.Add(in momentum.Linear, momentum.LinearAccel * gameTime.ellapsed, out momentum.Linear);
			Vector3.Add(in momentum.Scalar, momentum.ScalarAccel * gameTime.ellapsed, out momentum.Scalar);
			Vector3.Add(in momentum.Angular, momentum.AngularAccel * gameTime.ellapsed, out momentum.Angular);

			gameObject.Transform.Position += momentum.Linear * gameTime.ellapsed;
			gameObject.Transform.Scale += momentum.Scalar * gameTime.ellapsed;
			gameObject.Transform.Rotation *= Quaternion.FromEulerAngles(momentum.Angular * gameTime.ellapsed);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float ReduceToZero(in float left, in float right) => left < 0 ? Math.Min(0, left + right) : Math.Max(0, left - right);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ReduceToZero(ref Vector3 vector, in float drag)
		{
			vector.X = ReduceToZero(vector.X, drag);
			vector.Y = ReduceToZero(vector.Y, drag);
			vector.Z = ReduceToZero(vector.Z, drag);
		}
	}
}
