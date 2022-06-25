import { WebSocket } from "ws";
import { isInt } from "../utils/NumberCheck";
import { CommandHeader, ErrorHeader } from "./Commands";

type Header = CommandHeader | ErrorHeader;
type Payload = boolean | number | string | Object;

class MessageBuilder {
	private _header: Header;
	private _payload?: Payload;
	private _last_serialized?: string;

	constructor(header: Header) {
		this._header = header;
	}

	getHeader(): Header { return this._header; }
	header(value: Header): MessageBuilder {
		delete this._last_serialized;
		this._header = value;
		return this;
	}

	getPayload(): Payload | undefined { return this._payload; }
	payload(data: Payload): MessageBuilder {
		delete this._last_serialized;
		this._payload = data;
		return this;
	}


	send(socket: WebSocket): MessageBuilder {
		if (this._last_serialized !== undefined)
			socket.send(this._last_serialized);
		else
			socket.send(this.pack());
		return this;
	}

	async request(socket: WebSocket): Promise<Message> {
		return new Promise<Message>((res, rej) => {

		});
	}

	private serializePayload(): string {
		switch (typeof this._payload) {
			case 'boolean': return this._payload ? "y" : "n";
			case 'number':
				return (isInt(this._payload) ? 'i' : 'f') + this._payload;
			case 'string': return 's' + this._payload;
			case 'object': return 'o' + JSON.stringify(this._payload);
			default: throw Error("MessageBuilder: Invalid Payload Type - " + typeof this._payload);
		}
	}

	private pack(): string {
		return this._last_serialized = (this._payload !== undefined && this._payload !== null)
			? this._header.toString(16) + ':' + this.serializePayload()
			: this._header.toString(16);
	}
}

const Parser: {
	[key: string]: (x: string) => Payload
} = {
	y: (_: string) => true,
	n: (_: string) => false,
	i: parseInt,
	f: parseFloat,
	s: (x: string) => x,
	o: JSON.parse
}

export default class Message {
	private _header!: Header;
	private _payload?: Payload;
	private _type?: string;

	get header() { return this._header; }
	get payload() { return this._payload; }

	get isError(): boolean { return !!((this._header.valueOf() as number) & 0x8000); }
	get isBoolean(): boolean { return this._type === 'y' || this._type === 'n'; }
	get isInteger(): boolean { return this._type === 'i'; }
	get isFloat(): boolean { return this._type === 'f'; }
	get isNumber(): boolean { return this.isFloat || this.isInteger; }
	get isString(): boolean { return this._type === 's'; }
	get isObject(): boolean { return this._type === 'o'; }

	static build(header: Header): MessageBuilder {
		return new MessageBuilder(header);
	}

	static parse(data: string): Message {
		const message = new Message();
		const delimiter = data.indexOf(':');
		message._header = parseInt(delimiter < 0 ? data : data.slice(0, delimiter), 16);
		if (delimiter >= 0) {
			message._type = data[delimiter + 1];
			if (!(message._type in Parser))
				throw Error('Message.parse(): Invalid Payload Type - ' + message._type);
			message._payload = Parser[message._type](data.slice(delimiter + 2));
		}
		return message;
	}

	static success(socket: WebSocket, data?: Payload) {
		this.send(socket, CommandHeader.Success, data);
	}

	static failed(socket: WebSocket, error: ErrorHeader, reason?: Payload) {
		this.send(socket, error, reason);
	}

	static send(socket: WebSocket, header: Header, payload?: Payload) {
		const msg = new MessageBuilder(header)
		if (payload !== undefined)
			msg.payload(payload);
		msg.send(socket);
	}
}