#version 330 core
in vec3 aPosition;
in vec3 aNormal;
in vec3 aTexCoord;

out vec4 vertexColor;

void main()
{
	gl_Position = vec4(aPosition, 1.0);
	vertexColor = vec4(aPosition, 1.0);
}
