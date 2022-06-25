type EventListener<TSender, TEvent> = (sender: TSender, event: TEvent) => void;

export default class Event<TSender, TEvent> {
	protected lastHandle = 0;
	protected listeners = new Map<number, EventListener<TSender, TEvent>>();
	protected owner: TSender;

	constructor(owner: TSender) {
		this.owner = owner;
	}

	on(listener: EventListener<TSender, TEvent>): number {
		const handle = ++this.lastHandle;
		this.listeners.set(handle, listener);
		return handle;
	}

	off(handleOrListener: number | EventListener<TSender, TEvent>): boolean {
		if (typeof handleOrListener === 'number')
			return this.listeners.delete(handleOrListener);
		const size = this.listeners.size;
		for (const [handle, listener] of this.listeners) {
			if (listener === handleOrListener)
				this.listeners.delete(handle);
		}
		return size !== this.listeners.size;
	}
}
export class OwnedEvent<TSender, TEvent> extends Event<TSender, TEvent> {
	get event(): Event<TSender, TEvent> { return this; }

	constructor(owner: TSender) {
		super(owner)
	}
	trigger(data: TEvent) {
		for (const listener of this.listeners) {
			listener[1](this.owner, data);
		}
	}
}