#version 330 core

// Output Variables
out vec2 f_TexCoord;
out vec3 f_Normal;
out vec3 f_FragPos;

// Input Variables
layout(location = 0) in vec3 a_Position;
layout(location = 1) in vec2 a_TexCoord;
layout(location = 2) in vec3 a_Normal;

// Global Variables
uniform mat4 u_Model;
uniform mat4 u_View;
uniform mat4 u_Projection;
uniform mat3 u_ModelNormal;

void main() {
	f_TexCoord = a_TexCoord;
	vec4 position = vec4(a_Position, 1) * u_Model;
	gl_Position = position * u_View * u_Projection;
	f_Normal = normalize(a_Normal * u_ModelNormal);
	f_FragPos = position.xyz;
}
