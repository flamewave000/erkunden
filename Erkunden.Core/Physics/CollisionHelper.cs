using System;
using Erkunden.Core.Physics.Colliders;
using Erkunden.Core.Physics.CollisionObjects;

namespace Erkunden.Core.Physics
{
	public static class CollisionHelper
	{
		private static bool TestSphereToSphere(ICollidable collidableA, ICollidable collidableB, out Collision points)
		{
			var colA = (SphereCollider)collidableA.Collider;
			var colB = (SphereCollider)collidableB.Collider;

			var vector = collidableB.Transform.Position + colA.Center - (collidableA.Transform.Position + colA.Center);
			var distance = vector.LengthSquared;
			var fullRadius = colA.Radius + colB.Radius;

			// If there is no collision, return immediately
			if (distance > fullRadius * fullRadius)
			{
				points = null!;
				return false;
			}

			// Calculate the non-squared distance
			distance = (float)Math.Sqrt(distance);
			// Normalize the A-B vector
			vector /= distance;
			// Create the collision result
			points = new Collision(
				objA: collidableA,
				objB: collidableB,
				vecA: distance * vector,
				vecB: distance * -vector,
				normal: vector,
				depth: distance,
				isCollided: true);
			return true;
		}
		private static bool TestSphereToPlane(ICollidable collidableA, ICollidable collidableB, out Collision points)
		{

			//float distance = (A * x0 + B * y0 + C * z0 + D) / Math.Sqrt(A * A + B * B + C * C);

			points = null!;
			return false;
		}

		internal static bool TestCollision(ICollidable collidableA, ICollidable collidableB, out Collision points)
		{
			if (collidableA.Collider is SphereCollider)
			{
				if (collidableB.Collider is SphereCollider)
					return TestSphereToSphere(collidableA, collidableB, out points);
				else if (collidableB is PlaneCollider)
					return TestSphereToPlane(collidableA, collidableB, out points);
				else throw new InvalidOperationException();
			}
			else if (collidableA is PlaneCollider)
			{
				if (collidableB.Collider is SphereCollider)
					return TestSphereToSphere(collidableA, collidableB, out points);
				else if (collidableB is PlaneCollider)
				{
					points = null!;
					return false;
				}
				else throw new InvalidOperationException();
			}
			else throw new InvalidOperationException();
		}
	}
}
