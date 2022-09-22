using OpenTK.Mathematics;

namespace Erkunden.Core.Physics.Colliders
{
	public class PlaneCollider : ICollider
	{
		public Vector3 Plane = Vector3.UnitY;
		public float Distance = 0;

		public PlaneCollider() { }
		public PlaneCollider(float distance) { Distance = distance; }
		public PlaneCollider(float distance, Vector3 plane) { Plane = plane; Distance = distance; }
	}
}
