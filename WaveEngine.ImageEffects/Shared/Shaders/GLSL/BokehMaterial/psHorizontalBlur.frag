//-----------------------------------------------------------------------------
// Bokeh.fx
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision highp float;
#endif

#ifdef LOW
#define SAMPLES 6
#endif

#ifdef MEDIUM
#define SAMPLES 8
#endif

#ifdef HIGH
#define SAMPLES 10
#endif

float bleedingBias = 0.02;

// Parameters
uniform vec2 BlurDisp;
uniform float Aperture;
uniform float LastCoeff;
uniform float FocalDistance;
uniform float NearPlane;
uniform float FarParam;
uniform float FilmWidth;
uniform float ShineThreshold;
uniform float ShineAmount;

uniform sampler2D Texture;
uniform sampler2D DepthTexture;

// Input
varying vec2 outTexCoord;

vec3 Blur(vec4 c0, vec2 uv, vec2 step)
{
	// Accumulation
	vec3 acc = c0.xyz;

	// Total weight
	float totalweight = 1.0;

	for (int i = 1; i < SAMPLES; i++)
	{
		for (int j = -1; j <= 1; j += 2)
		{
			vec4 c1 = texture2D(Texture, uv + vec2(j * i) * step);

			float w = c0.a > (c1.a + bleedingBias) ? 0.0 : 1.0;

			acc += c1.xyz * w;
			totalweight += w;
		}
	}

	return acc / totalweight;
}

void main()
{
	// Get Color and Coc value
	vec4 c0 = texture2D(Texture, outTexCoord);

	// Calculate the step
	vec2 step = (BlurDisp * c0.a) / float(SAMPLES);

	// Horizontal Blur
	vec3 color = Blur(c0, outTexCoord, step);

	gl_FragColor = vec4(color, c0.a);
}
