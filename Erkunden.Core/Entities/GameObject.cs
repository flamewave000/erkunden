using Erkunden.Core.Components.Physics;
using Erkunden.Core.Util;
using Erkunden.ECS;

namespace Erkunden.Core.Entities
{
	public class GameObject : Entity
	{
		public Transform Transform;

		public GameObject()
		{
			Add(out Transform);
		}

		public virtual void Setup() { }
		public virtual void Update(GameTime gameTime) { }
	}
}
