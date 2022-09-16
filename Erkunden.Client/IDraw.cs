using System;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.Graphics.Data;
using Erkunden.Core.Util;

namespace Erkunden.Client
{
	public interface IDraw : IComparable<RenderLevel>
	{
		public RenderLevel Level { get; set; }
		public bool IsVisible { get; set; }
		public void OnPreDraw(Shader shader, in GameTime gameTime);
		public void OnDraw(Shader shader, in GameTime gameTime);
		public void OnPostDraw(Shader shader, in GameTime gameTime);
	}
}
