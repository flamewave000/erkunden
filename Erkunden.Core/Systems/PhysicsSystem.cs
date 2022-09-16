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

			var time = (float)gameTime.ellapsed;

			// Reduce momentum based on drag values
			if (momentum.LinearDrag != 0)
				ReduceToZero(ref momentum.Linear, momentum.LinearDrag * time);
			if (momentum.ScalarDrag != 0)
				ReduceToZero(ref momentum.Linear, momentum.ScalarDrag * time);
			if (momentum.AngularDrag != 0)
				ReduceToZero(ref momentum.Linear, momentum.AngularDrag * time);

			if (momentum.Linear != Vector3.Zero)
				gameObject.Transform.Position += momentum.Linear * time;
			if (momentum.Scalar != Vector3.Zero)
				gameObject.Transform.Scale += momentum.Scalar * time;
			if (momentum.Angular != Vector3.Zero)
				gameObject.Transform.Rotation *= Quaternion.FromEulerAngles(momentum.Angular * time);
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
