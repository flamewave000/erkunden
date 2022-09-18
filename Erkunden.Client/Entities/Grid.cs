using Erkunden.Client.AssetManagement;
using Erkunden.Client.AssetManagement.Models;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Core.Util;
using OpenTK.Mathematics;

namespace Erkunden.Client.Entities
{
	public class Grid : ClientGameObject
	{

		public Model Plane = null!;

		public Vector3[][] Positions = new Vector3[10][];

		protected override void OnSetup()
		{
			base.OnSetup();
			Level = Graphics.Data.RenderLevel.WireFrame;
			Plane = AssetProvider.Get<Model>("Plane");

			Transform.Position = Vector3.Zero;

			for (int x = 0, y; x < 10; x++)
			{
				Positions[x] = new Vector3[10];
				for (y = 0; y < 10; y++)
				{
					Positions[x][y] = new Vector3(x * 10, 0, y * 10);
				}
			}
		}

		public override void OnDraw(Shader shader, in GameTime gameTime)
		{
			base.OnDraw(shader, gameTime);
			shader.SetBool("u_UseVertexColour", true);
			shader.SetFloat("u_VertexScalar", 1f / 100f);
			Matrix4 model = Matrix4.Identity;
			Vector3 offset = new Vector3(-50, 0, -50);
			for (int c = 0; c < 100; c++)
			{
				Matrix4.CreateTranslation(Positions[c % 10][c / 10] + offset, out model);
				Matrix4.Mult(in model, in Transform.Matrix, out model);
				shader.SetModel(ref model);
				Plane.Draw(shader);
			}
			shader.SetBool("u_UseVertexColour", false);
		}
	}
}
