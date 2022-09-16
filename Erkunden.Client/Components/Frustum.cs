using System;
using Erkunden.Client.AssetManagement.Shaders;
using OpenTK.Mathematics;

namespace Erkunden.Client.Components
{
	public enum ProjectionType
	{
		Orthographic = 0,
		Perspective = 1
	}

	public class Frustum : ECS.Component
	{
		public Matrix4 Projection = Matrix4.Identity;
		public ProjectionType Type = ProjectionType.Perspective;
		public float Near = 0.1f;
		public float Far = 1000f;
		/// <summary>
		/// Field of View in degrees. Ignored if <see cref="Type"/> is equal to <see cref="ProjectionType.Orthographic"/>.
		/// </summary>
		public float FieldOfView = 70f;
		public float FieldOfViewRadians
		{
			get => MathHelper.DegreesToRadians(FieldOfView);
			set => FieldOfView = MathHelper.RadiansToDegrees(value);
		}

		public void ConfigurePerspective(float near, float far, float fieldOfView)
		{
			Near = near;
			Far = far;
			FieldOfView = fieldOfView;
			Type = ProjectionType.Perspective;
		}
		public void ConfigureOrthographic(float near, float far)
		{
			Near = near;
			Far = far;
			Type = ProjectionType.Orthographic;
		}

		public void BindProjection(Vector2 size, Shader shader)
		{
			if (Type == ProjectionType.Perspective)
				Matrix4.CreatePerspectiveFieldOfView(FieldOfViewRadians, Math.Max(size.X, size.Y) / Math.Min(size.X, size.Y), Near, Far, out Projection);
			else
				Matrix4.CreateOrthographic(size.X, size.Y, Near, Far, out Projection);
			shader.SetProjection(ref Projection);
		}
	}
}
