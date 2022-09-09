using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Assets.Scripts.Net;
using Assets.Scripts.Utils;
using UnityEngine;
using WebSocketSharp;

public class SocketManager : MonoBehaviour, IChannel
{
	private Dictionary<uint, Request> requests = new Dictionary<uint, Request>();
	private uint lastRequestId = 0;
	private CancellationTokenSource requestTimeoutTask = null;

	public WebSocket socket { get; private set; }

	public string id { get; private set; }

	public event Action<Message> OnMessage;
	public event Action<CloseEventArgs> OnClose;

	// Start is called before the first frame update
	private void Start()
	{
		socket = new WebSocket("ws://localhost:8080");
		socket.OnMessage += HandleMessage;
		socket.OnClose += HandleClose;
		socket.OnError += HandleError;
		socket.OnOpen += HandleOpen;
		socket.Connect();
		requestTimeoutTask = new CancellationTokenSource();
		StartCoroutine(TimeoutProcessor(requestTimeoutTask));
	}

	private void OnDestroy()
	{
		// Close socket when exiting application
		socket.Close();
		requestTimeoutTask.Cancel();
	}

	// WebSocket onMessage function
	private void HandleMessage(object sender, MessageEventArgs e)
	{
		if (!e.IsText)
		{
			Debug.LogWarning("Recieved Non-Text Message");
			return;
		}

		var message = new Message(e.Data);
		if (message.isRequest)
		{
			Request request;
			if (requests.TryGetValue(message.requestID.Value, out request))
				request.promise.Resolve(message);
		}
		else if (message.Is(Header.BindClientID))
			id = message.AsString();
		else
			OnMessage?.Invoke(message);
	}
	// If server connection closes (not client originated)
	private void HandleClose(object sender, CloseEventArgs e)
	{
		Debug.Log(e.Code);
		Debug.Log(e.Reason);
		Debug.Log("Connection Closed!");
		OnClose?.Invoke(e);
	}

	private void HandleError(object sender, ErrorEventArgs e)
	{
		Debug.LogError(e);
	}

	private void HandleOpen(object sender, EventArgs e)
	{
		Debug.Log("Connection Opened!");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Send(string data) => socket.Send(data);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Send(Message message) => socket.Send(message.Pack());

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IRequest SendRequest(Message message, long timeout = 10_000) => SendRequest(message.Pack(), timeout);
	public IRequest SendRequest(string data, long timeout = 10_000)
	{
		if (lastRequestId == uint.MaxValue) lastRequestId = 0;
		var request = new Request(++lastRequestId, new MutableDeferred<Message>(), UnixTime.now(), timeout, CancelRequest);
		requests.Add(request.id, request);
		socket.Send(data);
		return request;
	}

	private IRequest CancelRequest(uint requestID, Exception reason)
	{
		Request request;
		requests.Remove(requestID, out request);
		request.promise.Reject(reason);
		return request;
	}

	private IEnumerator TimeoutProcessor(CancellationTokenSource cancelToken)
	{
		long now;
		var delay = new WaitForSeconds(0.5f);
		while (!cancelToken.IsCancellationRequested)
		{
			now = UnixTime.now();
			foreach (var request in requests.Values.ToList())
			{
				if (request.timestamp + request.timeout < now) continue;
				CancelRequest(request.id, new TimeoutException());
			}
			yield return delay;
		}
	}

}
