using System.Runtime.Serialization;
using OpenTK.Mathematics;

namespace Erkunden.Core.Components
{
	[DataContract]
	public class Momentum : ECS.IComponent
	{
		[DataMember(Name = "ldrg")]
		public float LinearDrag = 0;
		[DataMember(Name = "sdrg")]
		public float ScalarDrag = 0;
		[DataMember(Name = "adrg")]
		public float AngularDrag = 0;
		[DataMember(Name = "lin")]
		public Vector3 Linear = Vector3.Zero;
		[DataMember(Name = "scl")]
		public Vector3 Scalar = Vector3.Zero;
		[DataMember(Name = "ang")]
		public Vector3 Angular = Vector3.Zero;
	}
}
