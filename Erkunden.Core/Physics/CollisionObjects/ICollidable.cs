using System;
using Erkunden.Core.Components;
using Erkunden.Core.Physics.Colliders;
using Erkunden.Core.Utils;
using OpenTK.Mathematics;

namespace Erkunden.Core.Physics.CollisionObjects
{
	public interface ICollidable : ECS.IComponent
	{
		public const double GravitationalConstant = 6.674E-11;

		public ICollider Collider { get; }
		public Transform Transform { get; }
		public float Mass { get; set; }
		public float InvMass { get; }
		public event Action<Collision, GameTime> OnCollision;
		public void Collided(in Collision collision, in GameTime gameTime);

		/// <summary>
		/// 
		/// </summary>
		/// <seealso cref="https://www.omnicalculator.com/physics/gravitational-force"/>
		/// <param name="objA"></param>
		/// <param name="objB"></param>
		/// <returns></returns>
		public static Vector3 CalcGravitationalAttraction(ICollidable objA, ICollidable objB)
		{
			if (objA.Mass == 0 || objB.Mass == 0) return Vector3.Zero;

			Vector3 vector = objB.Transform.Position - objA.Transform.Position;
			double distance = vector.LengthFast;
			double force = (GravitationalConstant * objA.Mass * objB.Mass) / (distance * distance);
			if (force < 1) return Vector3.Zero;
			return (vector / (float)distance) * (float)force;
		}
	}
}
