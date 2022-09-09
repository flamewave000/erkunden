import express from 'express';
import * as Tags from '../../Tags';
import { StatusCodes } from 'http-status-codes';
import Router from './Router';
import Authenticator from '../Authenticator';


const Errors = {
	Unknown: { code: StatusCodes.INTERNAL_SERVER_ERROR, error: "unknown_error" },
	NoUser: { code: StatusCodes.BAD_REQUEST, error: "missing_username" },
	NoPass: { code: StatusCodes.BAD_REQUEST, error: "missing_password" },
	BadToken: { code: StatusCodes.NOT_FOUND, error: "invalid_token" },
	BadCreds: { code: StatusCodes.NOT_FOUND, error: "invalid_credentials" },
}
export default class OAuth extends Router {
	registerRoutes(router: express.Router): void {
		router.post('/auth', this.authenticate.bind(this));
		router.delete('/auth', this.unauthenticate.bind(this));
	}

	private async authenticate(req: express.Request, res: express.Response) {
		// Validate request data
		if (!req.body.user) return this.ERROR(res, Errors.NoUser);
		if (!req.body.pass) return this.ERROR(res, Errors.NoPass);
		// Validate the user's credentials
		const userId = await Authenticator.validateCreds(req.body.user, req.body.pass);
		if (!userId) return this.ERROR(res, Errors.BadCreds);
		this.LOG(Tags.Color.FgGreen + "(login) " + req.body.user);
		// Update the "last_login" field to now
		await this.set('user', userId, 'last_login', Date.now());
		res.json({ token: await Authenticator.createToken(userId) });
	}

	private async unauthenticate(req: express.Request, res: express.Response) {
		const creds = await Authenticator.validate(req);
		if (!creds) return this.ERROR(res, Errors.BadToken);
		const userId = await Authenticator.revokeToken(creds.token);
		if (!userId)
			return this.ERROR(res, Errors.Unknown);
		this.LOG(Tags.Color.FgYellow + "(logout) " + await this.get('user', userId, 'username'));
		res.sendStatus(StatusCodes.OK);
	}
}