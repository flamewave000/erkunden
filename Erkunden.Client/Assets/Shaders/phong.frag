#version 430

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
const int POINT = 1;
const int DIRECT = 2;
const int SPOT = 3;
struct Light {
	vec4 color;
	vec4 ambientColor;
	vec4 position;
	vec4 direction;
	float intensity;
	float ambientStrength;
	float linear;
	float quadratic;
	float constant;
	float cuttoff;
	int type;
};

uniform vec3 u_EyePos;

// #define MAX_LIGHT_COUNT 10
// uniform int u_LightCount;
// uniform Light u_Lights[MAX_LIGHT_COUNT];

layout(binding = 0, std430) buffer LightsBuffer {
	int count;
	Light values[];
}
b_Lights;

vec3 materialAmbient;
vec3 materialSpecular;

vec4 getColour(vec4 color, sampler2D tex) { return textureSize(tex, 0).x > 1 ? texture(tex, f_TexCoord.st) : color; }

float attenuate(Light light, float distance) {
	return 1.0 / (light.constant + (light.linear * distance) + (light.quadratic * (distance * distance)));
}

float surfaceNormalScalar(vec3 lightNormal) { return max(dot(f_Normal, lightNormal), 0); }

vec3 CalculateSpecularColor(vec3 lightNormal, vec3 eyeNormal) {
	return materialSpecular * pow(max(dot(reflect(-lightNormal, f_Normal), eyeNormal), 0), u_Shininess);
}

vec3 CalculateLightColor(Light light, float scalar) { return light.color.rgb * light.intensity * scalar; }

vec3 CalculateAmbientColor(Light light, float scalar) {
	return materialAmbient * light.ambientColor.rgb * clamp(light.ambientStrength, 0.1, 1.0) * (1.0 - scalar);
}

vec3 CalculatePointLight(Light light) {
	// Set up light vertices
	vec3 lightVec = light.position.xyz - f_FragPos;
	vec3 lightNormal = normalize(lightVec);
	float lightScalar = surfaceNormalScalar(lightNormal);

	// Calculate the attenuation for the light
	float attenuation = attenuate(light, length(lightVec));
	// Calculate Ambient Colour
	vec3 ambientColor = CalculateAmbientColor(light, lightScalar) * attenuation;
	// Calculate Light Colour
	vec3 lightColor = CalculateLightColor(light, lightScalar) * attenuation * 2;
	// Calculate Specular Colour
	vec3 specularColor = CalculateSpecularColor(lightNormal, normalize(u_EyePos - f_FragPos)) * attenuation;
	return ambientColor + lightColor + specularColor;
}

vec3 CalculateDirectionalLight(Light light) {
	float lightScalar = surfaceNormalScalar(normalize(light.direction.xyz));
	// Calculate Ambient Colour
	vec3 ambientColor = CalculateAmbientColor(light, lightScalar);
	// Calculate Light Colour
	vec3 lightColor = CalculateLightColor(light, lightScalar);
	// Calculate Specular Colour
	vec3 specularColor = CalculateSpecularColor(light.direction.xyz, normalize(u_EyePos - f_FragPos));
	// Calculate the final colour
	return lightColor + ambientColor + specularColor;
}

void main() {
	vec4 diffuse = getColour(u_DiffuseColor, u_DiffuseTexture);

	if (b_Lights.count <= 0) {
		FragColor = diffuse;
		return;
	}

	materialAmbient = getColour(u_AmbientColor, u_AmbientTexture).rgb;
	materialSpecular = getColour(u_SpecularColor, u_SpecularTexture).rgb;

	vec3 lightTotal = vec3(0);
	for (int i = 0; i < b_Lights.count; i++) {
		switch (b_Lights.values[i].type) {
		case POINT:
			lightTotal += CalculatePointLight(b_Lights.values[i]);
			break;
		case DIRECT:
			lightTotal += CalculateDirectionalLight(b_Lights.values[i]);
			break;
		default:
			FragColor = vec4(1, 0, 1, 1);
			return;
		}
	}
	FragColor = vec4(diffuse.xyz * lightTotal, diffuse.a);
}
