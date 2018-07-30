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
	//  1  1  1  1  1
	//  1  1  1  1  1
	//  1  1  1  1  1
	//  1  1  1  1  1
	//  1  1  1  1  1
	vec2 samples[25];
	samples[0] = vec2(-2, -2);
	samples[1] = vec2(-1, -2);
	samples[2] = vec2( 0, -2);
	samples[3] = vec2( 1, -2);
	samples[4] = vec2( 2, -2);
	samples[5] = vec2(-2, -1);
	samples[6] = vec2(-1, -1);
	samples[7] = vec2( 0, -1);
	samples[8] = vec2( 1, -1);
	samples[9] = vec2( 2, -1);
	samples[10] = vec2(-2,  0);
	samples[11] = vec2(-1,  0);
	samples[12] = vec2( 0,  0);
	samples[13] = vec2( 1,  0);
	samples[14] = vec2( 2,  0);
	samples[15] = vec2(-2,  1);
	samples[16] = vec2(-1,  1);
	samples[17] = vec2( 0,  1);
	samples[18] = vec2( 1,  1);
	samples[19] = vec2( 2,  1);
	samples[20] = vec2(-2,  2);
	samples[21] = vec2(-1,  2);
	samples[22] = vec2( 0,  2);
	samples[23] = vec2( 1,  2);
	samples[24] = vec2( 2,  2);

	vec4 blur = vec4(0);

	for (int i = 0; i < 25; i++)
	{
		blur += Color(outTexCoord + (samples[i] * TexcoordOffset));
	}
	
	blur.rgb /= 25.0;
	gl_FragColor = blur;
}
