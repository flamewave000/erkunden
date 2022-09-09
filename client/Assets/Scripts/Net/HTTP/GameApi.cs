using System;
using Assets.Scripts.Net.HTTP.Data;
using Assets.Scripts.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;

using Map = System.Collections.Generic.Dictionary<string, string>;

namespace Assets.Scripts.Net.HTTP
{
	public class GameApi : MonoBehaviour
	{
		public static string accessToken { get; private set; } = null;
		public static bool isAuthenticated => accessToken != null;
		public const string URL = "http://localhost:8080/api/v1";

		public IDeferred<string> Login(string username, string password) => RestClient
				.Post(URL + "/auth")
				.Json(JToken.FromObject(new AuthLogin
				{
					user = username,
					pass = password
				}))
				.Execute(this)
				.OnResult(result =>
				{
					accessToken = JsonUtility.FromJson<AccessToken>(result.data).token;
					return accessToken;
				});

		public IDeferred<bool> CheckAccountExists(string username) => RestClient
			.Head(URL + "/account/" + username)
			.Execute(this)
			.OnResult(x => true);

		public IDeferred<AccountInfo> GetAccount(string username) => RestClient
			.Get(URL + "/account/" + username)
			.Authorize(accessToken)
			.Execute(this)
			.OnResult(x => JsonUtility.FromJson<AccountInfo>(x.data));

		public IDeferred<string> CreateAccount(string username, string password) => RestClient
			.Post(URL + "/account")
			.Json(new Map { { "user", username }, { "pass", password } })
			.Execute(this)
			.OnResult(x => (string)JObject.Parse(x.data).GetValue("user_id"));
	}
}
