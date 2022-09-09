using System;

namespace Assets.Scripts.Utils
{

	[Serializable]
	public class DeferredAlreadyCompleteException : Exception
	{
		public DeferredAlreadyCompleteException() : base("Deferred object has already been completed by either resolve or reject") { }
		public DeferredAlreadyCompleteException(string message) : base(message) { }
		public DeferredAlreadyCompleteException(string message, Exception inner) : base(message, inner) { }
		protected DeferredAlreadyCompleteException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	public interface IDeferred<TResult>
	{
		public IDeferred<TResult> OnResult(Action<TResult> handler);
		public IDeferred<TNew> OnResult<TNew>(Func<TResult, TNew> handler);
		public IDeferred<TNew> OnResult<TNew>(Func<TResult, IDeferred<TNew>> handler);
		public IDeferred<TResult> OnError(Action<Exception> handler);
		public IDeferred<TResult> OnComplete(Action<Optional<TResult>, Exception> handler);

		public static IDeferred<TResult> Resolved(TResult result) => new ResolvedDeferred<TResult>(result);
		public static IDeferred<TResult> Rejected(Exception error) => new RejectedDeferred<TResult>(error);

		private class ResolvedDeferred<T> : IDeferred<T>
		{
			private readonly T result;
			public ResolvedDeferred(T result)
			{
				this.result = result;
			}
			public IDeferred<T> OnError(Action<Exception> handler) => this;
			public IDeferred<TNew> OnResult<TNew>(Func<T, TNew> handler) => IDeferred<TNew>.Resolved(handler(result));
			public IDeferred<TNew> OnResult<TNew>(Func<T, IDeferred<TNew>> handler) => handler(result);
			public IDeferred<T> OnResult(Action<T> handler)
			{
				handler(result);
				return this;
			}
			public IDeferred<T> OnComplete(Action<Optional<T>, Exception> handler)
			{
				handler((Optional<T>)result, null);
				return this;
			}
		}
		private class RejectedDeferred<T> : IDeferred<T>
		{
			private readonly Exception error;
			public RejectedDeferred(Exception error)
			{
				this.error = error;
			}
			public IDeferred<T> OnError(Action<Exception> handler)
			{
				handler(error);
				return this;
			}
			public IDeferred<TNew> OnResult<TNew>(Func<T, TNew> handler) => IDeferred<TNew>.Rejected(error);
			public IDeferred<TNew> OnResult<TNew>(Func<T, IDeferred<TNew>> handler) => IDeferred<TNew>.Rejected(error);
			public IDeferred<T> OnResult(Action<T> handler) => this;
			public IDeferred<T> OnComplete(Action<Optional<T>, Exception> handler)
			{
				handler(Optional<T>.Empty(), error);
				return this;
			}
		}
	}

	public interface IMutableDeferred<TResult> : IDeferred<TResult>
	{
		public void Resolve(TResult result);
		public void Reject(string error);
		public void Reject(Exception error);
	}

	public interface IDeferred<TClass, TResult> : IDeferred<TResult>
		where TClass : IDeferred<TClass, TResult>
	{
		public new TClass OnResult(Action<TResult> handler);
		public new TClass OnError(Action<Exception> handler);
	}

	public abstract class Deferred<TResult> : IDeferred<Deferred<TResult>, TResult>
	{
		public bool isCompleted { get; protected set; }
		public bool isSuccess => isCompleted && error == null;
		public bool isFailure => isCompleted && error != null;

		protected TResult result;
		protected Exception error = null;

		IDeferred<TResult> IDeferred<TResult>.OnResult(Action<TResult> handler) => OnResult(handler);
		public abstract Deferred<TResult> OnResult(Action<TResult> handler);
		public abstract IDeferred<TNew> OnResult<TNew>(Func<TResult, TNew> handler);
		public abstract IDeferred<TNew> OnResult<TNew>(Func<TResult, IDeferred<TNew>> handler);

		IDeferred<TResult> IDeferred<TResult>.OnError(Action<Exception> handler) => OnError(handler);
		public abstract Deferred<TResult> OnError(Action<Exception> handler);

		public abstract IDeferred<TResult> OnComplete(Action<Optional<TResult>, Exception> handler);
	}

	public class MutableDeferred<TResult> : Deferred<TResult>, IMutableDeferred<TResult>
	{
		protected event Action<TResult> onResult;
		protected event Action<Exception> onError;
		protected event Action<Optional<TResult>, Exception> onComplete;

		public TResult Result => result;
		public Exception Error => error;

		#region Deferred Completion
		public void Resolve(TResult result)
		{
			if (isCompleted) throw new DeferredAlreadyCompleteException();
			isCompleted = true;
			this.result = result;
			onResult?.Invoke(result);
			onComplete?.Invoke(Optional<TResult>.Of(result), null);
			onResult = null;
			onError = null;
			onComplete = null;
		}
		public void Reject(string error) => Reject(new Exception(error));
		public void Reject(Exception error)
		{
			if (isCompleted) throw new DeferredAlreadyCompleteException();
			isCompleted = true;
			this.error = error;
			onError?.Invoke(error);
			onComplete?.Invoke(Optional<TResult>.Empty(), error);
			onResult = null;
			onError = null;
			onComplete = null;
		}
		#endregion

		#region IDeferreed Implementation
		public override Deferred<TResult> OnResult(Action<TResult> handler)
		{
			if (isCompleted)
			{
				if (isSuccess) handler(result);
			}
			else onResult += handler;
			return this;
		}
		public override IDeferred<TNew> OnResult<TNew>(Func<TResult, TNew> handler)
		{
			if (isCompleted)
			{
				if (isSuccess) return IDeferred<TNew>.Resolved(handler(result));
				return IDeferred<TNew>.Rejected(error);
			}
			var defer = new MutableDeferred<TNew>();
			onResult += x => defer.Resolve(handler(x));
			onError += defer.Reject;
			return defer;
		}
		public override IDeferred<TNew> OnResult<TNew>(Func<TResult, IDeferred<TNew>> handler)
		{
			if (isCompleted)
			{
				if (isSuccess) return handler(result);
				return IDeferred<TNew>.Rejected(error);
			}

			var deferred = new MutableDeferred<TNew>();
			onResult += x =>
			{
				var target = handler(x);
				target.OnResult(deferred.Resolve);
				target.OnError(deferred.Reject);
			};
			onError += deferred.Reject;
			return deferred;
		}

		public override Deferred<TResult> OnError(Action<Exception> handler)
		{
			if (isCompleted)
			{
				if (isFailure) handler(error);
			}
			else onError += handler;
			return this;
		}

		public override IDeferred<TResult> OnComplete(Action<Optional<TResult>, Exception> handler)
		{
			if (isCompleted)
			{
				if (isSuccess) handler(Optional<TResult>.Of(result), null);
				else handler(Optional<TResult>.Empty(), error);
			}
			else onComplete += handler;
			return this;
		}
		#endregion
	}
}
