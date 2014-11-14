//-----------------------------------------------------------------------------
// Convolution.fx
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Paramenters
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
	//  1  1  1
	//  1  1  1
	//  1  1  1
	vec2 samples[9];
	samples[0] = vec2(-1, -1);
	samples[1] = vec2( 0, -1);
	samples[2] = vec2( 1, -1);
	samples[3] = vec2(-1,  0);
	samples[4] = vec2( 0,  0);
	samples[5] = vec2( 1,  0);
	samples[6] = vec2(-1,  1);
	samples[7] = vec2( 0,  1);
	samples[8] = vec2( 1,  1);

	vec4 blur = vec4(0);

	for (int i = 0; i < 9; i++)
	{
		blur += Color(outTexCoord + (samples[i] * TexcoordOffset));
	}
	
	blur.rgb /= 9.0;
	gl_FragColor = blur;
}
