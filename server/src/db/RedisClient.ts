import { createClient, RedisClientType } from 'redis';
import * as Tags from '../Tags';

export default class RedisClient {
	private static _redis?: RedisClientType;

	static get redis() { if (!this._redis) throw new Error("RedisClient: Must call start() first"); return this._redis; }

	static async start(host: string = 'localhost', port: number = 6379) {
		if (this._redis) return;
		this._redis = createClient({
			url: `redis://${host}:${port}`
		});
		this._redis.on('connect', this.handleConnect.bind(this));
		this._redis.on('ready', this.handleReady.bind(this));
		this._redis.on('end', this.handleEnd.bind(this));
		this._redis.on('error', this.handleError.bind(this));
		this._redis.on('reconnecting', this.handleReconnecting.bind(this));
		await this._redis.connect();
	}

	static async stop() {
		if (!this.redis) return;
		await this.redis.quit();
		this._redis = undefined;
	}

	private static handleConnect() {
		console.log(Tags.RedisClient + "Connecting to Server");
	}
	private static handleReady() {
		console.log(Tags.RedisClient + "Connection Ready!");
	}
	private static handleEnd() {
		console.log(Tags.RedisClient + "Disconnected");
	}
	private static handleReconnecting() {
		console.log(Tags.RedisClient + "Reconnecting to Server");
	}
	private static handleError(error: Error) {
		console.error(Tags.RedisClient + Tags.Color.FgRed + ' (Error) ' + error.message, error);
	}
}