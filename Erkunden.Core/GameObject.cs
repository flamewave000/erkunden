using Erkunden.Core.Components;
using Erkunden.Core.Utils;
using Erkunden.ECS;

namespace Erkunden.Core
{
	public abstract class GameObject : Entity, IUpdate
	{
		public readonly Transform Transform = new Transform();
		public GameObject() { Transform.owner = this; }
		protected override void OnSetup() { }
		public virtual void OnPreUpdate(in GameTime gameTime) { }
		public virtual void OnUpdate(in GameTime gameTime) { }
		public virtual void OnPostUpdate(in GameTime gameTime) { Transform.UpdateMatrix(); }
	}
}
