import fs from 'fs';
import { Game } from "./game/GameServer";
import RedisClient from "./db/RedisClient";
import { Api } from './api/ApiServer';
import * as JSONC from 'jsonc-parser';

interface Config {
	api: Api.Options,
	game: Game.Options
}

const config: Config = JSONC.parse(fs.readFileSync(__dirname + '/../config.jsonc', { encoding: 'utf8' }));

(async function START() {
	await RedisClient.start();
	Api.Server.start(config.api, RedisClient.redis);
	Game.Server.start(config.game);
})();
