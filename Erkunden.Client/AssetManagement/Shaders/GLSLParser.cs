using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Erkunden.Client.Graphics.Data;
using Erkunden.Client.Graphics.Objects;
using Erkunden.Core.Util;
using OpenTK.Graphics.OpenGL4;

namespace Erkunden.Client.AssetManagement.Shaders
{
	public class GLSLInvalidTokenException : Exception
	{
		public GLSLInvalidTokenException(string token, string propName, int line)
			: base($"LN{line}: Unknown '{propName}' Value: {token}") { }
	}

	public class GLSLParser : AssetParser
	{
		public static void Register() => AssetProvider.RegisterParser(new GLSLParser());

		public string[] Extensions { get; } = new string[] { ".ini" };

		public IEnumerable<string> GetNames(FileInfo file)
		{
			using (StreamReader stream = new StreamReader(file.OpenRead()))
			{
				string? line;
				while (!stream.EndOfStream)
				{
					line = stream.ReadLine()?.Trim();
					if (line == null || !line.StartsWith("[")) continue;
					yield return AssetParser.ConformName<Shader>(
						line.ReduceSpaces().Substring(1, line.Length - 2).Trim()
					);
				}
			}
		}

		public void Parse(FileInfo file, AssetStore store)
		{
			using (StreamReader stream = new StreamReader(file.OpenRead()))
			{
				string? line;
				string[] tokens;

				int lineCount = 0;

				string? shaderName = null;
				List<ShaderScript> scripts = new List<ShaderScript>();
				ShaderConfig config = ShaderConfig.Default;

				Action<string?> pushShader = name =>
				{
					if (shaderName != null)
					{
						store.Add(AssetParser.ConformName<Shader>(shaderName), new Shader(shaderName, scripts.ToArray(), config));
						scripts.Clear();
						config = ShaderConfig.Default;
					}
					shaderName = name;
				};

				while (!stream.EndOfStream)
				{
					line = stream.ReadLine();
					if (line == null) continue;
					lineCount++;
					line = line.Trim();
					if (line.Length == 0) continue;
					if (line.StartsWith('['))
					{
						pushShader(line.Substring(1, line.Length - 2).Trim());
						continue;
					}
					else
						tokens = line.Trim().Split('=').Select(x => x.Trim()).ToArray();

					switch (tokens[0])
					{
						#region Shader Scripts
						case "vert":
							scripts.Add(new ShaderScript(tokens[1], ReadFile(file, tokens[1]), ShaderType.VertexShader));
							break;
						case "frag":
							scripts.Add(new ShaderScript(tokens[1], ReadFile(file, tokens[1]), ShaderType.FragmentShader));
							break;
						#endregion
						#region Draw Flags
						case "depth":
							config.DepthTest = ParseBool(tokens[1], tokens[0], lineCount);
							break;
						case "blend":
							config.Blend = ParseBool(tokens[1], tokens[0], lineCount);
							break;
						#endregion
						#region General Settings
						case "face":
							switch (tokens[1].ToLower())
							{
								case "cw": config.FaceDirection = FrontFaceDirection.Cw; break;
								case "ccw": config.FaceDirection = FrontFaceDirection.Ccw; break;
								default: throw new GLSLInvalidTokenException(tokens[1], tokens[0], lineCount);
							}
							break;
						case "poly":
							switch (tokens[1])
							{
								case "point": config.PolygonMode = PolygonMode.Point; break;
								case "line": config.PolygonMode = PolygonMode.Line; break;
								case "fill": config.PolygonMode = PolygonMode.Fill; break;
								default: throw new GLSLInvalidTokenException(tokens[1], tokens[0], lineCount);
							}
							break;
						case "cull":
							switch (tokens[1])
							{
								case "front": config.CullMode = CullFaceMode.Front; break;
								case "back": config.CullMode = CullFaceMode.Back; break;
								case "both": config.CullMode = CullFaceMode.FrontAndBack; break;
								default: throw new GLSLInvalidTokenException(tokens[1], tokens[0], lineCount);
							}
							break;
						#endregion
						#region Blending Options
						case "srccolor":
							config.SrcColorBlend = (BlendingFactorSrc)ParseBlendingFactor(tokens[1], tokens[0], lineCount);
							break;
						case "dstcolor":
							config.DstColorBlend = (BlendingFactorDest)ParseBlendingFactor(tokens[1], tokens[0], lineCount);
							break;
						case "srcalpha":
							config.SrcAlphaBlend = (BlendingFactorSrc)ParseBlendingFactor(tokens[1], tokens[0], lineCount);
							break;
						case "dstalpha":
							config.DstAlphaBlend = (BlendingFactorDest)ParseBlendingFactor(tokens[1], tokens[0], lineCount);
							break;
						#endregion
						#region Attributes
						case "attr_vert":
							config.AttributeVertex = tokens[1];
							break;
						case "attr_texc":
							config.AttributeTexCoord = tokens[1];
							break;
						case "attr_norm":
							config.AttributeNormal = tokens[1];
							break;
						#endregion
						#region Matrix Uniforms
						case "mat4_modl":
							config.MatrixModel = tokens[1];
							break;
						case "mat4_view":
							config.MatrixView = tokens[1];
							break;
						case "mat4_proj":
							config.MatrixProjection = tokens[1];
							break;
						#endregion
						#region Texture2D Uniforms
						case "tex2_occl":
							config.TextureAmbient = tokens[1];
							break;
						case "tex2_diff":
							config.TextureDiffuse = tokens[1];
							break;
						case "tex2_spec":
							config.TextureSpecular = tokens[1];
							break;
						case "tex2_norm":
							config.TextureNormal = tokens[1];
							break;
						#endregion
						#region Colour Uniforms
						case "colr_occl":
							config.ColorAmbient = tokens[1];
							break;
						case "colr_diff":
							config.ColorDiffuse = tokens[1];
							break;
						case "colr_spec":
							config.ColorSpecular = tokens[1];
							break;
						#endregion
						default:
							break;
					}
				}
				pushShader(null);
			}
		}

