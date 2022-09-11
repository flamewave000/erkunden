using System;
using System.Runtime.CompilerServices;

namespace Erkunden.Core.Util
{
	public static class Log
	{
		private static string Indentation = "";
		public static int IndentLevel => Indentation.Length;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteLine(string message, bool resetAtEnd = true, bool indent = false) => Write(message + '\n', resetAtEnd, indent);
		public static void Write(string message, bool resetAtEnd = true, bool indent = false)
		{
			message = (indent ? "\t" : "") + Indentation + message;
			string[] tokens = message.Split('@', StringSplitOptions.None);
			if (tokens.Length == 1)
			{
				Console.Write(tokens[0]);
				return;
			}
			for (int c = 0; c < tokens.Length; c++)
			{
				if (tokens[c].Length == 0)
				{
					if (c == 0) continue;
					Console.Write('@');
				}
				// If the first token should not be formatted
				else if (c == 0 && message[0] != '@')
					Console.Write(tokens[c]);
				else
				{
					int index = tokens[c].IndexOf(';');
					if (index == -1)
						Console.Write(tokens[c]);
					else
					{
						string[] colours = tokens[c].Substring(0, index).ToLower().Split(':');
						if (colours.Length == 1 && colours[0] == "clr")
							Console.ResetColor();
						else
						{
							Console.ForegroundColor = Enum.Parse<ConsoleColor>(colours[0][0].ToString().ToUpper() + colours[0].Substring(1));
							if (colours.Length == 2)
								Console.BackgroundColor = Enum.Parse<ConsoleColor>(colours[1][0] + colours[1].Substring(1));
						}
						Console.Write(tokens[c].Substring(index + 1));
					}
				}
			}
			if (resetAtEnd) Console.ResetColor();
		}

		public static void Pause(bool intercept = true)
		{
			Console.Write("Press any key to continue...");
			Console.ReadKey(intercept);
		}

		public static void Reset() => Console.ResetColor();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Indent() => (Indentation += '\t').Length;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Dedent() => (Indentation = (Indentation.Length > 0 ? Indentation.Substring(1) : "")).Length;
	}
}
