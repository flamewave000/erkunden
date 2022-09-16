using System.Collections;
using System.Collections.Generic;

namespace Erkunden.ECS
{
	internal class EntityEnumerator<T> : IEnumerator<T> where T : Entity
	{
		IEnumerator<Entity> enumerator;
		public T Current => (T)enumerator.Current;
		object IEnumerator.Current => enumerator.Current;
		internal EntityEnumerator(IEnumerator<Entity> enumerator) { this.enumerator = enumerator; }
		public void Dispose() => enumerator.Dispose();
		public bool MoveNext()
		{
			// Iterate through the enumerator until we find an element of type T
			bool moveNext;
			while ((moveNext = enumerator.MoveNext()) && !(enumerator.Current is T)) { }
			return moveNext;
		}
		public void Reset() => enumerator.Reset();
	}

	public class EntityCollection<T> : IEnumerable<T> where T : Entity
	{
		protected IEnumerable<Entity> entities;
		public EntityCollection(IEnumerable<Entity> entities) { this.entities = entities; }
		public IEnumerator<T> GetEnumerator() => new EntityEnumerator<T>(entities.GetEnumerator());
		IEnumerator IEnumerable.GetEnumerator() => new EntityEnumerator<T>(entities.GetEnumerator());
		public EntityCollection<TEntity> FilterAsType<TEntity>() where TEntity : Entity => new EntityCollection<TEntity>(entities);
	}

	public class EntityCollection : EntityCollection<Entity>
	{
		public EntityCollection(IEnumerable<Entity> entities) : base(entities) { }
	}
}
