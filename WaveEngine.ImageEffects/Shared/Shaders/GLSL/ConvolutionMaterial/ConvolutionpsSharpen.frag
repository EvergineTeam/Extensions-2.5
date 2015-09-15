//-----------------------------------------------------------------------------
// Convolution.fx
//
// Copyright © 2015 Wave Corporation
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
	//  0 -1  0
	// -1  5 -1
	//  0 -1  0
	vec2 samples[4];
	samples[0] = vec2( 0, -1);
	samples[1] = vec2(-1, -0);
	samples[2] = vec2( 1,  0);
	samples[3] = vec2( 0,  1);

	vec4 sharpen = 5.0 * Color(outTexCoord);

	for (int i = 0; i < 4; i++)
	{
		sharpen -= Color(outTexCoord + (samples[i] * TexcoordOffset));
	}

	gl_FragColor = sharpen;
}
