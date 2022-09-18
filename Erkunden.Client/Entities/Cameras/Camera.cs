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

	public abstract class Camera : ECS.IComponent, ICamera
	{
		protected Matrix4 view;
		protected Vector3 position = Vector3.Zero;
		protected Vector3 lookAt = -Vector3.UnitZ;

		public ref Vector3 Position => ref position;
		public ref Vector3 LookAt => ref lookAt;
		public ref Matrix4 ViewMatrix => ref view;

		protected Camera() { }

		public void BindView(Shader shader)
		{
			GenerateView(out view);
			shader.SetView(ref view);
		}

		protected abstract void GenerateView(out Matrix4 view);
	}
}
