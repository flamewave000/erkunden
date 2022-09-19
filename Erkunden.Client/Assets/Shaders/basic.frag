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
const int POINT = 0;
const int DIRECT = 1;
const int SPOT = 2;
struct Light {
	int type;
	vec3 position;
	vec3 direction;
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

float attenuate(float distance) {
	return 1.0 / (u_Light.constant + (u_Light.linear * distance) + (u_Light.quadratic * (distance * distance)));
}

vec3 CalculateSpecularColor(vec3 lightNormal, vec3 eyeNormal) {
	return getColour(u_SpecularColor, u_SpecularTexture).rgb *
		   pow(max(dot(reflect(-lightNormal, f_Normal), eyeNormal), 0), u_Shininess);
}

vec3 CalculateLightColor(vec3 lightNormal) { return u_Light.color.rgb * (max(dot(f_Normal, lightNormal), 0) * 2); }

vec3 CalculateAmbientColor() {
	return getColour(u_Light.ambientColor, u_AmbientTexture).rgb * u_Light.ambientColor.rgb *
		   clamp(u_Light.ambientStrength, 0.1, 1);
}

vec4 AggregateColor(vec3 light, vec3 ambient, vec3 specular) {
	vec4 diffuse = getColour(u_DiffuseColor, u_DiffuseTexture);
	return vec4(diffuse.xyz * (light + ambient + specular), diffuse.a);
}

void CalulatePointLight() {
	// Set up light vertices
	vec3 lightVec = u_Light.position - f_FragPos;
	vec3 lightNormal = normalize(lightVec);

	// Calculate the attenuation for the light
	float attenuation = attenuate(length(lightVec));
	// Calculate Ambient Colour
	vec3 ambientColor = CalculateAmbientColor() * attenuation;
	// Calculate Light Colour
	vec3 lightColor = CalculateLightColor(lightNormal) * attenuation * 2;
	// Calculate Specular Colour
	vec3 specularColor = CalculateSpecularColor(lightNormal, normalize(u_EyePos - f_FragPos)) * attenuation;
	// Calculate the final colour
	FragColor = AggregateColor(lightColor, ambientColor, specularColor);
}

void CalulateDirectionalLight() {
	// Calculate Ambient Colour
	vec3 ambientColor = CalculateAmbientColor();
	// Calculate Light Colour
	vec3 lightColor = CalculateLightColor(u_Light.direction);
	// Calculate Specular Colour
	vec3 specularColor = CalculateSpecularColor(u_Light.direction, normalize(u_EyePos - f_FragPos));
	// Calculate the final colour
	FragColor = AggregateColor(lightColor, ambientColor, specularColor);
}

// void CalulateSpotLight() {
// 	// Set up light vertices
// 	vec3 lightVec = u_Light.position - f_FragPos;
// 	vec3 lightNormal = normalize(lightVec);
// 	vec3 eyeNormal = normalize(u_EyePos - f_FragPos);

// 	// Calculate the attenuation for the light
// 	float attenuation = attenuate(length(lightVec));
// 	// Calculate Ambient Colour
// 	vec3 ambientColor = CalculateAmbientColor() * attenuation;
// 	// Calculate Light Colour
// 	vec3 lightColor = CalculateLightColor(lightNormal) * attenuation * 2;
// 	// Calculate Specular Colour
// 	vec3 specularColor = CalculateSpecularColor(lightNormal, eyeNormal) * attenuation;
// 	// Calculate the final colour
// 	FragColor = AggregateColor(lightColor, ambientColor, specularColor);
// }

void main() {
	switch (u_Light.type) {
	case POINT:
		CalulatePointLight();
		break;
	case DIRECT:
		CalulateDirectionalLight();
		break;
	// case SPOT:
	// 	CalulateSpotLight();
	// 	break;
	default:
		FragColor = vec4(1, 0, 1, 1);
		break;
	}
}
