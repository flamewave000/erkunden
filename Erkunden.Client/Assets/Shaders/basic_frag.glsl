#version 330 core
out vec4 FragColor;

in vec4 vertexColor;

void main()
{
	FragColor = vec4(vertexColor.x + 0.5, vertexColor.y + 0.5, vertexColor.z + 0.5, 1.0);
}
