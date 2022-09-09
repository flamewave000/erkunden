using System;

namespace Assets.Scripts.Serialization
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class SerializeAttribute : Attribute
	{
		public readonly string name;
		public Type serializer;

		public SerializeAttribute(string name = null)
		{
			this.name = name;
		}
	}
}
