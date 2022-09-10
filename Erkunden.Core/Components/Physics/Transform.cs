
using OpenTK.Mathematics;

namespace Erkunden.Core.Components.Physics
{
	public struct Transform : ECS.Component
	{
		Vector3 Position;
		Vector3 Scale;
		Quaternion Rotation;
	}
}
