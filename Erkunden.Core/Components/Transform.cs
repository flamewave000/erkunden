using System.Runtime.Serialization;
using OpenTK.Mathematics;

namespace Erkunden.Core.Components
{
	public enum TransformType
	{
		Absolute,
		Relative
	}
	[DataContract]
	public class Transform
	{
		private Matrix4 mat4 = Matrix4.Identity;
		private Vector3 sca = Vector3.One;
		private Vector3 rad = Vector3.Zero;
		private Quaternion orb = Quaternion.Identity;
		private Quaternion rot = Quaternion.Identity;
		private Vector3 pos = Vector3.Zero;
		private TransformType typ = TransformType.Relative;


		[IgnoreDataMember]
		internal GameObject owner;
		[IgnoreDataMember]
		public bool IsDirty { get; private set; } = true;

		public TransformType Type
		{
			get => typ;
			set
			{
				typ = value;
				IsDirty = true;
			}
		}

		[IgnoreDataMember]
		public ref Matrix4 Matrix => ref mat4;
		[IgnoreDataMember]
		public Quaternion Rotation
		{
			get => rot;
			set
			{
				rot = value;
				IsDirty = true;
			}
		}
		[IgnoreDataMember]
		public Vector3 Position
		{
			get => pos;
			set
			{
				pos = value;
				IsDirty = true;
			}
		}
		[IgnoreDataMember]
		public Vector3 Scale
		{
			get => sca;
			set
			{
				sca = value;
				IsDirty = true;
			}
		}

		public Transform() { owner = null!; }
		public Transform(GameObject owner) { this.owner = owner; }

		public ref Matrix4 GenerateMatrix(TransformType? typeOverride = null)
		{
			typeOverride = typeOverride ?? typ;
			if (!IsDirty && (typeOverride != TransformType.Relative || owner.ParentID == null || ((owner.Parent as GameObject)?.Transform?.IsDirty ?? false)))
				return ref mat4;
			IsDirty = false;
			// ISROT: Identity, Scale, Rotation, Orbit (Radius, Orbit), Translation

			// IDENTITY
			GameObject? parent;
			// Initialize with either the parent's matrix, or the Identity matrix
			if ((typeOverride ?? typ) == TransformType.Relative && (parent = owner.Parent as GameObject) != null)
				mat4 = parent.Transform.GenerateMatrix();
			else mat4 = Matrix4.Identity;

			// SCALE
			if (sca != Vector3.One)
				Matrix4.Mult(mat4, Matrix4.CreateScale(sca), out mat4);
			// ROTATION
			if (rot != Quaternion.Identity)
				Matrix4.Mult(mat4, Matrix4.CreateFromQuaternion(rot), out mat4);

			// ORBIT
			//		RADIUS
			if (rad != Vector3.Zero)
				Matrix4.Mult(mat4, Matrix4.CreateTranslation(rad), out mat4);
			//		ORBIT
			if (orb != Quaternion.Identity)
				Matrix4.Mult(mat4, Matrix4.CreateFromQuaternion(orb), out mat4);

			// TRANSLATION
			if (pos != Vector3.Zero)
				Matrix4.Mult(mat4, Matrix4.CreateTranslation(pos), out mat4);

			return ref mat4;
		}

		public override string ToString() => $"P{Position}|S{Scale}|R({Rotation})";
	}
}
