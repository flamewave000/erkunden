using Erkunden.Client.AssetManagement.Shaders;
using OpenTK.Mathematics;

namespace Erkunden.Client.Entities.Cameras
{
	public interface ICamera
	{
		ref Vector3 Position { get; }
		ref Vector3 LookAt { get; }
		ref Matrix4 ViewMatrix { get; }
		void BindView(Shader shader);
	}

	public class Camera : ECS.IComponent, ICamera
	{
		protected Matrix4 view;
		protected Vector3 position = Vector3.UnitZ;
		protected Vector3 lookAt = Vector3.Zero;

		public ref Vector3 Position => ref position;
		public ref Vector3 LookAt => ref lookAt;
		public ref Matrix4 ViewMatrix => ref view;

		public void BindView(Shader shader)
		{
			GenerateView(out view);
			shader.SetView(ref view);
			shader.SetVector3("u_EyePos", position);
		}

		protected virtual void GenerateView(out Matrix4 view)
		{
			view = Matrix4.LookAt(position, lookAt, Vector3.UnitY);
		}
	}
}
