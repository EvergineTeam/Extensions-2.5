//-----------------------------------------------------------------------------
// Fog.fx
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision highp float;
#endif

// Parameters
uniform vec3 FogColor;
uniform float FogDensity;
uniform float ZParamA;
uniform float ZParamB;
uniform float FogStart;
uniform float FogEnd;

uniform sampler2D Texture;
uniform sampler2D DepthTexture;

// Input
varying vec2 outTexCoord;

float LinearDepth(float z)
{
	return 1.0 / (ZParamA * z + ZParamB);
}

void main()
{
	float depth = texture2D(DepthTexture, outTexCoord).x;
	vec3 scene = texture2D(Texture, outTexCoord).rgb;

	// LinearDepth, range 0 - 1
	float distance = LinearDepth(depth);

	// Calculate exponential fog amount
	float fogAmount = (FogEnd - distance) / (FogEnd - FogStart);
	fogAmount = clamp(fogAmount, 0.0, 1.0);

	// Compute resultant pixel
	vec3 color = mix(vec3(FogColor), scene, fogAmount);

	gl_FragColor = vec4(color.rgb, 1.0);
}
