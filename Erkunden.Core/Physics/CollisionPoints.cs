using Erkunden.Core.Physics.CollisionObjects;
using OpenTK.Mathematics;

namespace Erkunden.Core.Physics
{
	public class Collision
	{
		/// <summary>Object A that collided with Object B</summary>
		public ICollidable ObjA;
		/// <summary>Object B that collided with Object A</summary>
		public ICollidable ObjB;
		/// <summary>Furthest point of A into B</summary>
		public Vector3 VecA;
		/// <summary>Furthest point of B into A</summary>
		public Vector3 VecB;
		/// <summary>B - A normalized</summary>
		public Vector3 Normal;
		/// <summary>Length of B - A</summary>
		public float Depth;
		/// <summary>
		/// true if A and B have collided.
		/// </summary>
		/// <remarks>
		/// This is only used if a test is manually made,
		/// otherwise non-collisions are ignored before a <see cref="Collision"/> object is made
		/// </remarks>
		public bool IsCollided;

		public Collision(ICollidable objA, ICollidable objB, Vector3 vecA, Vector3 vecB, Vector3 normal, float depth, bool isCollided)
		{
			ObjA = objA;
			ObjB = objB;
			VecA = vecA;
			VecB = vecB;
			Normal = normal;
			Depth = depth;
			IsCollided = isCollided;
		}
	}
}