		private string ReadFile(FileInfo currentFile, string shaderFile)
		{
			string path = Path.Combine(currentFile.Directory.FullName, shaderFile);
			using (StreamReader stream = new StreamReader(path))
			{
				return stream.ReadToEnd();
			}
		}

		private BlendingFactor ParseBlendingFactor(string token, string fieldName, int lineNumber)
		{
			BlendingFactor result = BlendingFactor.One;
			if (!Enum.TryParse(token, true, out result))
				throw new GLSLInvalidTokenException(token, fieldName, lineNumber);
			return result;
		}

		private bool ParseBool(string token, string fieldName, int lineNumber)
		{
			switch (token.ToLower())
			{
				case "0":
				case "off":
				case "false": return false;
				case "1":
				case "on":
				case "true": return true;
				default: throw new GLSLInvalidTokenException(token, fieldName, lineNumber);
			}
		}
	}

	/* GLSL Shader INI Declaration File Technical Definition

	; Line comments start with semicolon

	[MyShader]						; Shader definition starts with the name in a set of square brackets
	vert = ./MyShader.vert			; Define Vertex Shader source file
	frag = ./MyShader.frag			; Define Fragment Shader source file

	blend = <bool>					; Blending Enabled/Disabled. Default true
									;	<bool>: on | off | 1 | 0 | true | false
	depth = <bool>					; Blending Enabled/Disabled. Default true
									;	<bool>: on | off | 1 | 0 | true | false
	
	poly = <mode>					; Polygon Draw Mode. Default: fill
									;	<mode>:	point | line | fill

	face = <dir>					; Triangle Draw Direction for determining Front|Back facing and culling
									;	<dir>: cw | ccw

	cull = <face>					; Which faces should be culled. Default: back
									;	<face>: front | back | both
	
	; Blend Functions, values are the string names of the OpenTK.Graphics.OpenGL4.BlendingFactor enums.
	; These values can be seen in a note below.
	srccolor = BlendingFactor		; Default: SrcAlpha
	dstcolor = BlendingFactor		; Default: OneMinusSrcAlpha
	srcalpha = BlendingFactor		; Default: One
	dstalpha = BlendingFactor		; Default: Zero
	
	; Below are common attributes and uniforms along with their default values.
	; These can be changed to reflect what is in the shader script.
	attr_vert = a_Position			; vec4 in a_Position;					Vertex Attribute
	attr_texc = a_TexCoord			; vec4 in a_TexCoord;					TexCoord Attribute
	attr_norm = a_Normal			; vec4 in a_Normal;						Normal Attribute

	mat4_modl = u_Model				; uniform mat4 u_Model;					Model Matrix
	mat4_view = u_View				; uniform mat4 u_View;					Camera Matrix
	mat4_proj = u_Projection		; uniform mat4 u_Projection;			Projection Matrix

	tex2_occl = u_AmbientTexture	; uniform sampler2D u_AmbientTexture;	Ambient Occlusion
	tex2_diff = u_DiffuseTexture	; uniform sampler2D u_DiffuseTexture;	Diffuse/Base Colour
	tex2_spec = u_SpecularTexture	; uniform sampler2D u_SpecularTexture;	Specular/Metallic
	tex2_norm = u_NormalTexture		; uniform sampler2D u_NormalTexture;	Normal/Bump
	
	colr_occl = u_AmbientColor		; uniform vec4 u_AmbientColor;			Static Fallback Colour (if missing texture)
	colr_diff = u_DiffuseColor		; uniform vec4 u_DiffuseColor;			Static Fallback Colour (if missing texture)
	colr_spec = u_SpecularColor		; uniform vec4 u_SpecularColor;			Static Fallback Colour (if missing texture)
	

	; NOTE BlendingFactor Enum Values
	;	Zero
	;	SrcColor
	;	OneMinusSrcColor
	;	SrcAlpha
	;	OneMinusSrcAlpha
	;	DstAlpha
	;	OneMinusDstAlpha
	;	DstColor
	;	OneMinusDstColor
	;	SrcAlphaSaturate
	;	ConstantColor
	;	OneMinusConstantColor
	;	ConstantAlpha
	;	OneMinusConstantAlpha
	;	Src1Alpha
	;	Src1Color
	;	OneMinusSrc1Color
	;	OneMinusSrc1Alpha
	;	One

	*/
}
