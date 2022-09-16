using Erkunden.Client.AssetManagement.Materials;
using Erkunden.Client.AssetManagement.Shaders;
using Erkunden.Client.Graphics.Objects;
using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.AssetManagement.Models
{
	public partial class Model : BaseAsset
	{
		public class Part : BaseAsset
		{
			private IndexBuffer? IndexBuffer = null;
			public Material Material;
			public readonly PrimitiveType PrimitiveType;
			public readonly int Count;
			public readonly int Offset;

			public override bool IsDisposed => IndexBuffer?.IsDisposed ?? true;

			private Part(string name, int count, int offset, Material material, PrimitiveType primitiveType) : base(name)
			{
				Material = material;
				PrimitiveType = primitiveType;
				Count = count;
				Offset = offset;
			}

			private Part(string name, uint[] indexes, Material material, PrimitiveType primitiveType) : base(name)
			{
				Material = material;
				IndexBuffer = IndexBuffer.Create(in indexes);
				IndexBuffer.Unbind();
				PrimitiveType = primitiveType;
				Count = indexes.Length;
				Offset = 0;
			}

			public override void Dispose() => IndexBuffer?.Dispose();
			public void Draw(Shader shader)
			{
				Material.Bind(shader);
				if (IndexBuffer != null)
				{
					if (IsDisposed) return;
					IndexBuffer.Bind();
					GL.DrawElements(PrimitiveType, IndexBuffer.Length, DrawElementsType.UnsignedInt, 0);
				}
				else
					GL.DrawArrays(PrimitiveType, Offset, Count);
			}

			#region Factory Methods
			public static Part CreateWithOffset(string name, int count, int offset, Material material, PrimitiveType primitiveType) =>
				new Part(name, count, offset, material, primitiveType);
			public static Part CreateWithElements(string name, uint[] indexes, Material material, PrimitiveType primitiveType) =>
				new Part(name, indexes, material, primitiveType);
			#endregion
		}
	}
}
