using Newtonsoft.Json.Linq;

namespace Assets.Scripts.Net
{
	public interface INetSynced
	{
		/// <summary>
		/// Save state to the provided target.
		/// </summary>
		/// <param name="target">Target JObject to save the state into.</param>
		public void SaveData(JObject target);

		/// <summary>
		/// Load state from the provided source.
		/// </summary>
		/// <param name="source">Source JObject to load the state from.</param>
		public void LoadData(JObject source);

		/// <summary>
		/// Publish this object's state to the provided channel.
		/// </summary>
		/// <param name="channel">Channel to receive the state data.</param>
		public void Publish(IChannel channel);
	}
}
