
export default class Header {

	readonly code: number;
	private constructor(code: number) {
		this.code = code;
	}

	static from(code: number) {
		return new Header(code);
	}

	//#region CRUD Operations
	public static readonly CREATE = 0x1;
	public static readonly LOOKUP = 0x2;
	public static readonly UPDATE = 0x3;
	public static readonly DELETE = 0x4;
	//#endregion

	//#region Network
	public static readonly NETWORK = 0x1000;
	
	public static readonly SERVER = Header.NETWORK | 0x100;
	public static readonly ServerSaving = Header.from(Header.SERVER | 0x01);
	public static readonly ServerShutdown = Header.from(Header.SERVER | 0x02);
	public static readonly ServerMessage = Header.from(Header.SERVER | 0x03);
	
	public static readonly CLIENT = Header.NETWORK | 0x200;
	public static readonly BindClientID = Header.from(Header.CLIENT | 0x01);
	public static readonly Authenticate = Header.from(Header.CLIENT | 0x02);
	//#endregion

	//#region Object Messages
	public static readonly OBJECT = 0x2000;
	public static readonly PLAYER = Header.OBJECT | 0x200;
	public static readonly MOBILE = Header.OBJECT | 0x400;

	public static readonly ObjectCreate = Header.from(Header.OBJECT | Header.CREATE);
	public static readonly ObjectUpdate = Header.from(Header.OBJECT | Header.UPDATE);
	public static readonly ObjectDelete = Header.from(Header.OBJECT | Header.DELETE);

	public static readonly PlayerUpdate = Header.from(Header.PLAYER | Header.UPDATE);

	public static readonly MobileCreate = Header.from(Header.MOBILE | Header.CREATE);
	public static readonly MobileUpdate = Header.from(Header.MOBILE | Header.UPDATE);
	public static readonly MobileDelete = Header.from(Header.MOBILE | Header.DELETE);
	//#endregion

	public static readonly UNUSED_4 = 0x4000;
	public static readonly UNUSED_8 = 0x8000;

	//#region Errors
	public static readonly ERROR = 0xF000;
	public static readonly Unauthorized = Header.from(Header.ERROR | 0x401);
	public static readonly Forbidden = Header.from(Header.ERROR | 0x403);
	public static readonly NotFound = Header.from(Header.ERROR | 0x404);
	//#endregion
}