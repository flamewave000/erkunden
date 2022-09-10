using System;

namespace Erkunden.ECS
{
	public interface System
	{
		public Type Component { get; }
		public void Process(Entity entity);
	}
}
