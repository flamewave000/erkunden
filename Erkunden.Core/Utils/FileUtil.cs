using System.IO;

namespace Erkunden.Core.Utils
{
	public static class FileUtil
	{
		public static string GetParentDirectory(string path)
		{
			var name = new FileInfo(path).Name;
			return PlatformPath(path.Remove(path.Length - name.Length, name.Length));
		}
		public static string PlatformPath(string path) => Path.Combine(path.Contains('/') ? path.Split('/') : path.Split('\\'));
	}
}
