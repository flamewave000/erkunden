using System;
using System.Collections.ObjectModel;

namespace Erkunden.ECS
{
	internal class ComponentCollection : KeyedCollection<Type, IComponent>
	{
		protected override Type GetKeyForItem(IComponent item) => item.GetType();
	}
}
