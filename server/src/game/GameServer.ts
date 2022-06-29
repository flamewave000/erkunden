
import WebSocket from 'ws';
import { IncomingMessage } from 'http';
import Client from './net/Client';
import Message from './net/Message';
import Header from './net/Headers';
import CloseEvent from './net/CloseEvent';
import * as Tags from '../Tags';

export namespace Game {
	export interface Options {
		port: number;
	}
	export class Server {
		private static _socketServer?: WebSocket.Server;
		private static clients = new Map<string, Client>();
		private static requestTimeoutProcessorHandle?: NodeJS.Timer;

		static get socketServer(): WebSocket.Server {
			if (!this._socketServer) throw new Error("Server: Illegal access of socketServer, start() has not been called yet");
			return this._socketServer;
		}

		static start(options: Options) {
			this._socketServer = new WebSocket.WebSocketServer({ port: options.port }, this.startedEvent.bind(this));
			this._socketServer.on('listening', this.listeningEvent.bind(this));
			this._socketServer.on('connection', this.clientConnected);
			this.requestTimeoutProcessorHandle = setInterval(this.processRequestTimeouts.bind(this), 500);
		}

		static stop() {
			const message = Message.build(Header.ServerShutdown);
			for (const [_, client] of this.clients) {
				message.send(client.socket);
			}

			this.socketServer.close();
			clearTimeout(this.requestTimeoutProcessorHandle);
		}

		// #region Event Handlers
		private static listeningEvent() {
			console.log(`${Tags.GameServer}ws://${this.socketServer.options.host ?? 'localhost'}:${this.socketServer.options.port}`);
		}
		private static startedEvent() { console.log(`${Tags.GameServer}${Tags.Color.FgGreen}Started`); }

		// Websocket function that managages connection with clients
		private static clientConnected(socket: WebSocket.WebSocket, request: IncomingMessage) {
			const client = Client.create(socket);
			console.log(Tags.GameClient + ': (Connected) ' + client.id);
			this.clients.set(client.id, client);
			client.onClosed.on(this.handleClientClosed.bind(this));
		}

		private static handleClientClosed(sender: Client, event: CloseEvent) {
			console.log(Tags.GameClient + ': (Disconnected) ' + sender.id);
			this.clients.delete(sender.id);
		}
		// #endregion

		private static processRequestTimeouts() {
			const time = Date.now();
			for (const [_, client] of this.clients) {
				client.processTimeouts(time);
			}
		}
	}
}