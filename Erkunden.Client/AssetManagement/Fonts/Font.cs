using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Erkunden.Client.AssetManagement.Fonts.Blocks;
using Erkunden.Client.AssetManagement.Textures;
using Erkunden.Client.Utils;
using OpenTK.Mathematics;

namespace Erkunden.Client.AssetManagement.Fonts
{
	public class Font : BaseAsset
	{
		private Dictionary<int, ScaledCharBlock[]> sizeCache = new Dictionary<int, ScaledCharBlock[]>();
		private Dictionary<uint, int> characters = new Dictionary<uint, int>();
		private Dictionary<uint, Dictionary<uint, short>> kernings = new Dictionary<uint, Dictionary<uint, short>>();

		public readonly Texture[] Pages;

		public readonly FontMeta Meta;
		public readonly FontBody Body;

		public override bool IsDisposed => Pages[0].IsDisposed;

		public CharBlock this[uint i] => Body.Chars[characters[i]];

		public Font(string name, FontMeta meta, FontBody body, Texture[] pages) : base(name)
		{
			Pages = pages;
			Meta = meta;
			Body = body;
			for (int c = 0; c < body.Chars.Length; c++)
			{
				characters[body.Chars[c].Id] = c;
			}
			foreach (var kerning in body.Kernings)
			{
				if (!kernings.TryGetValue(kerning.First, out var value))
					kernings.Add(kerning.First, value = new Dictionary<uint, short>());
				value[kerning.Second] = kerning.Amount;
			}
		}

