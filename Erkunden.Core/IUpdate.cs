using Erkunden.Core.Utils;

namespace Erkunden.Core
{
	public interface IUpdate
	{
		public void OnPreUpdate(in GameTime gameTime);
		public void OnUpdate(in GameTime gameTime);
		public void OnPostUpdate(in GameTime gameTime);
	}
}
