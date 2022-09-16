using System;
using System.Runtime.Serialization;

namespace Erkunden.ECS
{
	[DataContract]
	public struct EntityID : IComparable, IComparable<EntityID>, IEquatable<EntityID>, IEquatable<ulong>
	{
		[DataMember(Name = "i", IsRequired = true, EmitDefaultValue = true)]
		private ulong ID;
		public bool IsInvalid => ID == 0;

		public static readonly EntityID Invalid = new EntityID { ID = 0 };

		public int CompareTo(EntityID other) => ID.CompareTo(other.ID);
		public int CompareTo(object obj) => obj is EntityID ? ID.CompareTo(((EntityID)obj).ID) : obj is ulong ? ID.CompareTo(obj) : 0;
		public bool Equals(ulong other) => ID == other;
		public bool Equals(EntityID other) => ID == other.ID;
		public override bool Equals(object obj)
		{
			if (obj is ulong) return ID == (ulong)obj;
			else if (obj is EntityID) return ID == ((EntityID)obj).ID;
			else return false;
		}
		public override int GetHashCode() => ID.GetHashCode();
		public override string ToString() => "E" + ID.ToString("x");

		public static EntityID Parse(string value)
		{
			if (!value.StartsWith('E')) throw new FormatException();
			return (EntityID)Convert.ToUInt64(value.Substring(1), 16);
		}
		public static bool TryParse(string value, out EntityID idOut)
		{
			try { idOut = Parse(value); return true; }
			catch { idOut = (EntityID)0; return false; }
		}

		public static explicit operator ulong(EntityID entityID) => entityID.ID;
		public static explicit operator EntityID(ulong entityID) => new EntityID { ID = entityID };
		public static bool operator ==(EntityID left, ulong right) => left.ID == right;
		public static bool operator !=(EntityID left, ulong right) => left.ID != right;
		public static bool operator ==(EntityID left, uint right) => left.ID == right;
		public static bool operator !=(EntityID left, uint right) => left.ID != right;
	}
}
