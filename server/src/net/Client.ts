import { WebSocket, RawData } from "ws";
import { randomUUID } from 'crypto';
import { OwnedEvent } from "../utils/Event";
import Message from "./Message";
import CloseEvent from "./CloseEvent";
import Request from "./Request";

export default class Client {
	// #region Fields
	private requests = new Map<number, Request>();
	readonly socket: WebSocket;
	readonly id: string = randomUUID();
	// #endregion

	// #region Events
	private closed = new OwnedEvent<Client, CloseEvent>(this);
	private message = new OwnedEvent<Client, Message>(this);
	get onClosed() { return this.closed.event; }
	get onMessage() { return this.message.event; }
	// #endregion

	// #region Properties
	get isClosed(): boolean { return this.socket.readyState === WebSocket.CLOSED || this.socket.readyState === WebSocket.CLOSING }
	get isOpen(): boolean { return this.socket.readyState === WebSocket.OPEN }
	// #endregion

	private constructor(socket: WebSocket) {
		this.socket = socket;
		socket.on('message', this.handleMessage.bind(this));
		socket.on('close', this.handleClose.bind(this));
	}

	// #region Event Handlers
	private handleMessage(data: RawData) {
		const message = Message.parse(data.toString('utf8'));
		// Handle any Client specific messages
		this.message.trigger(message);
	}

	private handleClose(code: number, reason: Buffer) {
		this.closed.trigger({ code, reason: reason.toString('utf8') });
	}
	// #endregion

	processTimeouts(time: number) {
		for(const [id, request] of this.requests) {
			if (time >= request.timestamp + request.timeout) {
				request.promise.reject("timeout");
				this.requests.delete(id);
			}
		}
	}

	static create(socket: WebSocket) {
		const client = new Client(socket);
		Message.success(socket, client.id);
		return client;
	}
}