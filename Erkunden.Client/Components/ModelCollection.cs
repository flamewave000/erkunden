using System.Collections.Generic;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.ECS;

namespace Erkunden.Client.Components
{
	public class ModelCollection : Dictionary<string, Model>, IComponent
	{
		public void AddModel(Model model) => Add(model.Name, model);
	}
}
