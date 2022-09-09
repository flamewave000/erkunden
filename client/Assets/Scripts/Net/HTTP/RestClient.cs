using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Assets.Scripts.Utils;
using Newtonsoft.Json.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Net
{

	[Serializable]
	public class HttpException : Exception
	{
		public long code;
		public string error;
		public HttpException(long code, string error) : base(error) { this.code = code; this.error = error; }
		protected HttpException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
	public struct WebResult
	{
		public long status;
		public string data;
	}
	public interface IRequestBuilder
	{
		IRequestBuilder Url(string value);
		IRequestBuilder Url(Uri value);
		IRequestBuilder Authorize(string accessToken);
		IRequestBuilder Header(string key, string value);
		IRequestBuilder Headers(params Tuple<string, string>[] headers);

		IRequestBuilder SetBody(string content, string contentType);
		IRequestBuilder Json(string content);
		IRequestBuilder Json(Dictionary<string, string> content);
		IRequestBuilder Json(JToken jToken);
		IRequestBuilder Text(string content);
		IRequestBuilder Xml(string content);
		IRequestBuilder Html(string content);

		IDeferred<WebResult> Execute(MonoBehaviour context);
	}

	public static class RestClient
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IRequestBuilder Get(string url) => Get(new Uri(url));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IRequestBuilder Put(string url) => Put(new Uri(url));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IRequestBuilder Post(string url) => Post(new Uri(url));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IRequestBuilder Delete(string url) => Delete(new Uri(url));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IRequestBuilder Head(string url) => Head(new Uri(url));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IRequestBuilder Get(Uri uri) => new RequestBuilder(uri, UnityWebRequest.kHttpVerbGET);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IRequestBuilder Put(Uri uri) => new RequestBuilder(uri, UnityWebRequest.kHttpVerbPUT);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IRequestBuilder Post(Uri uri) => new RequestBuilder(uri, UnityWebRequest.kHttpVerbPOST);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IRequestBuilder Delete(Uri uri) => new RequestBuilder(uri, UnityWebRequest.kHttpVerbDELETE);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IRequestBuilder Head(Uri uri) => new RequestBuilder(uri, UnityWebRequest.kHttpVerbHEAD);

		private class RequestBuilder : IRequestBuilder
		{
			public MutableDeferred<WebResult> promise = new MutableDeferred<WebResult>();
			private Uri uri;
			private string method;
			private UploadHandlerRaw uploadHandler;
			private Dictionary<string, string> headers = new Dictionary<string, string>();

			public RequestBuilder(Uri uri, string method)
			{
				this.uri = uri;
				this.method = method;
			}

			public IRequestBuilder Authorize(string accessToken) => Header("Authorization", "Bearer " + accessToken);
			public IRequestBuilder Header(string key, string value)
			{
				if (headers.ContainsKey(key))
					headers[key] = value;
				else headers.Add(key, value);
				return this;
			}

			public IRequestBuilder Headers(params Tuple<string, string>[] headers)
			{
				foreach (var (key, value) in headers)
				{
					if (this.headers.ContainsKey(key))
						this.headers[key] = value;
					else this.headers.Add(key, value);
				}
				return this;
			}

			public IRequestBuilder SetBody(string content, string contentType)
			{
				uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(content));
				uploadHandler.contentType = contentType;
				return this;
			}

			public IRequestBuilder Json(string content) => SetBody(content, "application/json");
			public IRequestBuilder Json(Dictionary<string, string> content) => Json(JToken.FromObject(content).ToString());
			public IRequestBuilder Json(JToken jToken) => Json(jToken.ToString());
			public IRequestBuilder Text(string content) => SetBody(content, "text/plain");
			public IRequestBuilder Html(string content) => SetBody(content, "text/html");
			public IRequestBuilder Xml(string content) => SetBody(content, "application/xml");

			public IRequestBuilder Url(string value) => Url(new Uri(value));
			public IRequestBuilder Url(Uri value)
			{
				uri = value;
				return this;
			}

			public IDeferred<WebResult> Execute(MonoBehaviour context)
			{
				var request = new UnityWebRequest(uri, method, new DownloadHandlerBuffer(), uploadHandler);
				if (uploadHandler != null)
					request.SetRequestHeader("Content-Type", uploadHandler.contentType);
				foreach (var (key, value) in headers)
				{
					request.SetRequestHeader(key, value);
				}
				context.StartCoroutine(ProcessRequest(request));
				return promise;
			}

			private IEnumerator ProcessRequest(UnityWebRequest request)
			{
				using (request)
				{
					yield return request.SendWebRequest();
					try
					{
						if (request.result == UnityWebRequest.Result.Success)
							promise.Resolve(new()
							{
								status = request.responseCode,
								data = request.downloadedBytes > 0 ? request.downloadHandler.text : null
							});
						else
						{
							if (request.downloadedBytes > 0)
							{
								string text;
								try
								{
									text = (string)JObject.Parse(request.downloadHandler.text).GetValue("error");
								}
								catch
								{
									text = request.downloadHandler.text;
								}
								promise.Reject(new HttpException(request.responseCode, text));
							}
							else
								promise.Reject(new HttpException(request.responseCode, request.error));
						}
					}
					catch (Exception error)
					{
						promise.Reject(error);
					}
				}
			}
		}
	}
}
