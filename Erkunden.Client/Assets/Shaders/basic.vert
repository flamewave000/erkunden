
#version 330 core

// Input Variables
layout(location = 0) in vec3 a_Position;
layout(location = 1) in vec2 a_TexCoord;

out vec2 f_TexCoord;
out vec3 f_FragPos;

// Global Variables
uniform mat4 u_Model;
uniform mat4 u_View;
uniform mat4 u_Projection;

void main() {
	f_TexCoord = a_TexCoord;
	vec4 position = vec4(a_Position, 1) * u_Model;
	f_FragPos = position.xyz;
	gl_Position = position * u_View * u_Projection;
}
