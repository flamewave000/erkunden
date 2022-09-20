#version 330 core

//* Output Variables
out vec4 FragColor;

in vec4 f_FragPos;
in vec2 f_TexCoord;

//* Texture
uniform sampler2D u_DiffuseTexture;

//* Colours
uniform vec4 u_DiffuseColor;
uniform bool u_UseVertexColour;
uniform float u_VertexScalar;

void main() {
	if (u_UseVertexColour) {
		float r = (f_FragPos.x * u_VertexScalar * 0.5) + 0.5;
		float g = (f_FragPos.z * u_VertexScalar * 0.5) + 0.5;
		float b = 1 - (r + g);
		FragColor = vec4(r, g, b, 1);
	} else {
		FragColor = texture(u_DiffuseTexture, f_TexCoord);
		FragColor.r = 1 - FragColor.r;
		FragColor.g = 1 - FragColor.g;
		FragColor.b = 1 - FragColor.b;
	}
}
