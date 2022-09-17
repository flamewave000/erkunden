using System;
using System.Collections.Generic;
using Erkunden.Client.AssetManagement;
using Erkunden.Client.AssetManagement.Fonts;
using OpenTK.Mathematics;

namespace Erkunden.Client.Utils
{
	public class FontNotFoundException : Exception { }
	public static class SpriteFont
	{
		private const int LargeText = 48;
		private const int MediumText = 24;
		private const int SmallText = 12;

		private const int LargeTextLowerBound = (int)(LargeText * 0.75);
		private const int MediumTextLowerBound = (int)(MediumText * 0.75);

		private static Dictionary<string, Font> Fonts = new Dictionary<string, Font>();

		private static bool LoadFont(string fontName)
		{
			if (Fonts.ContainsKey(fontName))
				return true;
			Font font = null!;
			if (!AssetProvider.TryGet(fontName, ref font))
				return false;
			Fonts[fontName] = font;
			return true;
		}

		public static void DrawText(string fontName, int fontSize, Vector2 position, string text, Color4 color, int wrapAt = 0)
		{
			int baseFontSize = SmallText;
			if (fontSize >= LargeTextLowerBound)
				baseFontSize = LargeText;
			else if (fontSize >= MediumTextLowerBound)
				baseFontSize = MediumText;

			fontName += baseFontSize.ToString();

			if (!Fonts.TryGetValue(fontName, out var font))
			{
				if (LoadFont(fontName)) font = Fonts[fontName];
				else throw new FontNotFoundException();
			}
			font.DrawText(position, text, fontSize, color, wrapAt);
		}
	}
}
