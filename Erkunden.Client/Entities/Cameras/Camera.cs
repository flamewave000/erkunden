using Erkunden.Client.AssetManagement.Shaders;
using OpenTK.Mathematics;

namespace Erkunden.Client.Entities.Cameras
{
	public interface ICamera
	{
		Vector3 UpVec { get; }
		Vector3 Position { get; }
		Matrix4 ViewMatrix { get; }
		void BindView(Shader shader);
	}

	public abstract class Camera : ECS.Component, ICamera
	{
		protected Matrix4 viewMat4;
		protected Vector3 upVec = Vector3.UnitY;
		protected Vector3 position = Vector3.Zero;

		public Vector3 UpVec { get => upVec; set => upVec = value; }
		public Vector3 Position { get => position; set => position = value; }
		public Matrix4 ViewMatrix => viewMat4;

		protected Camera() { }

		public void BindView(Shader shader)
		{
			GenerateView(out viewMat4);
			shader.SetView(ref viewMat4);
		}

		protected abstract void GenerateView(out Matrix4 view);
	}
}
