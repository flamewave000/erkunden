using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.Components;
using Erkunden.Client.Entities.Cameras;
using Erkunden.Core.Util;
using OpenTK.Windowing.Common;

namespace Erkunden.Client.Entities.Scenes
{
	public class Scene : ClientGameObject
	{
		protected DirectionCamera DefaultCamera = null!;
		public Camera CurrentOrDefaultCamera => CurrentCamera ?? DefaultCamera;
		public Camera? CurrentCamera { get; set; } = null;
		public ViewPort ViewPort { get; private set; } = null!;
		public Frustum Frustum { get; private set; } = null!;
		public Game Game;

		public Scene(Game game)
		{
			Game = game;
			game.Resize += OnResize;
		}

		public override void OnPreDraw(Shader shader, in GameTime gameTime)
		{
			// Bind the View Port for drawing
			ViewPort.Bind(Game.ClientSize);
			// Bind the Projection Matrix
			Frustum.BindProjection(ViewPort.SizeF, shader);
			// Bind the current camera's View Matrix
			(CurrentCamera ?? DefaultCamera).BindView(shader);
		}

		protected override void OnSetup()
		{
			// Initialize Default Camera Component
			DefaultCamera = Add<DirectionCamera>();
			CurrentCamera = DefaultCamera;

			// Initialize View Port component
			ViewPort = Add<ViewPort>();
			ViewPort.AbsoluteX = 0;
			ViewPort.AbsoluteY = 0;
			ViewPort.AbsoluteWidth = Game.ClientSize.X;
			ViewPort.AbsoluteHeight = Game.ClientSize.Y;

			// Initialize Projection Frustum
			Frustum = Add<Frustum>();
			Frustum.ConfigurePerspective(0.1f, 1000f, 60f);
		}

		public virtual void OnResize(ResizeEventArgs eventArgs)
		{
			ViewPort.AbsoluteX = 0;
			ViewPort.AbsoluteY = 0;
			ViewPort.AbsoluteWidth = eventArgs.Width;
			ViewPort.AbsoluteHeight = eventArgs.Height;
		}
	}
}
