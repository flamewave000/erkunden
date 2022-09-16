using System.Runtime.Serialization;
using OpenTK.Mathematics;

namespace Erkunden.Core.Data
{
	[DataContract]
	public struct Bounds
	{
		[DataMember(Name = "l")]
		public int Left;
		[DataMember(Name = "t")]
		public int Top;
		[DataMember(Name = "r")]
		public int Right;
		[DataMember(Name = "b")]
		public int Bottom;

		public int Width { get => Right - Left; set => Right = value - Left; }
		public int Height { get => Bottom - Top; set => Bottom = value - Top; }
		public Box2i Box2i => new Box2i(Left, Top, Right, Bottom);

		public Bounds(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public static Bounds Mult(Bounds left, in int right) => new Bounds()
		{
			Left = left.Left * right,
			Top = left.Top * right,
			Right = left.Right * right,
			Bottom = left.Bottom * right
		};
		public static Bounds Mult(Bounds left, in Vector2i right) => new Bounds()
		{
			Left = left.Left * right.X,
			Top = left.Top * right.X,
			Right = left.Right * right.Y,
			Bottom = left.Bottom * right.Y
		};
		public static Bounds Mult(Bounds left, in Vector4i right) => new Bounds()
		{
			Left = left.Left * right.X,
			Top = left.Top * right.Y,
			Right = left.Right * right.Z,
			Bottom = left.Bottom * right.W
		};
		public static Bounds operator *(Bounds left, in Bounds right) => new Bounds()
		{
			Left = left.Left * right.Left,
			Top = left.Top * right.Top,
			Right = left.Right * right.Right,
			Bottom = left.Bottom * right.Bottom
		};

		public void Mult(in int right)
		{
			Left *= right;
			Top *= right;
			Right *= right;
			Bottom *= right;
		}
		public void Mult(in Vector2i right)
		{
			Left *= right.X;
			Top *= right.Y;
			Right *= right.X;
			Bottom *= right.Y;
		}
		public void Mult(in Vector4i right)
		{
			Left *= right.X;
			Top *= right.Y;
			Right *= right.Z;
			Bottom *= right.W;
		}
		public void Mult(Bounds right)
		{
			Left *= right.Left;
			Top *= right.Top;
			Right *= right.Right;
			Bottom *= right.Bottom;
		}

		public static readonly Bounds Zero = new Bounds(0, 0, 0, 0);
	}
}
