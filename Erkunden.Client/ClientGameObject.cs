using System.Diagnostics.CodeAnalysis;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.Graphics.Data;
using Erkunden.Core;
using Erkunden.Core.Util;

namespace Erkunden.Client
{
	public class ClientGameObject : GameObject, IDraw
	{
		public bool IsVisible { get; set; } = true;
		public RenderLevel Level { get; set; } = RenderLevel.Default;// | RenderLevel.WireFrame;

		public virtual void BindMatrix(Shader shader)
		{
			shader.SetModel(ref Transform.ModelMatrix);
			shader.SetModelNormal(ref Transform.ModelNormalMatrix);
		}
		public int CompareTo([AllowNull] RenderLevel other) => ((int)Level).CompareTo((int)other);

		public virtual void OnDraw(Shader shader, in GameTime gameTime) { BindMatrix(shader); }
		public virtual void OnPreDraw(Shader shader, in GameTime gameTime) { }
		public virtual void OnPostDraw(Shader shader, in GameTime gameTime) { }
	}
}
