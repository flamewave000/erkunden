using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Erkunden.ECS
{
	[Serializable]
	public class Entity
	{
		public Guid ID { get; internal set; }

		#region Component Operators
		[IgnoreDataMember]
		private Dictionary<Type, Component> _components = new Dictionary<Type, Component>();
		[IgnoreDataMember]
		internal IEnumerable<Component> Components => _components.Values;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Add<T>() where T : struct, Component => (T)_components.GetValueOrDefault(typeof(T), new T());
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add<T>(out T outVal) where T : struct, Component => outVal = (T)_components.GetValueOrDefault(typeof(T), new T());
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T? Get<T>() where T : struct, Component => _components.TryGetValue(typeof(T), out var value) ? (T?)value : null;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T? Get<T>(Type type) where T : struct, Component => _components.TryGetValue(type, out var value) ? (T?)value : null;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has<T>() where T : struct, Component => _components.ContainsKey(typeof(T));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(Type type) => _components.ContainsKey(type);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(Type[] types) => types.All((type) => _components.ContainsKey(type));
		#endregion

		#region Children Management
		[IgnoreDataMember]
		private Entity? _parent = null;
		[IgnoreDataMember]
		private Dictionary<Guid, Entity> _children = new Dictionary<Guid, Entity>();
		[IgnoreDataMember]
		public int ChildCount => _children.Count;
		[IgnoreDataMember]
		public IReadOnlyDictionary<Guid, Entity> Children => _children;

		public bool AddChild(Entity child)
		{
			if (child._parent != null)
				child.RemoveFromParent();
			if (!_children.TryAdd(child.ID, child))
				return false;
			child._parent = this;
			return true;
		}
		public bool RemoveChild(Entity child)
		{
			if (!_children.Remove(child.ID, out var value))
				return false;
			value._parent = null;
			return true;
		}
		public void RemoveFromParent()
		{
			if (_parent == null) return;
			_parent.RemoveChild(this);
		}
		public Entity GetChild(Guid id) => _children[id];
		#endregion
	}
}
