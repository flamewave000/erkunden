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

#define MAX_LIGHT_COUNT 10
uniform int u_LightCount;
uniform Light u_Lights[MAX_LIGHT_COUNT];

vec3 ambientColor;
vec3 specularColor;

vec4 getColour(vec4 color, sampler2D tex) { return textureSize(tex, 0).x > 1 ? texture(tex, f_TexCoord.st) : color; }

float attenuate(Light light, float distance) {
	return 1.0 / (light.constant + (light.linear * distance) + (light.quadratic * (distance * distance)));
}

vec3 CalculateSpecularColor(vec3 lightNormal, vec3 eyeNormal) {
	return specularColor * pow(max(dot(reflect(-lightNormal, f_Normal), eyeNormal), 0), u_Shininess);
}

vec3 CalculateLightColor(Light light, vec3 lightNormal) {
	return light.color.rgb * (max(dot(f_Normal, lightNormal), 0) * 2);
}

vec3 CalculateAmbientColor(Light light) {
	return ambientColor * light.ambientColor.rgb * clamp(light.ambientStrength, 0.1, 1);
}

vec3 CalulatePointLight(Light light) {
	// Set up light vertices
	vec3 lightVec = light.position - f_FragPos;
	vec3 lightNormal = normalize(lightVec);

	// Calculate the attenuation for the light
	float attenuation = attenuate(light, length(lightVec));
	// Calculate Ambient Colour
	vec3 ambientColor = CalculateAmbientColor(light) * attenuation;
	// Calculate Light Colour
	vec3 lightColor = CalculateLightColor(light, lightNormal) * attenuation * 2;
	// Calculate Specular Colour
	vec3 specularColor = CalculateSpecularColor(lightNormal, normalize(u_EyePos - f_FragPos)) * attenuation;
	return ambientColor + lightColor + specularColor;
}

vec3 CalulateDirectionalLight(Light light) {
	// Calculate Ambient Colour
	vec3 ambientColor = CalculateAmbientColor(light);
	// Calculate Light Colour
	vec3 lightColor = CalculateLightColor(light, light.direction);
	// Calculate Specular Colour
	vec3 specularColor = CalculateSpecularColor(light.direction, normalize(u_EyePos - f_FragPos));
	// Calculate the final colour
	return lightColor + ambientColor + specularColor;
}

void main() {
	ambientColor = getColour(u_AmbientColor, u_AmbientTexture).rgb;
	specularColor = getColour(u_SpecularColor, u_SpecularTexture).rgb;

	vec3 lightTotal = vec3(0);
	int lightCount = min(u_LightCount, MAX_LIGHT_COUNT);

	for (int i = 0; i < lightCount; i++) {
		switch (u_Lights[i].type) {
		case POINT:
			lightTotal += CalulatePointLight(u_Lights[i]);
			break;
		case DIRECT:
			lightTotal += CalulateDirectionalLight(u_Lights[i]);
			break;
		}
	}
	vec4 diffuse = getColour(u_DiffuseColor, u_DiffuseTexture);
	FragColor = vec4(diffuse.xyz * lightTotal, diffuse.a);
}
