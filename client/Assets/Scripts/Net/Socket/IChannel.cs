using System;

namespace Assets.Scripts.Net
{
	public interface IChannel
	{
		/// <summary>
		/// Event that is fired whenever a message is received.
		/// </summary>
		event Action<Message> OnMessage;

		/// <summary>
		/// Send data to the other end.
		/// </summary>
		/// <param name="data">Data payload to be sent.</param>
		void Send(string data);

		/// <summary>
		/// Send a message to the other end.
		/// </summary>
		/// <param name="message">Message to be sent.</param>
		void Send(Message message);

		/// <summary>
		/// Send data with the expectation of a response from the other end.
		/// </summary>
		/// <param name="data">Data payload to be sent.</param>
		/// <param name="timeout">Timeout milliseconds before the request is automatically cancelled.</param>
		/// <returns>Request object that can be cancelled an provided OnResult/OnError handlers</returns>
		IRequest SendRequest(string data, long timeout = 10_000);

		/// <summary>
		/// Send a message with the expectation of a response from the other end.
		/// </summary>
		/// <param name="message">Message to be sent.</param>
		/// <param name="timeout">Timeout milliseconds before the request is automatically cancelled.</param>
		/// <returns>Request object that can be cancelled an provided OnResult/OnError handlers</returns>
		IRequest SendRequest(Message message, long timeout = 10_000);
	}
}
