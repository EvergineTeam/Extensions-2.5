//-----------------------------------------------------------------------------
// SSAO.fx
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision highp float;
#endif

// Parameters
uniform float DistanceThreshold;
uniform float AOintensity;
uniform mat4 ViewProjectionInverse;

uniform sampler2D Texture;
uniform sampler2D AOTexture;
uniform sampler2D GBufferTexture;
uniform sampler2D DepthTexture;


// Input
varying vec2 outTexCoord;
varying vec2 outPoisson[16];

const int sample_count = 16;
const float sample_count_f = 16.0;


vec3 CalculatePosition(in vec2 texCoord, in float depth)
{
  // H is the viewport position at this pixel in the range -1 to 1.
	vec4 H = vec4(texCoord.x * 2.0 - 1.0, (1.0 - texCoord.y) * 2.0 - 1.0, depth, 1.0);

	// Transform by the view-projection inverse.
	vec4 D = ViewProjectionInverse * H;
	return D.xyz / vec3(D.w);
}

void main()
{
	vec2 texCoord = outTexCoord;
	vec4 gbuffer = texture2D(GBufferTexture, texCoord);
	float depth = texture2D(DepthTexture, texCoord).x;

	vec3 normal = gbuffer.xyz * 2.0 - 1.0;
	vec3 position = CalculatePosition(texCoord, depth);

	float ambientOcclusion = 0.0;

	// perform AO
	for (int i = 0; i < sample_count; ++i)
	{
		// sample at an offset specified by the current Poisson-Disk sample and scale it by a radius (has to be in Texture-Space)
		vec2 sampleTexCoord = outPoisson[i];
		float depth = texture2D(DepthTexture, sampleTexCoord).x;
		vec3 samplePos = CalculatePosition(sampleTexCoord, depth);
		vec3 sampleDir = normalize(samplePos - position);

		// angle between SURFACE-NORMAL and SAMPLE-DIRECTION (vector from SURFACE-POSITION to SAMPLE-POSITION)
		float NdotS = dot(normal, sampleDir);

		// distance between SURFACE-POSITION and SAMPLE-POSITION
		float VPdistSP = distance(position, samplePos);

		// a = distance function
		float a = 1.0 - smoothstep(DistanceThreshold, DistanceThreshold * 2.0, VPdistSP);

		// b = dot-Product
		float b = max(NdotS, 0.15);

		ambientOcclusion += (a * b);
	}

	float ao = 1.0 - (ambientOcclusion / sample_count_f);

	gl_FragColor = vec4(ao, ao, ao, 1.0);
}
