using System.Diagnostics.CodeAnalysis;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.Entities.Scenes;
using Erkunden.Client.Graphics.Data;
using Erkunden.Core;
using Erkunden.Core.Util;

namespace Erkunden.Client
{
	public class ClientGameObject : GameObject, IDraw
	{
		private Scene? ParentScene = null;
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

		protected Scene GetParentScene()
		{
			var parent = Parent ?? throw new System.Exception("ClientGameObject is not a child of a scene");
			if (ParentScene != null) return ParentScene;
			if (parent is Scene)
			{
				ParentScene = (Scene)parent;
				return ParentScene;
			}
			if (!(parent is ClientGameObject))
				throw new System.InvalidOperationException("Parent is not a ClientGameObject");
			return ((ClientGameObject)parent).GetParentScene();
		}
	}
}
