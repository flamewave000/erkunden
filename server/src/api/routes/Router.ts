import express from 'express';
import ApiError from '../ApiError';
import * as Tags from '../../Tags';
import { StatusCodes } from 'http-status-codes';
import { RedisClientType } from 'redis';
import { HSETObject, HSETMap, HSETTuples } from '../../../node_modules/@redis/client/dist/lib/commands/HSET';


export default abstract class Router {
	public readonly router = express.Router();

	protected redis: RedisClientType;
	protected Codes = StatusCodes;

	constructor(redis: RedisClientType) {
		this.redis = redis;
		this.registerRoutes(this.router);
	}

	ERROR(res: express.Response, error: ApiError) {
		console.error(`${Tags.ApiServer}${this.constructor.name} ${Tags.Color.FgRed}(${error.code}) ${error.error}`);
		res.status(error.code).json(error);
	}

	LOG(...args: string[]) {
		console.log(`${Tags.ApiServer}${this.constructor.name} ${args.join(' ')}`);
	}

	abstract registerRoutes(router: express.Router): void;

	protected get(table: string, field: string): Promise<string | undefined>;
	protected get(table: string, id: string, field: string): Promise<string | undefined>;
	protected get(table: string, idOrField: string, field?: string): Promise<string | undefined> {
		return !field
			? this.redis.hGet(table, idOrField)
			: this.redis.hGet(table + ':' + idOrField, field);
	}
	protected getAll(table: string, id: string): Promise<{ [x: string]: string; }> {
		return this.redis.hGetAll(table + ':' + id);
	}
	protected set(table: string, field: string, value: string | number | boolean | object | Buffer): Promise<number>;
	protected set(table: string, id: string, field: string, value: string | number | boolean | object | Buffer): Promise<number>;
	protected set(table: string, idOrField: string, fieldOrValue: string | number | boolean | object | Buffer, value?: string | number | boolean | object | Buffer): Promise<number> {
		let converted = value ?? fieldOrValue;
		if (typeof converted === 'number')
			converted = converted.toString();
		if (typeof converted === 'boolean')
			converted = converted.toString();
		if (typeof converted === 'object')
			converted = JSON.stringify(converted);
		if (value === undefined)
			return this.redis.hSet(table, <string>idOrField, converted);
		return this.redis.hSet(table + ':' + idOrField, <string>fieldOrValue, converted);
	}
	protected setAll(table: string, id: string, values: HSETObject | HSETMap | HSETTuples): Promise<number> {
		return this.redis.hSet(table + ':' + id, values);
	}
}