using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Erkunden.ECS
{
	public abstract class Entity : IEquatable<Entity>
	{
		#region Fields
		private bool UnSafe = true;
		private Dictionary<EntityID, Entity> _children = new Dictionary<EntityID, Entity>();
		private KeyedByTypeCollection<Component> _components = new KeyedByTypeCollection<Component>();

		public EntityID ID { get; internal set; }
		public EntityID? ParentID { get; private set; }
		#endregion

		#region Properties
		public bool IsDestroyed => ID == 0UL;
		public IEnumerable<Component> Components => _components;

		public int ChildCount => _children.Count;
		public readonly EntityCollection Children;
		public Entity? Parent => ParentID.HasValue ? EntityFactory.TryGet(ParentID.Value) : null;
		#endregion

		#region Constructors
		public Entity()
		{
			Children = new EntityCollection(_children.Values);
		}
		#endregion

		internal void Setup()
		{
			UnSafe = false;
			OnSetup();
		}
		internal void Destroy()
		{
			RemoveFromParent();
			_components.Clear();
			ID = (EntityID)0;
			OnDestroy();
		}
		protected abstract void OnSetup();
		protected virtual void OnDestroy() { }

		#region Component Operators
		public T Add<T>() where T : Component, new()
		{
			if (UnSafe) throw new InvalidOperationException("Components must be added in the Setup() call");
			if (_components.Contains(typeof(T)))
				return Get<T>()!;
			var element = new T();
			_components.Add(element);
			return element;
		}
		public void Add<T>(out T outVal) where T : Component, new()
		{
			if (UnSafe) throw new InvalidOperationException("Components must be added in the Setup() call");
			Component component;
			if (!_components.TryGetValue(typeof(T), out component))
				component = new T();
			outVal = (T)component;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T? Get<T>() where T : Component => Get<T>(typeof(T));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T? Get<T>(Type type) where T : Component => _components.TryGetValue(type, out var value) ? (T?)value : null;
		public bool TryGet(Type type, out Component value) => _components.TryGetValue(type, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has<T>() where T : Component => Has(typeof(T));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(Type type) => _components.Contains(type);
		public bool Has(Type[] types)
		{
			foreach (Type type in types)
				if (!_components.Contains(type)) return false;
			return true;
		}
		#endregion

		#region Parent-Children Management


		public EntityCollection GetAllChildren() => new EntityCollection(InternalGetAllChildren());
		private List<Entity> InternalGetAllChildren()
		{
			List<Entity> entities = new List<Entity>(_children.Values);
			foreach (var entity in _children.Values)
				entities.AddRange(entity.InternalGetAllChildren());
			return entities;
		}

		public T CreateChild<T>() where T : Entity, new()
		{
			T child = new T();
			EntityFactory.Register(child, true);
			AddChild(child);
			child.Setup();
			return child;
		}
		public T CreateChild<T>(object arg0, params object[] arguments) where T : Entity
		{
			var args = arguments.Prepend(arg0);
			T child = (T)typeof(T).GetConstructor(args.Select(x => x.GetType()).ToArray()).Invoke(args.ToArray());
			EntityFactory.Register(child, true);
			AddChild(child);
			child.Setup();
			return child;
		}

		public bool HasChild(EntityID childID) => _children.ContainsKey(childID);
		public bool HasChild(Entity child) => HasChild(child.ID);

		public bool AddChild(EntityID childID) => AddChild(EntityFactory.Get(childID));
		public bool AddChild(Entity child)
		{
			if (child.ParentID != null)
				child.RemoveFromParent();
			if (!_children.TryAdd(child.ID, child))
				return false;
			child.ParentID = ID;
			return true;
		}

		public bool RemoveChild(EntityID childID)
		{
			if (!_children.Remove(childID))
				return false;
			var child = EntityFactory.TryGet(childID);
			if (child != null) child!.ParentID = null;
			return true;
		}
		public bool RemoveChild(Entity child)
		{
			if (!_children.Remove(child.ID))
				return false;
			child.ParentID = null;
			return true;
		}

		public T GetParent<T>() where T : Entity => Parent is T ? (T)Parent : Parent!.GetParent<T>();
		public void RemoveFromParent()
		{
			var parent = Parent;
			if (parent == null) return;
			parent.RemoveChild(this);
			ParentID = null;
		}

		public bool Equals(Entity other) => ReferenceEquals(this, other);
		#endregion
	}
}
