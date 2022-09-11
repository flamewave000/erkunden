using System;
using Erkunden.Core.Components;
using Erkunden.Core.Util;
using Erkunden.ECS;

namespace Erkunden.Core.Entities
{
	public class GameObject : Entity, IDisposable
	{
		public Transform Transform;

		public GameObject()
		{
			Add(out Transform);
		}

		public virtual void Setup() { }
		public virtual void Update(GameTime gameTime) { }
		public virtual void Dispose() { }
	}
}
