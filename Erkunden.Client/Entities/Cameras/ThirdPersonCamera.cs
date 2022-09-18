using Erkunden.Core.Components;
using OpenTK.Mathematics;

namespace Erkunden.Client.Entities.Cameras
{
	public class ThirdPersonCamera : Camera
	{
		public Transform? Target = null;

		public float Zoom = 1.0f;
		public Vector3 RelativePosition = -Vector3.UnitZ;
		public Vector3 LookAtOffset = Vector3.Zero;

		public static Vector3 RelativePositionFromRadiusRotation(float radius, in Quaternion rotation)
		{
			Vector4 forward = Vector4.UnitZ;
			Vector4.Transform(in forward, in rotation, out forward);
			return forward.Xyz * radius;
		}

		protected override void GenerateView(out Matrix4 view)
		{
			var target = Target;
			if (target == null)
			{
				view = Matrix4.LookAt(RelativePosition / Zoom, LookAtOffset, Vector3.UnitY);
				return;
			}
			Matrix4 targetMatrix = Matrix4.Identity;
			target.GenerateMatrix(target.Type, true, true, ref targetMatrix);
			targetMatrix = targetMatrix.ClearScale();

			// Transform the eye, and ahead vectors to the Target's space
			Position = (new Vector4(RelativePosition / Zoom, 1) * targetMatrix).Xyz;
			LookAt = (new Vector4(LookAtOffset, 1) * targetMatrix).Xyz;
			view = Matrix4.LookAt(Position, LookAt, (Vector4.UnitY * targetMatrix).Xyz);
		}
	}
}
