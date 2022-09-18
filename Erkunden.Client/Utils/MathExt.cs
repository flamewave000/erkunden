using OpenTK.Mathematics;

namespace Erkunden.Client.Utils
{
	public struct Orientation
	{
		public Vector3 Forward;
		public Vector3 Right;
		public Vector3 Up;
		public Matrix4 LookAt;
	}

	public static class MathExt
	{
		public static Orientation CalculateOrientation(Quaternion rotation)
		{
			Orientation orient = new Orientation();
			orient.Forward = Vector3.Transform(Vector3.UnitZ, rotation).Normalized();
			orient.Right = Vector3.Transform(Vector3.UnitX, rotation).Normalized();
			orient.Up = Vector3.Transform(Vector3.UnitY, rotation).Normalized();
			orient.LookAt = new Matrix4(
				orient.Right.X, orient.Right.Y, orient.Right.Z, 0f,
				orient.Up.X, orient.Up.Y, orient.Up.Z, 0f,
				orient.Forward.X, orient.Forward.Y, orient.Forward.Z, 0f,
				0f, 0f, 0f, 1f
			);
			return orient;
		}
	}
}
