namespace Assets.Scripts.Net
{
	public struct Header
	{
		private readonly ushort code;
		private Header(ushort code)
		{
			this.code = code;
		}

		public bool HasFlags(ushort flags) => (code & flags) != 0;

		public static implicit operator Header(int code) => new((ushort)code);
		public static implicit operator Header(uint code) => new((ushort)code);
		public static implicit operator Header(ushort code) => new(code);
		public static implicit operator int(Header header) => header.code;
		public static implicit operator uint(Header header) => header.code;
		public static implicit operator ushort(Header header) => header.code;

		public const ushort INVALID = 0x0;

		#region CRUD Operations
		public const ushort CREATE = 0x1;
		public const ushort LOOKUP = 0x2;
		public const ushort UPDATE = 0x3;
		public const ushort DELETE = 0x4;
		#endregion

		#region Network
		public const ushort NETWORK = 0x1000;

		public const ushort SERVER = NETWORK | 0x100;
		public const ushort ServerSaving = SERVER | 0x01;
		public const ushort ServerShutdown = SERVER | 0x02;
		public const ushort ServerMessage = SERVER | 0x03;

		public const ushort CLIENT = NETWORK | 0x200;
		public const ushort BindClientID = CLIENT | 0x01;
		public const ushort Authenticate = CLIENT | 0x02;
		#endregion

		#region Object Messages
		public const ushort OBJECT = 0x2000;
		public const ushort PLAYER = OBJECT | 0x200;
		public const ushort MOBILE = OBJECT | 0x400;

		public const ushort ObjectCreate = OBJECT | CREATE;
		public const ushort ObjectUpdate = OBJECT | UPDATE;
		public const ushort ObjectDelete = OBJECT | DELETE;

		public const ushort PlayerUpdate = PLAYER | UPDATE;

		public const ushort MobileCreate = MOBILE | CREATE;
		public const ushort MobileUpdate = MOBILE | UPDATE;
		public const ushort MobileDelete = MOBILE | DELETE;
		#endregion

		public const ushort UNUSED_4 = 0x4000;
		public const ushort UNUSED_8 = 0x8000;

		#region Errors
		public const ushort ERROR = 0xF000;
		public const ushort Unauthorized = ERROR | 0x401;
		public const ushort Forbidden = ERROR | 0x403;
		public const ushort NotFound = ERROR | 0x404;
		#endregion
	}
}
