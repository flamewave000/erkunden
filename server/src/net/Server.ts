
import WebSocket from 'ws';
import { IncomingMessage } from 'http';
import Client from './Client';
import Message from './Message';
import { CommandHeader } from './Commands';
import CloseEvent from './CloseEvent';

export default class Server {

	private _socketServer?: WebSocket.Server;
	private clients = new Map<string, Client>();
	private requestTimeoutProcessorHandle?: NodeJS.Timer;

	get socketServer(): WebSocket.Server {
		if (!this._socketServer) throw new Error("Server: Illegal access of socketServer, start() has not been called yet");
		return this._socketServer;
	}

	start(port: number) {
		this._socketServer = new WebSocket.WebSocketServer({ port }, this.startedEvent.bind(this));
		this._socketServer.on('listening', this.listeningEvent.bind(this));
		this._socketServer.on('connection', this.clientConnected);

		this.requestTimeoutProcessorHandle = setInterval(this.processRequestTimeouts.bind(this), 500);
	}

	stop() {
		const message = Message.build(CommandHeader.ServerShutdown);
		for (const [_, client] of this.clients) {
			message.send(client.socket);
		}

		this.socketServer.close();
		clearTimeout(this.requestTimeoutProcessorHandle);
	}

	// #region Event Handlers
	private listeningEvent() {
		console.log(`Server Status: //${this.socketServer.options.host ?? 'localhost'}:${this.socketServer.options.port}`);
	}
	private startedEvent() { console.log("Server Status: Started"); }

	// Websocket function that managages connection with clients
	private clientConnected(socket: WebSocket.WebSocket, request: IncomingMessage) {
		const client = Client.create(socket);
		console.log('Client Connected: ' + client.id);
		this.clients.set(client.id, client);
		client.onClosed.on(this.handleClientClosed.bind(this));
	}

	private handleClientClosed(sender: Client, event: CloseEvent) {
		console.log('Client Disconnected: ' + sender.id);
		this.clients.delete(sender.id);
	}
	// #endregion

	private processRequestTimeouts() {
		const time = Date.now();
		for (const [_, client] of this.clients) {
			client.processTimeouts(time);
		}
	}
}