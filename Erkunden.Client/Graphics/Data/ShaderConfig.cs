using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.Graphics.Data
{
	public struct ShaderConfig
	{
		public bool DepthTest;
		public bool Blend;

		public CullFaceMode CullMode;
		public FrontFaceDirection FaceDirection;
		public PolygonMode PolygonMode;

		public BlendingFactorSrc SrcColorBlend;
		public BlendingFactorDest DstColorBlend;
		public BlendingFactorSrc SrcAlphaBlend;
		public BlendingFactorDest DstAlphaBlend;

		public string AttributeVertex;
		public string AttributeTexCoord;
		public string AttributeNormal;

		public string MatrixModel;
		public string MatrixModelNormal;
		public string MatrixView;
		public string MatrixProjection;

		public string TextureAmbient;
		public string TextureDiffuse;
		public string TextureSpecular;
		public string TextureNormal;

		public string ColorAmbient;
		public string ColorDiffuse;
		public string ColorSpecular;

		public static readonly ShaderConfig Default = new ShaderConfig
		{
			DepthTest = true,
			Blend = true,

			CullMode = CullFaceMode.Back,
			FaceDirection = FrontFaceDirection.Ccw,
			PolygonMode = PolygonMode.Fill,

			SrcColorBlend = BlendingFactorSrc.SrcAlpha,
			DstColorBlend = BlendingFactorDest.OneMinusSrcAlpha,
			SrcAlphaBlend = BlendingFactorSrc.One,
			DstAlphaBlend = BlendingFactorDest.Zero,

			AttributeVertex = "a_Position",
			AttributeTexCoord = "a_TexCoord",
			AttributeNormal = "a_Normal",

			MatrixModel = "u_Model",
			MatrixModelNormal = "u_ModelNormal",
			MatrixView = "u_View",
			MatrixProjection = "u_Projection",

			TextureAmbient = "u_AmbientTexture",
			TextureDiffuse = "u_DiffuseTexture",
			TextureSpecular = "u_SpecularTexture",
			TextureNormal = "u_NormalTexture",

			ColorAmbient = "u_AmbientColor",
			ColorDiffuse = "u_DiffuseColor",
			ColorSpecular = "u_SpecularColor"
		};
	}
}
