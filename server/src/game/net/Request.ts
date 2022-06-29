import PromiseResolver from "../../utils/PromiseResolver";
import Message from "./Message";

export default class Request {
	id: number;
	promise: PromiseResolver<Message>;
	timestamp: number;
	timeout: number;
	constructor(id: number, promise: PromiseResolver<Message>, timestamp: number, timeout: number) {
		this.id = id;
		this.promise = promise;
		this.timestamp = timestamp;
		this.timeout = timeout;
	}
}