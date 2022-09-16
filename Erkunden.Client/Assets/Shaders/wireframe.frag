#version 330 core

//* Output Variables
out vec4 FragColor;

//* Colours
uniform vec4 u_AmbientColor;
uniform vec4 u_DiffuseColor;

void main() { FragColor = u_AmbientColor * u_DiffuseColor; }
