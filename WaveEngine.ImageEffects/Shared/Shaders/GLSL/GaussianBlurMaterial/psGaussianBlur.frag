//-----------------------------------------------------------------------------
// GaussianBlur.fx
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform float SampleWeights0;
uniform float SampleWeights1;
uniform float SampleWeights2;
uniform float SampleWeights3;
uniform float SampleWeights4;
uniform float SampleWeights5;
uniform float SampleWeights6;
uniform float SampleWeights7;
uniform float SampleWeights8;
uniform float SampleWeights9;
uniform float SampleWeights10;
uniform float SampleWeights11;
uniform float SampleWeights12;
uniform float SampleWeights13;

uniform sampler2D Texture;

// Input
varying vec4 OutUV[7];

void main()
{
	vec3 N0 = texture2D(Texture, OutUV[0].xy).rgb;
	vec3 N1 = texture2D(Texture, OutUV[0].zw).rgb;
	vec3 N2 = texture2D(Texture, OutUV[1].xy).rgb;
	vec3 N3 = texture2D(Texture, OutUV[1].zw).rgb;
	vec3 N4 = texture2D(Texture, OutUV[2].xy).rgb;
	vec3 N5 = texture2D(Texture, OutUV[2].zw).rgb;
	vec3 N6 = texture2D(Texture, OutUV[3].xy).rgb;
	vec3 N7 = texture2D(Texture, OutUV[3].zw).rgb;
	vec3 N8 = texture2D(Texture, OutUV[4].xy).rgb;
	vec3 N9 = texture2D(Texture, OutUV[4].zw).rgb;
	vec3 N10 = texture2D(Texture, OutUV[5].xy).rgb;
	vec3 N11 = texture2D(Texture, OutUV[5].zw).rgb;
	vec3 N12 = texture2D(Texture, OutUV[6].xy).rgb;
	vec3 N13 = texture2D(Texture, OutUV[6].zw).rgb;

	vec3 color = vec3(0.0,0.0,0.0);

	color.rgb =
		(N0 * SampleWeights0) +
		(N1 * SampleWeights1) +
		(N2 * SampleWeights2) +
		(N3 * SampleWeights3) +
		(N4 * SampleWeights4) +
		(N5 * SampleWeights5) +
		(N6 * SampleWeights6) +
		(N7 * SampleWeights7) +
		(N8 * SampleWeights8) +
		(N9 * SampleWeights9) +
		(N10 * SampleWeights10) +
		(N11 * SampleWeights11) +
		(N12 * SampleWeights12) +
		(N13 * SampleWeights13);
		

	gl_FragColor = vec4(color.rgb, 1.0);
}
