import express from 'express';
import { StatusCodes } from 'http-status-codes';
import Router from './Router';
import Authenticator from '../Authenticator';
import * as Tags from '../../Tags';


const Errors = {
	NoAuth: { code: StatusCodes.FORBIDDEN, error: "authentication_required" },
	NoUser: { code: StatusCodes.BAD_REQUEST, error: "missing_username" },
	NoPass: { code: StatusCodes.BAD_REQUEST, error: "missing_password" },
	NoNewPass: { code: StatusCodes.BAD_REQUEST, error: "missing_new_password" },
	UserExists: { code: StatusCodes.NOT_ACCEPTABLE, error: "user_exists" },
	UserNonexistant: { code: StatusCodes.NOT_FOUND, error: "user_does_not_exist" },
	MissingUser: { code: StatusCodes.BAD_REQUEST, error: "missing_user_id" },
	BadToken: { code: StatusCodes.FORBIDDEN, error: "invalid_token" },
	BadPass: { code: StatusCodes.FORBIDDEN, error: "invalid_old_password" },
}
export default class Accounts extends Router {

	registerRoutes(router: express.Router): void {
		router.get('/account/:username', this.getAccount.bind(this));
		router.head('/account/:username', this.checkAccountExists.bind(this));
		router.post('/account', this.newAccount.bind(this));
		router.put('/account/username', this.updateUsername.bind(this));
		router.put('/account/password', this.updatePassword.bind(this));
	}

	private async updateUsername(req: express.Request, res: express.Response) {
		const authentication = await Authenticator.validate(req);
		if (!authentication) return this.ERROR(res, Errors.NoAuth);
		if (!req.body.username) return this.ERROR(res, Errors.NoUser);
		const oldUsername = await this.get('user', authentication.user, 'username');
		const success = await this.redis.hSetNX('users', req.body.username, authentication.user);
		if (!success) return this.ERROR(res, Errors.UserExists);
		await this.redis.hDel('users', oldUsername!);
		await this.set('user', authentication.user, 'username', req.body.username);
		this.LOG(`(Rename) ${oldUsername} -> ${req.body.username}`);
		res.sendStatus(StatusCodes.OK);
	}

	private async updatePassword(req: express.Request, res: express.Response) {
		const authentication = await Authenticator.validate(req);
		if (!authentication) return this.ERROR(res, Errors.NoAuth);
		if (!req.body.oldpass) return this.ERROR(res, Errors.NoPass);
		if (!req.body.newpass) return this.ERROR(res, Errors.NoNewPass);

		const username = await this.get('user', authentication.user, 'username');
		// Check if the old password is valid
		if (!await Authenticator.validateCreds(username!, req.body.oldpass))
			return this.ERROR(res, Errors.BadPass);

		// Update the new password
		const salt = Authenticator.generateSalt();
		const pass = await Authenticator.hash(req.body.newpass, salt);
		await this.redis.multi()
			.hSet('user:' + authentication.user, 'salt', salt)
			.hSet('user:' + authentication.user, 'password', pass)
			.exec();
		this.LOG('(' + username + ') Updated Password');
		res.sendStatus(StatusCodes.OK);
	}

	private async checkAccountExists(req: express.Request, res: express.Response) {
		if (!req.params.username) res.sendStatus(StatusCodes.BAD_REQUEST);
		else if (!await this.get('users', req.params.username)) res.sendStatus(StatusCodes.NOT_FOUND);
		else res.sendStatus(StatusCodes.OK);
	}

	private async getAccount(req: express.Request, res: express.Response) {
		const authentication = await Authenticator.validate(req);
		const username = req.body.user_name;
		let userId = req.body.user_id ?? authentication?.user;
		// If an explicit username is provided, search for it instead
		if (!!username) {
			userId = await this.get('users', username);
			if (!userId) return this.ERROR(res, Errors.UserNonexistant);
		}
		if (!!userId) {
			const data = await this.getAll('user', userId);
			res.json({
				username: data['username'],
				created: data['created'],
				last_login: data['last_login'],
			});
		}
		else this.ERROR(res, Errors.MissingUser);
	}

	private async newAccount(req: express.Request, res: express.Response) {
		// Validate request data
		if (!req.body.user) return this.ERROR(res, Errors.NoUser);
		if (!req.body.pass) return this.ERROR(res, Errors.NoPass);
		if (await this.redis.hGet('users', req.body.user)) return this.ERROR(res, Errors.UserExists);
		const salt = Authenticator.generateSalt();
		const password = await Authenticator.hash(req.body.pass, salt);
		const userId = (await this.redis.incr('user_root')).toString();
		// Register the new User ID
		await this.set('users', req.body.user, userId);
		// Create the User Data
		const user: Record<string, string | number> = {
			username: req.body.user,
			password, salt,
			created: Date.now(),
			last_login: Date.now()
		};
		// Store the new User Data
		await this.redis.hSet('user:' + userId, user);
		this.LOG(Tags.Color.FgGreen + `: New Account - "${req.body.user}"`);
		// Return the new User ID
		res.json({ user_id: userId });
	}
}