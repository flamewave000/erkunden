using System;
using System.Runtime.Serialization;
using OpenTK.Mathematics;

namespace Erkunden.Core.Components
{
	public class Momentum : ECS.IComponent
	{
		public float LinearDrag = 0;
		public float ScalarDrag = 0;
		public float AngularDrag = 0;
		public Vector3 LinearAccel = Vector3.Zero;
		public Vector3 ScalarAccel = Vector3.Zero;
		public Vector3 AngularAccel = Vector3.Zero;
		public Vector3 Linear = Vector3.Zero;
		public Vector3 Scalar = Vector3.Zero;
		public Vector3 Angular = Vector3.Zero;

		public void Reset(
			bool linearDrag = true,
			bool scalarDrag = true,
			bool angularDrag = true,
			bool linearAccel = true,
			bool scalarAccel = true,
			bool angularAccel = true,
			bool linear = true,
			bool scalar = true,
			bool angular = true
		)
		{
			if (linearDrag) LinearDrag = 0;
			if (scalarDrag) ScalarDrag = 0;
			if (angularDrag) AngularDrag = 0;
			if (linearAccel) LinearAccel = Vector3.Zero;
			if (scalarAccel) ScalarAccel = Vector3.Zero;
			if (angularAccel) AngularAccel = Vector3.Zero;
			if (linear) Linear = Vector3.Zero;
			if (scalar) Scalar = Vector3.Zero;
			if (angular) Angular = Vector3.Zero;
		}
	}
}
