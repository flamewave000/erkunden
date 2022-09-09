using System;
using Assets.Scripts.Utils;

namespace Assets.Scripts.Net
{
	public interface IRequest : IDeferred<IRequest, Message>
	{
		public uint id { get; }
		public void OnTimeout(Action handler);
		public void Cancel(Exception reason);
	}

	public struct Request : IRequest
	{
		public uint id { get; private set; }
		public MutableDeferred<Message> promise { get; private set; }
		public long timestamp { get; private set; }
		public long timeout { get; private set; }

		public Func<uint, Exception, IRequest> cancel { get; private set; }

		public Request(uint id, MutableDeferred<Message> promise, long timestamp, long timeout, Func<uint, Exception, IRequest> cancel)
		{
			this.id = id;
			this.promise = promise;
			this.timestamp = timestamp;
			this.timeout = timeout;
			this.cancel = cancel;
		}

		#region IDeferred Implementation
		IDeferred<Message> IDeferred<Message>.OnResult(Action<Message> handler) => promise.OnResult(handler);
		IDeferred<Message> IDeferred<Message>.OnError(Action<Exception> handler) => promise.OnError(handler);
		public IDeferred<TNew> OnResult<TNew>(Func<Message, TNew> handler) => promise.OnResult(handler);
		public IDeferred<TNew> OnResult<TNew>(Func<Message, IDeferred<TNew>> handler) => promise.OnResult(handler);
		public IRequest OnResult(Action<Message> handler) { promise.OnResult(handler); return this; }
		public IRequest OnError(Action<Exception> handler) { promise.OnError(handler); return this; }
		public IDeferred<Message> OnComplete(Action<Optional<Message>, Exception> handler) { promise.OnComplete(handler); return this; }
		#endregion

		public void OnTimeout(Action handler)
		{
			if (promise.isCompleted)
			{
				if (promise.isFailure && promise.Error is TimeoutException)
				{
					handler();
				}
				return;
			}
			promise.OnError(err =>
			{
				if (err is TimeoutException)
					handler();
			});
		}
		public void Cancel(Exception reason) => cancel(id, reason);
	}
}
