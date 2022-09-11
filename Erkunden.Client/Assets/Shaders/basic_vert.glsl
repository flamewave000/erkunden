#version 330 core

// Output Variables
out vec2 f_TexCoord;
out vec3 f_Normal;

// Input Variables
layout(location = 0) in vec3 a_Position;
layout(location = 1) in vec2 a_TexCoord;
layout(location = 2) in vec3 a_Normal;

// Global Variables

void main()
{
	f_TexCoord = a_TexCoord;
	f_Normal = a_Normal;
	gl_Position = vec4(a_Position, 1.0);
}
