using Erkunden.Client.AssetManagement;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Core.Utils;
using OpenTK.Mathematics;

namespace Erkunden.Client.Entities
{
	public class Grid : ClientGameObject
	{

		public Model Plane = null!;

		public Vector3[][] Positions = null!;

		public uint Mult { get; private set; } = 10;

		public Grid() { }
		public Grid(uint mult) { Mult = mult; }

		protected override void OnSetup()
		{
			base.OnSetup();
			Level = Graphics.Data.RenderLevel.WireFrame;
			Plane = AssetProvider.Get<Model>("Plane");

			Transform.Position = Vector3.Zero;

			Positions = new Vector3[Mult][];
			for (int x = 0, y; x < Mult; x++)
			{
				Positions[x] = new Vector3[10];
				for (y = 0; y < Mult; y++)
				{
					Positions[x][y] = new Vector3(x * Mult, 0, y * Mult);
				}
			}
		}

		public override void OnDraw(Shader shader, in GameTime gameTime)
		{
			base.OnDraw(shader, gameTime);
			shader.SetBool("u_UseVertexColour", true);
			shader.SetFloat("u_VertexScalar", 1f / (Mult * Mult));
			Matrix4 model = Matrix4.Identity;
			Vector3 offset = new Vector3(-(Mult * Mult * 0.5f), 0, -(Mult * Mult * 0.5f));
			for (int c = 0; c < Mult * Mult; c++)
			{
				Matrix4.CreateTranslation(Positions[c % Mult][c / Mult] + offset, out model);
				Matrix4.Mult(in model, in Transform.ModelMatrix, out model);
				shader.SetModel(ref model);
				Plane.Draw(shader);
			}
			shader.SetBool("u_UseVertexColour", false);
		}
	}
}
