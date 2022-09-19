#version 330 core

//* Output Variables
out vec4 FragColor;

//* Input Variables
in vec2 f_TexCoord;
in vec3 f_Normal;
in vec3 f_FragPos;

//* Textures
uniform sampler2D u_AmbientTexture;
uniform sampler2D u_DiffuseTexture;
uniform sampler2D u_SpecularTexture;
uniform sampler2D u_NormalTexture;

//* Default Colours
uniform vec4 u_AmbientColor;
uniform vec4 u_DiffuseColor;
uniform vec4 u_SpecularColor;
uniform float u_Shininess;

//* Light Info
struct Light {
	int type;
	vec3 vector;
	vec4 color;
	vec4 ambientColor;
	float ambientStrength;
	float linear;
	float quadratic;
	float constant;
};

uniform vec3 u_EyePos;
uniform Light u_Light;
uniform mat4 u_View;

vec4 getColour(vec4 color, sampler2D tex) { return textureSize(tex, 0).x > 1 ? texture(tex, f_TexCoord.st) : color; }

// float attenuation(float radius, float falloff, float dist) {
// 	float denom = dist / radius + 1.0;
// 	float attenuation = 1.0 / (denom * denom);
// 	float result = (attenuation - falloff) / (1.0 - falloff);
// 	return max(result, 0.0);
// }

void main() {
	// Set up light vertices
	vec3 lightVec = u_Light.vector - f_FragPos;
	vec3 lightNormal = normalize(lightVec);
	vec3 eyeNormal = normalize(u_EyePos - f_FragPos);

	// Calculate Ambient Colour
	vec3 ambientColor = getColour(u_Light.ambientColor, u_AmbientTexture).rgb;
	ambientColor *= u_Light.ambientColor.rgb * clamp(u_Light.ambientStrength, 0.1, 1);

	// Calculate Light Colour
	vec3 lightColor = u_Light.color.rgb * (max(dot(f_Normal, lightNormal), 0) * 2);

	// Calculate Specular Colour
	vec3 specularColor = getColour(u_SpecularColor, u_SpecularTexture).rgb;
	vec3 reflection = reflect(-lightNormal, f_Normal);
	specularColor *= pow(max(dot(reflection, eyeNormal), 0), u_Shininess);

	float dist = length(lightVec);
	float attenuation = 1.0 / (u_Light.constant + (u_Light.linear * dist) + (u_Light.quadratic * (dist * dist)));

	lightColor *= attenuation;
	ambientColor *= attenuation * 2;
	specularColor *= attenuation;

	// Calculate the final colour
	vec4 diffuseColor = getColour(u_DiffuseColor, u_DiffuseTexture);
	gl_FragColor = vec4(diffuseColor.rgb * (lightColor + ambientColor + specularColor), diffuseColor.a);
}
