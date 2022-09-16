#version 330 core

out vec4 FragColor;

in vec2 f_TexCoord;
in int f_TexIndex;

uniform vec4 u_Tint0;
uniform vec4 u_Tint1;

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
	sampler2D tex;
	switch(f_TexIndex) {
		case 0: tex = u_Texture0; break;
		case 1: tex = u_Texture1; break;
		case 2: tex = u_Texture2; break;
		case 3: tex = u_Texture3; break;
		case 4: tex = u_Texture4; break;
		case 5: tex = u_Texture5; break;
		case 6: tex = u_Texture6; break;
		case 7: tex = u_Texture7; break;
		case 8: tex = u_Texture8; break;
		case 9: tex = u_Texture9; break;
		case 10: tex = u_Texture10; break;
		case 11: tex = u_Texture11; break;
		case 12: tex = u_Texture12; break;
		case 13: tex = u_Texture13; break;
		case 14: tex = u_Texture14; break;
		case 15: tex = u_Texture15; break;
	}

	float glyph = texture(tex, f_TexCoord).r;
	if (glyph == 0)
		FragColor = vec4(0);
	else if (glyph == 1)
		FragColor = u_GlyphTint;
	else
		FragColor = u_BorderTint;
}