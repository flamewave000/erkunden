using OpenTK.Mathematics;

namespace Erkunden.Core.Physics.Colliders
{
	public class SphereCollider : ICollider
	{
		public Vector3 Center = Vector3.Zero;
		public float Radius = 1;

		public SphereCollider() { }
		public SphereCollider(float radius) { Radius = radius; }
		public SphereCollider(float radius, Vector3 center) { Center = center; Radius = radius; }
	}
}
