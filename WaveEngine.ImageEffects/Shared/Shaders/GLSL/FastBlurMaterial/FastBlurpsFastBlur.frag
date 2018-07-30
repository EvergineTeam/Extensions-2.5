//-----------------------------------------------------------------------------
// FastBlur.fx
//
// Copyright © 2018 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform sampler2D Texture;

// Input
varying vec4 OutUV[8];

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
	vec3 N14 = texture2D(Texture, OutUV[7].xy).rgb;

	float W = 1.0 / 15.0;
	
	vec3 color = vec3(0.0,0.0,0.0);

	color.rgb =
		(N0 * W) +
		(N1 * W) +
		(N2 * W) +
		(N3 * W) +
		(N4 * W) +
		(N5 * W) +
		(N6 * W) +
		(N7 * W) +
		(N8 * W) +
		(N9 * W) +
		(N10 * W) +
		(N11 * W) +
		(N12 * W) +
		(N13 * W) +
		(N14 * W);

	gl_FragColor = vec4(color.rgb, 1.0);
}
