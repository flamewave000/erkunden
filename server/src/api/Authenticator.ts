import express from "express";
import RedisClient from "../db/RedisClient";
import crypto from 'crypto';
import { promisify } from "util";


export default class Authenticator {

	public static generateSalt(): string {
		return crypto.randomBytes(16).toString('hex');
	}

	public static async hash(data: string, salt: string): Promise<string> {
		return (await promisify(crypto.pbkdf2)(data, salt, 1000, 64, 'sha512')).toString('hex');
	}

	public static async validateCreds(username: string, password: string): Promise<string | null> {
		const userId = await RedisClient.redis.hGet('users', username);
		if (!userId) return null;
		const passDB = await RedisClient.redis.hGet('user:' + userId, 'password');
		const saltDB = await RedisClient.redis.hGet('user:' + userId, 'salt');
		const challenge = await this.hash(password, saltDB!);
		return passDB === challenge ? userId : null;
	}

	public static async validateToken(token: string): Promise<string | null> {
		return await RedisClient.redis.hGet('tokens', token) ?? null;
	}

	/**
	 * Validates the authorization of the given request.
	 * @param req Request to be validated.
	 * @returns The Token and User ID if the token is valid, otherwise will return null.
	 */
	public static async validate(req: express.Request): Promise<{ token: string, user: string } | null> {
		const bearer = req.header("Authorization");
		if (!bearer || !bearer.startsWith("Bearer ")) return null;
		const token = bearer.substring("Bearer ".length);
		const user = await this.validateToken(token);
		return user ? { user, token } : null;
	}
	/**
	 * Creates a new Access Token and stores it in the database.
	 * @param userId User the access token is for.
	 * @returns The generated access token.
	 */
	public static async createToken(userId: string): Promise<string> {
		// Verify the user exists, throw an error if it doesn't
		if (!await RedisClient.redis.hGet('user:' + userId, 'created')) throw Error("Invalid User ID");
		// Create a token by hashing the userid with the new salt
		const token = await this.hash(userId, this.generateSalt());
		// Save the token
		await RedisClient.redis.hSet('tokens', token, userId);
		return token;
	}

	/**
	 * Removes the provided token from the tokens table
	 * @param token Access Token to be revoked.
	 * @returns User ID if token was successfully removed, otherwise null if it didn't exist.
	 */
	public static async revokeToken(token: string): Promise<string | null> {
		const userId = await RedisClient.redis.hGet('tokens', token);
		return userId && await RedisClient.redis.hDel('tokens', token) !== 0
			? userId : null;
	}
}