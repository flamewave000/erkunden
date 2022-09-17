using System;
using System.Runtime.CompilerServices;
using Erkunden.Client.AssetManagement;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.AssetManagement.Textures;
using Erkunden.Client.Graphics.Objects;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

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
		public SpriteVertex BL0;
		public SpriteVertex TL0;
		public SpriteVertex TR0;
		public SpriteVertex TR1;
		public SpriteVertex BR0;
		public SpriteVertex BL1;
		public static readonly int SizeInBytes = Unsafe.SizeOf<SpritePlane>();
		public static readonly Vector4 TLVec = new Vector4(0, 0, 0, 1);
		public static readonly Vector4 TRVec = new Vector4(1, 0, 0, 1);
		public static readonly Vector4 BLVec = new Vector4(0, -1, 0, 1);
		public static readonly Vector4 BRVec = new Vector4(1, -1, 0, 1);
	}

	struct SpriteMap
	{
		public Texture Texture;
		public Color4 Tint;
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
		private static Shader SpriteShader = null!;
		private static Matrix4 Projection = Matrix4.Identity;
		private static Matrix4 ProjectionOffset = Matrix4.CreateTranslation(-1f, 1f, 0f);

		private static int PrevSpriteCount = 0;
		private static int SpriteCount = 0;
		private static int MapIndex;
		private static SpritePlane[] Sprites = new SpritePlane[BUFFER_SIZE];
		private static SpriteMap[] SpriteMaps = new SpriteMap[BUFFER_SIZE];
		#endregion

		#region Properties
		public static bool IsDisposed => SpriteCount < 0;
		public static int Count => SpriteCount;
		public static int Capacity { get => Sprites.Length; set => Resize(value); }
		#endregion

		public static void Initialize(NativeWindow window)
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
			SpriteShader = AssetProvider.Get<Shader>("Sprite");
			SpriteShader.Compile();

			window.Resize += OnWindowResize;
			OnWindowResize(new OpenTK.Windowing.Common.ResizeEventArgs(window.ClientSize));
		}

		private static void OnWindowResize(OpenTK.Windowing.Common.ResizeEventArgs obj)
		{
			Matrix4.CreateOrthographic(obj.Width, obj.Height, -1, 1, out Projection);
			Matrix4.Mult(in Projection, in ProjectionOffset, out Projection);
		}

		public static void DrawBatchedSprites(Vector2i clientSize)
		{
			if (SpriteCount == 0) return;
			SpriteShader.Use();
			VertexArrayObject.Bind();
			VertexBuffer.Bind();

			// If the Sprite Capacity is different than the VBO, resize the VBO
			if (Capacity != VertexBufferCapacity)
			{
				VertexBuffer.SetData(Sprites, Capacity, SpritePlane.SizeInBytes, BufferUsageHint.DynamicDraw);
				VertexBufferCapacity = Capacity;
			}
			else
				VertexBuffer.SetSubData(Sprites, SpriteCount, SpritePlane.SizeInBytes);
			PrevSpriteCount = SpriteCount;
			SpriteCount = 0;
			MapIndex = 0;

			// Iterate over and draw all sprites in groups of 16
			for (int sprite = 0, map; sprite < PrevSpriteCount; sprite += map)
			{
				// Bind the next group of sprite maps, stops at min(sprite + 16, PrevSpriteCount)
				for (map = 0; map < 16 && map + sprite < PrevSpriteCount; map++)
				{
					SpriteShader.SetBool("u_SingleChannel" + map, SpriteMaps[sprite + map].Texture.Texture2D.Format == PixelInternalFormat.R8);
					SpriteShader.SetColor4("u_Tint" + map, SpriteMaps[sprite + map].Tint);
					SpriteShader.SetTexture("u_Texture" + map, (TextureUnit)TEXTURE0 + map, ref SpriteMaps[sprite + map].Texture);
				}
				GL.DrawArrays(PrimitiveType.Triangles, sprite * 6, map * 6);
			}

			// Reduce buffer size by 25% if we used less than 75% of the available sprites
			int newBuffer = (int)(Sprites.Length / BUFFER_SIZE * 0.75) * BUFFER_SIZE;
			if (PrevSpriteCount <= newBuffer)
				Resize(newBuffer);
		}

		/// <summary>
		/// Queues a sprite to be drawn in the next batch during the next Render Frame.
		/// </summary>
		public static void QueueSprite(Texture texture, Color4 tint,
			float rotation, in Vector2 rotationOrigin,
			in Vector2 position, in Vector2 size,
			Vector2 textureOffset, Vector2 textureSize,
			bool transpose, bool transformSpriteTexCoord)
		{
			int spriteIdx = SpriteCount++;
			int mapIdx = MapIndex++;
			MapIndex %= MAX_TEXTURE_COUNT;

			// Create a vec4 and invert the Y to go from world space to screen space
			Vector4 pos = new Vector4(position.X, -position.Y, 0, 1);
			Vector3 rotOrigin = new Vector3(size * rotationOrigin);
			Matrix4 matrix =
				Matrix4.CreateTranslation(-rotOrigin) *
				Matrix4.CreateRotationZ(rotation) *
				Matrix4.CreateTranslation(rotOrigin) *
				Matrix4.CreateScale(size.X, size.Y, 1) *
				Matrix4.CreateTranslation(pos.Xyz) *
				Projection;

			// Transform sprite location to texture coordinates
			if (transformSpriteTexCoord)
			{
				var scale = new Vector2(1f / texture.Texture2D.Width, 1f / texture.Texture2D.Height);
				textureOffset *= scale;
				textureSize *= scale;
			}

			ref var sprite = ref Sprites[spriteIdx];

			sprite.BL0.Position = Vector4.TransformRow(SpritePlane.BLVec, matrix).Xy;
			sprite.TL0.Position = Vector4.TransformRow(SpritePlane.TLVec, matrix).Xy;
			sprite.TR0.Position = Vector4.TransformRow(SpritePlane.TRVec, matrix).Xy;
			sprite.BR0.Position = Vector4.TransformRow(SpritePlane.BRVec, matrix).Xy;

			sprite.BL0.TexCoord = textureOffset;
			sprite.TL0.TexCoord = new Vector2(textureOffset.X, textureOffset.Y + textureSize.Y);
			sprite.TR0.TexCoord = textureOffset + textureSize;
			sprite.BR0.TexCoord = new Vector2(textureOffset.X + textureSize.X, textureOffset.Y);

			if (transpose)
			{
				float top = 1 - sprite.TL0.TexCoord.Y;
				float bot = 1 - sprite.BL0.TexCoord.Y;
				sprite.BL0.TexCoord.Y = top;
				sprite.TL0.TexCoord.Y = bot;
				sprite.TR0.TexCoord.Y = bot;
				sprite.BR0.TexCoord.Y = top;
			}

			sprite.BL0.TexIdx = sprite.TL0.TexIdx = sprite.TR0.TexIdx = sprite.BR0.TexIdx = mapIdx;

			sprite.BL1 = sprite.BL0;
			sprite.TR1 = sprite.TR0;

			SpriteMaps[spriteIdx] = new SpriteMap { Texture = texture, Tint = tint };
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
