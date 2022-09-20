using System;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.Graphics.Objects;
using Erkunden.Client.Lights;
using Erkunden.Client.Utils;
using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.Components
{
	public class LightCollection : UnsafeArrayList<Light>, IDisposable
	{
		private VertexBuffer LightsBuffer = VertexBuffer.Create(BufferTarget.ShaderStorageBuffer);

		public void PushLightsToBuffer(Shader shader)
		{
			int index = GL.GetProgramResourceIndex(shader.Program.Handle, ProgramInterface.ShaderStorageBlock, "LightsBuffer");
			if (index < 0)
				return;
			LightsBuffer.Bind();
			GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, index, LightsBuffer.Handle);

			byte[] data;
			LightData[] lights;
			if (Capacity < LightsBuffer.Capacity)
			{
				data = new byte[16 + LightData.SizeInBytes * Capacity];
				lights = new LightData[Capacity];
			}
			else
			{
				data = new byte[16 + LightData.SizeInBytes * Count];
				lights = new LightData[Count];
			}
			for (int c = 0; c < Count; c++)
			{
				lights[c] = this[c].AsLightData();
			}
			unsafe
			{
				fixed (byte* dataPtr = data)
				{
					*((int*)dataPtr) = Count;
					fixed (LightData* lightsPtr = lights)
					{
						int dataSize = LightData.SizeInBytes * lights.Length;
						System.Buffer.MemoryCopy(lightsPtr, dataPtr + 16, dataSize, dataSize);
					}
				}
			}
			if (Capacity < LightsBuffer.Capacity || LightsBuffer.Capacity == 0)
				LightsBuffer.SetData(data, data.Length, BufferUsageHint.DynamicDraw);
			else
				LightsBuffer.SetSubData(data, data.Length);
		}

		public void Dispose()
		{
			LightsBuffer.Dispose();
		}
	}
}
