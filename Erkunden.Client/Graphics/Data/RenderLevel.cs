using System;

namespace Erkunden.Client.Graphics.Data
{
	[Flags]
	public enum RenderLevel : uint
	{
		SkyBox		= 0x0000_0001,
		Default		= 0x0000_0002,
		UI			= 0x0000_0004,
		WireFrame	= 0x0000_0008,
	}
}
