import express from 'express';
import https from 'https';
import http from 'http';
import fs from 'fs';
import { Certificate } from '../utils/Certificate';
import OAuth from './routes/OAuth';
import * as Tags from '../Tags';
import { RedisClientType } from 'redis';
import Accounts from './routes/Accounts';

export namespace Api {
	export interface Options {
		host: string;
		port: number;
		portSSL: number;
		ssl: boolean;
		key?: string;
		cert?: string;
	}

	export class Server {
		private static server: http.Server | https.Server;
		private static oauth: OAuth;
		private static accounts: Accounts;
		private static _app: express.Application;

		public static get app() { return this._app; }

		static async start(options: Options, redis: RedisClientType) {
			if (this.server) throw Error('Api.Server has already been started!');
			this.oauth = new OAuth(redis);
			this.accounts = new Accounts(redis);
			this._app = express();

			Server.app.use(express.json());
			Server.app.use(express.urlencoded({ extended: false }));
			Server.app.use('/api/v1',
				this.oauth.router,
				this.accounts.router);

			let port: number;
			if (options.ssl) {
				const config: { [key: string]: any } = {};
				if (!options.key || !options.cert) {
					console.log(Tags.ApiServer + ': Generated SSL Cert');
					const cert = Certificate.generateX509([
						{ type: 6, value: 'http://localhost' },
						{ type: 7, ip: '127.0.0.1' }
					]);
					config.key = cert.key;
					config.cert = cert.cert;
				} else {
					console.log(`${Tags.ApiServer}: Loaded SSL Cert\n\t key: ${options.key}\n\tcert: ${options.cert}`);
					config.key = fs.readFileSync(options.key!);
					config.cert = fs.readFileSync(options.cert!);
				}
				this.server = https.createServer(config, this.app);
				port = options.portSSL;
			}
			else {
				this.server = http.createServer(this.app);
				port = options.port;
			}
			console.log(`${Tags.ApiServer}${Tags.Color.FgGreen}Started`);
			console.log(`${Tags.ApiServer}${options.ssl ? 'https' : 'http'}://${options.host}:${options.port}`);
			this.server.listen(port, options.host);
		}

		static stop() {
			if (!this.server) throw Error('Api.Server has not been started!');
			this.server.close();
			// @ts-expect-error
			this.server = undefined;
		}
	}
}