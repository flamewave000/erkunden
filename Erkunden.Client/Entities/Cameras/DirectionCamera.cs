using OpenTK.Mathematics;

namespace Erkunden.Client.Entities.Cameras
{
	public class DirectionCamera : Camera
	{
		public Vector3 Direction = -Vector3.UnitZ;

		protected override void GenerateView(out Matrix4 view)
		{
			view = Matrix4.LookAt(Position, Position + Direction.Normalized(), Vector3.UnitY);
		}
	}
}
