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
				view = Matrix4.LookAt(RelativePosition / Zoom, LookAtOffset, upVec);
				return;
			}
			Vector4 eye = new Vector4(RelativePosition / Zoom, 1);
			Vector4 up = new Vector4(UpVec, 1);
			Vector4 ahead = new Vector4(LookAtOffset, 1);

			// Transform the eye, up, and ahead vectors to the Target's space
			Vector4.TransformRow(in eye, target.Matrix.ClearScale(), out eye);
			Vector4.Transform(in up, target.Rotation, out up);
			Vector4.TransformRow(in ahead, target.Matrix.ClearScale(), out ahead);

			// Set our position to the one calculated for eye
			Position = eye.Xyz;

			view = Matrix4.LookAt(Position, ahead.Xyz, up.Xyz);
		}
	}
}
