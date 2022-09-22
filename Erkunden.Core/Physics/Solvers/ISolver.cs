using System.Collections.Generic;
using Erkunden.Core.Utils;

namespace Erkunden.Core.Physics.Solvers
{
	public interface ISolver
	{
		public void Solve(List<Collision> collisions, GameTime gameTime);
	}
}
