
export enum CommandHeader {
	UNUSED_0 = 0x0000,

	ServerSaving = 0x0001,
	ServerShutdown = 0x0002,
	ServerMessage = 0x0003,

	UNUSED_2 = 0x2000,

	Success = 0x4200,

	UNUSED_8 = 0x8000,
};

export enum ErrorHeader {
	Unknown = 0xF001,
	Unauthorized = 0xF401,
	Forbidden = 0xF403,
	NotFound = 0xF404,
}