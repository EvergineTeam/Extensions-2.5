//-----------------------------------------------------------------------------
// Convolution.fx
//
// Copyright © 2018 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform vec2 TexcoordOffset;
uniform float Scale;
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

vec4 Color(vec2 texcoord)
{
	return texture2D(Texture, texcoord);
}

void main()
{
	vec4 emboss;
	emboss.a = 1.0;
	emboss.rgb = vec3(0.5, 0.5, 0.5);

	emboss -= 2.0 * Color(outTexCoord + (TexcoordOffset * vec2(-1.0, -1.0)));
	emboss += 2.0 * Color(outTexCoord + (TexcoordOffset * vec2(1.0, 1.0)));
	emboss.rgb = vec3(emboss.r + emboss.g + emboss.b) / 3.0;

	gl_FragColor = emboss;
}
