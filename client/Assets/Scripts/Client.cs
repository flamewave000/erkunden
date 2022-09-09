using Assets.Scripts.Net;
using Assets.Scripts.Objects.Mobiles;
using UnityEngine;

namespace Assets.Scripts
{
	public class Client : Player
	{
		private bool authenticated = false;
		public SocketManager socketManager;

		protected override void Start()
		{
			base.Start();

			var auth = Message.Create(Header.Authenticate)
				.Data("abc123")
				.Message();
			socketManager.SendRequest(auth)
				.OnResult(result =>
				{
					authenticated = true;
					switch ((ushort)result.header)
					{
						case Header.PlayerUpdate:
							LoadData(result.AsObject());
							break;
					}
				})
				.OnError(error =>
				{
					Debug.LogError(error);
					handleNetworkFailure();
				});
		}

		protected override void Update()
		{
			base.Update();
			rigidBody.velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		}

		private void handleNetworkFailure()
		{

		}
	}
}
