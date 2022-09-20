#version 330 core

//* Output Variables
out vec4 FragColor;

//* Input Variables
in vec2 f_TexCoord;

//* Texture Uniforms
uniform sampler2D u_DiffuseTexture;

//* Colours
uniform vec4 u_DiffuseColor;

vec4 getColour(vec4 color, sampler2D tex, vec2 texCoord) {
	return textureSize(tex, 0).x > 1 ? texture(tex, texCoord) : color;
}

void main() { FragColor = getColour(u_DiffuseColor, u_DiffuseTexture, f_TexCoord.st); }
