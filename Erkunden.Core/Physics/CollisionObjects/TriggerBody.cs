using Erkunden.Core.Components;
using Erkunden.Core.Physics.Colliders;

namespace Erkunden.Core.Physics.CollisionObjects
{
	public class TriggerBody : StaticBody
	{
		public TriggerBody(ICollider collider, Transform transform) : base(collider, transform) { }
	}
}
