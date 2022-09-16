#version 330 core

out vec2 f_TexCoord;
out int f_TexIndex;

layout(location = 0) in vec2 u_Position;
layout(location = 1) in vec2 u_TexCoord;
layout(location = 2) in int u_TexIndex;

uniform mat4 u_Model;
uniform mat4 u_Projection;

void main() {
	f_TexCoord = u_TexCoord;
	f_TexIndex = u_TexIndex;
	gl_Position = projection * model * vec4(u_Position, 0.0, 1.0);
}