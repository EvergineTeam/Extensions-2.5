//-----------------------------------------------------------------------------
// Sobel.fx
//
// Copyright © 2018 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform float Threshold;
uniform vec2 TexcoordOffset;
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

float Color(vec2 texcoord)
{
	vec3 color = texture2D(Texture, texcoord).xyz;
	return dot(color, vec3(0.2126, 0.7152, 0.0722));
}

void main()
{
	// Sample the neighbor pixels
	float s00 = Color(outTexCoord + (vec2(-1.0, -1.0) * TexcoordOffset));
	float s01 = Color(outTexCoord + (vec2(0.0, -1.0) * TexcoordOffset));
	float s02 = Color(outTexCoord + (vec2(1.0, -1.0) * TexcoordOffset));
	float s10 = Color(outTexCoord + (vec2(-1.0, 0.0) * TexcoordOffset));
	float s12 = Color(outTexCoord + (vec2(1.0, 0.0) * TexcoordOffset));
	float s20 = Color(outTexCoord + (vec2(-1.0, 1.0) * TexcoordOffset));
	float s21 = Color(outTexCoord + (vec2(0.0, 1.0) * TexcoordOffset));
	float s22 = Color(outTexCoord + (vec2(1.0, 1.0) * TexcoordOffset));

	// Sobel filter in X and Y directions
	float sobelX = s00 + 2.0 * s10 + s20 - s02 - 2.0 * s12 - s22;
	float sobelY = s00 + 2.0 * s01 + s02 - s20 - 2.0 * s21 - s22;

	// Find edge using a threshold to detect most edges.
	float edgeSqr = (sobelX * sobelX + sobelY * sobelY);

	if(edgeSqr > Threshold)
	{
		gl_FragColor = vec4(vec3(0), 1.0);
	}
	else
	{
		gl_FragColor = vec4(vec3(1), 1.0);
	}
}

