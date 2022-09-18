using System;
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
		private Quaternion tilt = Quaternion.Identity;
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
		public Quaternion Tilt
		{
			get => tilt;
			set
			{
				tilt = value;
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

		public ref Matrix4 UpdateMatrix(TransformType? typeOverride = null)
		{
			GenerateMatrix(typeOverride, false, false, ref mat4);
			return ref mat4;
		}
		public void GenerateMatrix(TransformType? typeOverride, bool IgnoreTilt, bool ignoreDirty, ref Matrix4 result)
		{
			typeOverride = typeOverride ?? typ;
			if ((!ignoreDirty && !IsDirty) && (typeOverride != TransformType.Relative || owner.ParentID == null || ((owner.Parent as GameObject)?.Transform?.IsDirty ?? false)))
				return;
			if (!ignoreDirty) IsDirty = false;

			// ISROT: Identity, Scale, Rotation, Orbit (Radius, Orbit), Translation

			// IDENTITY
			GameObject? parent;
			// Initialize with either the parent's matrix, or the Identity matrix
			if ((typeOverride ?? typ) == TransformType.Relative && (parent = owner.Parent as GameObject) != null)
				result = parent.Transform.UpdateMatrix();
			else result = Matrix4.Identity;

			// SCALE
			if (sca != Vector3.One)
				Matrix4.Mult(result, Matrix4.CreateScale(sca), out result);
			// ROTATION
			if (rot != Quaternion.Identity)
				Matrix4.Mult(result, Matrix4.CreateFromQuaternion(rot), out result);
			if (!IgnoreTilt && tilt != Quaternion.Identity)
				Matrix4.Mult(result, Matrix4.CreateFromQuaternion(tilt), out result);

			//// ORBIT
			////		RADIUS
			//if (rad != Vector3.Zero)
			//	Matrix4.Mult(mat4, Matrix4.CreateTranslation(rad), out mat4);
			////		ORBIT
			//if (orb != Quaternion.Identity)
			//	Matrix4.Mult(mat4, Matrix4.CreateFromQuaternion(orb), out mat4);

			// TRANSLATION
			if (pos != Vector3.Zero)
				Matrix4.Mult(result, Matrix4.CreateTranslation(pos), out result);
		}

		public override string ToString() => $"P{Position}|S{Scale}|R({Rotation})";

		public void Reset(bool scale = true, bool position = true, bool rotation = true, bool orbit = true, bool radius = true, bool tilt = true)
		{
			IsDirty = true;
			mat4 = Matrix4.Identity;
			if (scale) sca = Vector3.One;
			if (radius) rad = Vector3.Zero;
			if (orbit) orb = Quaternion.Identity;
			if (rotation) rot = Quaternion.Identity;
			if (tilt) this.tilt = Quaternion.Identity;
			if (position) pos = Vector3.Zero;
		}
	}
}
