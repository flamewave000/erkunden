#version 330 core

out vec4 FragColor;

in vec2 f_TexCoord;
flat in int f_TexIndex;

uniform bool u_SingleChannel0;
uniform bool u_SingleChannel1;
uniform bool u_SingleChannel2;
uniform bool u_SingleChannel3;
uniform bool u_SingleChannel4;
uniform bool u_SingleChannel5;
uniform bool u_SingleChannel6;
uniform bool u_SingleChannel7;
uniform bool u_SingleChannel8;
uniform bool u_SingleChannel9;
uniform bool u_SingleChannel10;
uniform bool u_SingleChannel11;
uniform bool u_SingleChannel12;
uniform bool u_SingleChannel13;
uniform bool u_SingleChannel14;
uniform bool u_SingleChannel15;

uniform vec4 u_Tint0;
uniform vec4 u_Tint1;
uniform vec4 u_Tint2;
uniform vec4 u_Tint3;
uniform vec4 u_Tint4;
uniform vec4 u_Tint5;
uniform vec4 u_Tint6;
uniform vec4 u_Tint7;
uniform vec4 u_Tint8;
uniform vec4 u_Tint9;
uniform vec4 u_Tint10;
uniform vec4 u_Tint11;
uniform vec4 u_Tint12;
uniform vec4 u_Tint13;
uniform vec4 u_Tint14;
uniform vec4 u_Tint15;

uniform sampler2D u_Texture0;
uniform sampler2D u_Texture1;
uniform sampler2D u_Texture2;
uniform sampler2D u_Texture3;
uniform sampler2D u_Texture4;
uniform sampler2D u_Texture5;
uniform sampler2D u_Texture6;
uniform sampler2D u_Texture7;
uniform sampler2D u_Texture8;
uniform sampler2D u_Texture9;
uniform sampler2D u_Texture10;
uniform sampler2D u_Texture11;
uniform sampler2D u_Texture12;
uniform sampler2D u_Texture13;
uniform sampler2D u_Texture14;
uniform sampler2D u_Texture15;

void main() {
	vec4 texel;
	vec4 tint;
	bool singleChannel;

	switch(f_TexIndex) {
		case 0: texel = texture(u_Texture0, f_TexCoord); tint = u_Tint0; singleChannel = u_SingleChannel0; break;
		case 1: texel = texture(u_Texture1, f_TexCoord); tint = u_Tint1; singleChannel = u_SingleChannel1; break;
		case 2: texel = texture(u_Texture2, f_TexCoord); tint = u_Tint2; singleChannel = u_SingleChannel2; break;
		case 3: texel = texture(u_Texture3, f_TexCoord); tint = u_Tint3; singleChannel = u_SingleChannel3; break;
		case 4: texel = texture(u_Texture4, f_TexCoord); tint = u_Tint4; singleChannel = u_SingleChannel4; break;
		case 5: texel = texture(u_Texture5, f_TexCoord); tint = u_Tint5; singleChannel = u_SingleChannel5; break;
		case 6: texel = texture(u_Texture6, f_TexCoord); tint = u_Tint6; singleChannel = u_SingleChannel6; break;
		case 7: texel = texture(u_Texture7, f_TexCoord); tint = u_Tint7; singleChannel = u_SingleChannel7; break;
		case 8: texel = texture(u_Texture8, f_TexCoord); tint = u_Tint8; singleChannel = u_SingleChannel8; break;
		case 9: texel = texture(u_Texture9, f_TexCoord); tint = u_Tint9; singleChannel = u_SingleChannel9; break;
		case 10: texel = texture(u_Texture10, f_TexCoord); tint = u_Tint10; singleChannel = u_SingleChannel10; break;
		case 11: texel = texture(u_Texture11, f_TexCoord); tint = u_Tint11; singleChannel = u_SingleChannel11; break;
		case 12: texel = texture(u_Texture12, f_TexCoord); tint = u_Tint12; singleChannel = u_SingleChannel12; break;
		case 13: texel = texture(u_Texture13, f_TexCoord); tint = u_Tint13; singleChannel = u_SingleChannel13; break;
		case 14: texel = texture(u_Texture14, f_TexCoord); tint = u_Tint14; singleChannel = u_SingleChannel14; break;
		case 15: texel = texture(u_Texture15, f_TexCoord); tint = u_Tint15; singleChannel = u_SingleChannel15; break;
		default: FragColor = vec4(0,0,0,0); return;
	}

	if (singleChannel)
		FragColor = vec4(1, 1, 1, texel.r) * tint;
	else
		FragColor = texel * tint;
}