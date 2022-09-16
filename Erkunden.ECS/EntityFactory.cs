using System;
using System.Collections.Generic;
using System.Linq;

namespace Erkunden.ECS
{
	public static class EntityFactory
	{
		private static ulong currentID = 0;
		private static Dictionary<EntityID, Entity> entities = new Dictionary<EntityID, Entity>();

		public static event Action<Entity> EntityCreated = null!;
		public static event Action<Entity> EntityDestroyed = null!;

		public static EntityCollection Entities => new EntityCollection(entities.Values);

		public static TEntity Create<TEntity>(object arg0, params object[] arguments) where TEntity : Entity
		{
			var args = arguments.Prepend(arg0);
			return Register((TEntity)typeof(TEntity).GetConstructor(args.Select(x => x.GetType()).ToArray()).Invoke(args.ToArray()));
		}

		public static TEntity Create<TEntity>() where TEntity : Entity, new() => Register(new TEntity());
		public static TEntity Register<TEntity>(TEntity entity, bool suppressSetupCall = false) where TEntity : Entity
		{
			entity.ID = (EntityID)(++currentID);
			entities.Add(entity.ID, entity);
			if (!suppressSetupCall) entity.Setup();
			EntityCreated?.Invoke(entity);
			return entity;
		}

		public static void DestroyAll() { foreach (var entity in entities) Destroy(entity.Value); }
		public static void Destroy(EntityID entity) => Destroy(Get(entity));
		public static void Destroy(Entity entity)
		{
			entities.Remove(entity.ID);
			entity.Destroy();
			EntityDestroyed?.Invoke(entity);
		}

		public static void Save() { }
		public static void Load()
		{
			currentID = 0;
			entities.Clear();
			// Deserialize all entities
			// Detect highest ID value
			// Invoke Setup on each entity
		}

		public static Entity Get(EntityID id) => entities[id];
		public static Entity? TryGet(EntityID id) => id.IsInvalid ? null : (entities.TryGetValue(id, out var entity) ? entity : null);
		public static TEntity Get<TEntity>(EntityID id) where TEntity : Entity => (TEntity)entities[id];
		public static TEntity? TryGet<TEntity>(EntityID id) where TEntity : Entity => (TEntity?)TryGet(id);
	}
}
