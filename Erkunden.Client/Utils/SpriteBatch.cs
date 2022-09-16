using System;
using System.Runtime.CompilerServices;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.AssetManagement.Textures;
using Erkunden.Client.Graphics.Objects;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Erkunden.Client.Utils
{
	struct SpriteVertex
	{
		public Vector2 Position;
		public Vector2 TexCoord;
		public int TexIdx;
		public static readonly int Stride = Unsafe.SizeOf<SpriteVertex>();
	}

	struct SpritePlane
	{
		public SpriteVertex TL;
		public SpriteVertex TR;
		public SpriteVertex BR;
		public SpriteVertex BL;

		public void Apply(in Box2 vertex, in Box2 texture, int index)
		{
			TL.Position = vertex.Min;
			TL.TexCoord = new Vector2(texture.Min.X, texture.Max.Y);
			TL.TexIdx = index;

			TR.Position = new Vector2(vertex.Max.X, vertex.Min.Y);
			TR.TexCoord = texture.Max;
			TR.TexIdx = index;

			BL.Position = new Vector2(vertex.Min.X, vertex.Max.Y);
			BL.TexCoord = texture.Min;
			BL.TexIdx = index;

			BR.Position = vertex.Max;
			BR.TexCoord = new Vector2(texture.Max.X, texture.Min.Y);
			BR.TexIdx = index;
		}

		public static readonly int SizeInBytes = Unsafe.SizeOf<SpritePlane>();
	}


	public static class SpriteBatch
	{
		#region Fields
		private const int BUFFER_SIZE = 64;
		// This is the minimum texture bind limit that is gaurenteed to be available in a GLSL shader
		private const int MAX_TEXTURE_COUNT = 16;
		private const int TEXTURE0 = (int)TextureUnit.Texture0;

		private static VertexArrayObject VertexArrayObject = null!;
		private static VertexBuffer VertexBuffer = null!;
		private static int VertexBufferCapacity = 0;

		private static int PrevSpriteCount = 0;
		private static int SpriteCount = 0;
		private static int MapIndex;
		private static SpritePlane[] Sprites = new SpritePlane[BUFFER_SIZE];
		private static Texture[] SpriteMaps = new Texture[BUFFER_SIZE];
		#endregion

		#region Properties
		public static bool IsDisposed => SpriteCount < 0;
		public static int Count => SpriteCount;
		public static int Capacity { get => Sprites.Length; set => Resize(value); }
		#endregion

		public static void Initialize()
		{
			VertexArrayObject = VertexArrayObject.Create();
			VertexArrayObject.Bind();
			VertexBuffer = VertexBuffer.Create(BufferTarget.ArrayBuffer);
			VertexBuffer.Bind();
			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, SpriteVertex.Stride, 0);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, SpriteVertex.Stride, Vector2.SizeInBytes);
			GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, SpriteVertex.Stride, Vector2.SizeInBytes * 2);
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			GL.EnableVertexAttribArray(2);
			VertexBuffer.Unbind();
			VertexArrayObject.Unbind();
		}

		public static void DrawBatchedSprites()
		{
			if (SpriteCount == 0) return;
			VertexArrayObject.Bind();
			VertexBuffer.Bind();

			// If the Sprite Capacity is different than the VBO, resize the VBO
			if (Capacity != VertexBufferCapacity)
			{
				VertexBuffer.SetData(Sprites, SpriteCount, SpritePlane.SizeInBytes, BufferUsageHint.DynamicDraw);
				VertexBufferCapacity = Capacity;
			}
			else
				VertexBuffer.SetSubData(Sprites, SpriteCount, SpritePlane.SizeInBytes);
			PrevSpriteCount = SpriteCount;
			SpriteCount = 0;
			MapIndex = 0;

			// Iterate over and draw all sprites in groups of 16
			for (int sprite = 0, map = 0; sprite < PrevSpriteCount; sprite += map)
			{
				// Bind the next group of sprite maps, stops at min(sprite + 16, PrevSpriteCount)
				for (map = 0; map < 16 && map + sprite < PrevSpriteCount; map++)
				{
					SpriteMaps[sprite + map].Bind((TextureUnit)TEXTURE0 + map);
				}
				GL.DrawArrays(PrimitiveType.TriangleFan, sprite, map);
			}

			// Reduce buffer size by 25% if we used less than 75% of the available sprites
			int newBuffer = (int)(Sprites.Length / BUFFER_SIZE * 0.75) * BUFFER_SIZE;
			if (PrevSpriteCount <= newBuffer)
				Resize(newBuffer);
		}

		/// <summary>
		/// Queues a sprite to be drawn in the next batch during the next Render Frame.
		/// </summary>
		/// <param name="texture">Texture for the sprite that is to be drawn.</param>
		/// <param name="screenLocation">Bounds on the 2D screen where the sprite is to be drawn. Note: (0,0) is the top left corner.</param>
		/// <param name="spriteLocation">Bounds on the <see cref="texture"/> where the sprite exists.</param>
		/// <param name="transpose">If the <see cref="spriteLocation"/> uses the top-left of the image as (0,0) and needs to be transposed.</param>
		public static void QueueSprite(Texture texture, in Box2 screenLocation, Box2 spriteLocation, bool transpose, bool transformSpriteTextCoord)
		{
			int spriteIdx = SpriteCount++;
			int mapIdx = MapIndex;
			MapIndex = MapIndex++ % MAX_TEXTURE_COUNT;

			// Transform sprite location to texture coordinates
			if (transformSpriteTextCoord)
			{
				var scale = new Vector2(1 / texture.Texture2D.Width, 1 / texture.Texture2D.Height);
				spriteLocation.Min *= scale;
				spriteLocation.Max *= scale;
			}
			if (transpose)
			{
				spriteLocation.Min = new Vector2(spriteLocation.Min.X, 1 - spriteLocation.Min.Y);
				spriteLocation.Max = new Vector2(spriteLocation.Max.X, 1 - spriteLocation.Max.Y);
			}

			Sprites[spriteIdx].Apply(in screenLocation, spriteLocation, mapIdx);
			SpriteMaps[spriteIdx] = texture;
			if (SpriteCount == Sprites.Length)
				Resize(Sprites.Length + BUFFER_SIZE);
		}

		public static void Dispose()
		{
			VertexBuffer.Dispose();
			VertexArrayObject.Dispose();
			SpriteCount = -1;
			Resize(0);
		}

		private static void Resize(int size)
		{
			if (size == SpriteCount) return;
			if (size < SpriteCount) throw new ArgumentOutOfRangeException(nameof(size), "Capacity  must be equal to or more than the current element count.");
			Array.Resize(ref Sprites, size);
			Array.Resize(ref SpriteMaps, size);
		}
	}
}
