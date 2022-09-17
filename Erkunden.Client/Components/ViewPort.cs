using System.Runtime.Serialization;
using Erkunden.ECS;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Erkunden.Client.Components
{
	[DataContract]
	public class ViewPort : IComponent
	{
		public enum ViewType
		{
			Absolute,
			Relative
		}
		[DataMember(Name = "t")]
		public ViewType Type = ViewType.Absolute;

		[DataMember(Name = "ax")]
		public int AbsoluteX;
		[DataMember(Name = "ay")]
		public int AbsoluteY;
		[DataMember(Name = "aw")]
		public int AbsoluteWidth;
		[DataMember(Name = "ah")]
		public int AbsoluteHeight;

		public int AbsoluteLeft
		{
			get => AbsoluteX;
			set => AbsoluteX = value;
		}
		public int AbsoluteTop
		{
			get => AbsoluteY;
			set => AbsoluteY = value;
		}
		public int AbsoluteRight
		{
			get => AbsoluteX + AbsoluteWidth;
			set => AbsoluteWidth = value - AbsoluteX;
		}
		public int AbsoluteBottom
		{
			get => AbsoluteY + AbsoluteHeight;
			set => AbsoluteHeight = value - AbsoluteY;
		}

		[DataMember(Name = "rx")]
		public float RelativeX;
		[DataMember(Name = "ry")]
		public float RelativeY;
		[DataMember(Name = "rw")]
		public float RelativeWidth;
		[DataMember(Name = "rh")]
		public float RelativeHeight;

		public float RelativeLeft
		{
			get => RelativeX;
			set => RelativeX = value;
		}
		public float RelativeTop
		{
			get => RelativeY;
			set => RelativeY = value;
		}
		public float RelativeRight
		{
			get => RelativeX + RelativeWidth;
			set => RelativeWidth = value - RelativeX;
		}
		public float RelativeBottom
		{
			get => RelativeY + RelativeHeight;
			set => RelativeHeight = value - RelativeY;
		}

		public Vector2i Size => new Vector2i(AbsoluteWidth, AbsoluteHeight);
		public Vector2 SizeF => new Vector2(AbsoluteWidth, AbsoluteHeight);

		public void Bind(Vector2i clientSize)
		{
			if (clientSize.X == 0 || clientSize.Y == 0) return;
			if (Type == ViewType.Absolute)
			{
				RelativeX = AbsoluteX / clientSize.X;
				RelativeY = AbsoluteY / clientSize.Y;
				RelativeWidth = AbsoluteWidth / clientSize.X;
				RelativeHeight = AbsoluteHeight / clientSize.Y;
			}
			else
			{
				AbsoluteX = (int)(clientSize.X * RelativeX);
				AbsoluteY = (int)(clientSize.Y * RelativeY);
				AbsoluteWidth = (int)(clientSize.X * RelativeWidth);
				AbsoluteHeight = (int)(clientSize.Y * RelativeHeight);
			}
			GL.Viewport(AbsoluteX, AbsoluteY, AbsoluteWidth, AbsoluteHeight);
		}
	}
}
