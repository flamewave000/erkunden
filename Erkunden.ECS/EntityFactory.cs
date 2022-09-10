using System;
using System.Collections.Generic;
using System.Text;

namespace Erkunden.ECS
{
	public static class EntityFactory
	{
		public static void Save(Entity entity)
		{

		}

		public static TEntity Load<TEntity>(Guid? id = null) where TEntity : Entity, new()
		{
			TEntity entity = new TEntity();
			entity.ID = Guid.NewGuid();
			return entity;
		}
	}
}
