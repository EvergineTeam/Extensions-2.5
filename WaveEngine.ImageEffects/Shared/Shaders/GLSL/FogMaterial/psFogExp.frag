//-----------------------------------------------------------------------------
// Fog.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
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
	float depth = texture2D(DepthTexture, outTexCoord).r;
	depth = depth  * 2.0 - 1.0;	

//	vec3 scene = texture2D(Texture, outTexCoord).rgb;

	// LinearDepth, range 0 - 1
	//float distance = LinearDepth(depth);

	gl_FragColor = vec4(depth.xxx, 1.0);

//	// Calculate exponential fog amount
//	float fogAmount = exp2(-distance * FogDensity);
//
//	// Compute resultant pixel
//	vec3 color = mix(vec3(FogColor), scene, fogAmount);
//
//	gl_FragColor = vec4(distance.xxx, 1.0);
}
