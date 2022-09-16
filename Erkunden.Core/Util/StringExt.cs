using System.Text.RegularExpressions;

namespace Erkunden.Core.Util
{
	public static class StringExt
	{
		private static Regex SpaceRedux = new Regex("(\t|[ ]{2,})", RegexOptions.Compiled);

		public static int ByteSize(this string value) => value.Length * sizeof(char);

		public static string ReduceSpaces(this string value, string substitute = " ") => SpaceRedux.Replace(value, substitute);
	}
}
