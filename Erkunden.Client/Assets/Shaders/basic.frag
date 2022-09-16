#version 330 core

//* Output Variables
out vec4 FragColor;

//* Input Variables
in vec2 f_TexCoord;
in vec3 f_Normal;

//* Textures
uniform sampler2D u_AmbientTexture;
uniform sampler2D u_DiffuseTexture;
uniform sampler2D u_SpecularTexture;
uniform sampler2D u_NormalTexture;

//* Default Colours
uniform vec4 u_AmbientColor;
uniform vec4 u_DiffuseColor;
uniform vec4 u_SpecularColor;

//* Shininess
uniform float u_Shininess;

vec4 getColour(vec4 color, sampler2D tex) {
	return textureSize(tex, 0).x > 0 ? texture(tex, f_TexCoord.st) : color;
}

void main() {
	vec4 ambient = getColour(u_AmbientColor, u_AmbientTexture);
	vec4 diffuse = getColour(u_DiffuseColor, u_DiffuseTexture);
	vec4 specular = getColour(u_SpecularColor, u_SpecularTexture);
	vec4 normal = getColour(vec4(f_Normal, 0.0), u_NormalTexture);
	FragColor = diffuse;
}
