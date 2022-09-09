using System;

namespace Assets.Scripts.Net.HTTP.Data
{
	[Serializable]
	public struct AccountInfo
	{
		public string id;
		public string username;
		public long created;
		public long lastLogin;
	}
}