		public void DrawText(Vector2 position, string text, int fontSize, Color4 color, int wrapAt = 0)
		{
			float scale = fontSize / (float)Meta.Info.FontSize;
			float lineHeight = Body.Common.LineHeight * scale;
			var chars = GetScaledChars(fontSize, scale);
			var cursor = position;

			float kern;
			ScaledCharBlock block;
			for (int c = 0; c < text.Length; c++)
			{
				// Ignore Carriage Returns
				if (text[c] == '\r') continue;
				// If we have hit a New Line, push down
				if (text[c] == '\n')
				{
					cursor.Y += lineHeight;
					cursor.X = position.X;
					continue;
				}
				if (text[c] == '\t')
				{
					cursor.X += chars[characters[' ']].XAdvance * 4;
					continue;
				}
				// If the character is unknown, use Invalid Glyph
				if (!characters.ContainsKey(text[c]))
					block = chars[characters[uint.MaxValue]];
				// Otherwise fetch the regular Glyph
				else block = chars[characters[text[c]]];

				// Retrieve any kerning rule for the current glyph pair
				kern = (c > 0 ? CalcKerning(text[c], text[c - 1]) : 0) * scale;

				// Draw the glyph at the calculated position
				DrawGlyph(cursor, block, color);

				// After drawing glyph, Push forward to next character
				cursor.X += kern + block.XAdvance;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DrawGlyph(Vector2 position, ScaledCharBlock glyph, Color4 color)
		{
			SpriteBatch.QueueSprite(
				texture: Pages[glyph.Page],
				tint: color,
				rotation: 0,
				rotationOrigin: Vector2.Zero,
				position: position + glyph.DrawOffset,
				size: glyph.DrawSize,
				textureOffset: glyph.GlyphOffset,
				textureSize: glyph.GlyphSize,
				transpose: false,
				transformSpriteTexCoord: false);
		}

		public ScaledCharBlock[] GetScaledChars(int fontSize, float scale)
		{
			if (sizeCache.TryGetValue(fontSize, out var chars)) return chars;

			Vector2[] scales = new Vector2[Pages.Length];
			for (int c = 0; c < Pages.Length; c++)
			{
				scales[c] = new Vector2(1f / Pages[c].Texture2D.Width, 1f / Pages[c].Texture2D.Height);
			}

			chars = new ScaledCharBlock[Body.Chars.Length];
			for (int c = 0; c < chars.Length; c++)
			{
				chars[c] = new ScaledCharBlock
				{
					Id = (char)(Body.Chars[c].Id),
					// Create a vector for glyph location and scale it to texture coordinates (0,1)
					GlyphOffset = new Vector2(Body.Chars[c].X, Body.Chars[c].Y) * scales[Body.Chars[c].Page],
					GlyphSize = new Vector2(Body.Chars[c].Width, Body.Chars[c].Height) * scales[Body.Chars[c].Page],
					// Create a draw offset and size that represent the glyph in screenspace.
					// Scale it to the requested font size as the reciprical of the font's natural size
					DrawOffset = new Vector2(Body.Chars[c].XOffset, Body.Chars[c].YOffset) * scale,
					DrawSize = new Vector2(Body.Chars[c].Width, Body.Chars[c].Height) * scale,
					// Also scale the x-advance to the requested font size
					XAdvance = Body.Chars[c].XAdvance * scale,
					// The page and channel indicators are unmodified
					Page = Body.Chars[c].Page,
					Chnl = Body.Chars[c].Chnl
				};
				// Invert the GlyphOffset.Y and GlyphSize.Y to bring it from screen coord to texture coord
				chars[c].GlyphOffset.Y = (1f - chars[c].GlyphOffset.Y) - chars[c].GlyphSize.Y;
			}

			sizeCache[fontSize] = chars;
			return chars;
		}

		public Vector2i MeasureString(string text, int wrapAt = 0)
		{
			return wrapAt > 0
				? MeasureTextWrapped(text, wrapAt)
				: MeasureText(text);
		}

		private Vector2i MeasureText(string text)
		{
			Vector2i size = new Vector2i(0, Body.Common.LineHeight);
			int currX = 0;
			int kern;
			// For the last character on a line, we want to track the previous measured character's right most point
			int lastExtraBit = 0;
			CharBlock block;
			for (int c = 0; c < text.Length; c++)
			{
				if (text[c] == '\r') continue;
				if (!characters.ContainsKey(text[c]))
					block = this[uint.MaxValue];
				else if (text[c] == '\n')
				{
					size.Y += Body.Common.LineHeight;
					size.X = Math.Max(size.X, currX) + lastExtraBit;
					currX = 0;
					continue;
				}
				else block = this[text[c]];

				kern = c > 0 ? CalcKerning(text[c], text[c - 1]) : 0;

				currX = kern + block.XAdvance;
				if (c == 0) currX += block.XOffset;
				lastExtraBit = (block.XOffset + block.Width) + block.XAdvance;
			}
			size.X = Math.Max(size.X, currX) + lastExtraBit;
			return size;
		}

		private Vector2i MeasureTextWrapped(string text, int wrapAt)
		{
			if (text.Length == 0) return Vector2i.Zero;

			string[] lines = text.Split('\n');
			if (lines.Length == 0) return Vector2i.Zero;

			List<string> words = new List<string>();
			words.Add(lines[0]);
			lines.Aggregate(words, (acc, el) =>
			{
				acc.Add("\n");
				acc.AddRange(el.Split(' '));
				return acc;
			});

			Vector2i size = new Vector2i(0, Body.Common.LineHeight);
			Vector2i wordSize;
			int currX = 0;
			foreach (var word in words)
			{
				if (word == "\n")
				{
					size.Y += Body.Common.LineHeight;
					if (currX > size.X)
						continue;
				}
				wordSize = MeasureText(word);
				if (wordSize.X + currX <= wrapAt)
					currX += wordSize.X;
			}
			return size;
		}

		private int CalcKerning(uint first, uint second)
		{
			if (!kernings.TryGetValue(first, out var group)) return 0;
			if (!group.TryGetValue(second, out var amount)) return 0;
			return amount;
		}

		public override void Dispose()
		{
			foreach (var sprite in Pages)
				sprite.Dispose();
		}
	}
}
