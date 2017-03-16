//-----------------------------------------------------------------------------
// SSAO.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform float DistanceThreshold;
uniform float AOintensity;		
uniform vec2 FilterRadius;
uniform mat4 ViewProjectionInverse;

uniform sampler2D Texture;
uniform sampler2D AOTexture;
uniform sampler2D GBufferTexture;
uniform sampler2D DepthTexture;


// Input
varying vec2 outTexCoord;

void main()
{
	vec4 color = texture2D(Texture, outTexCoord);

	float ao = texture2D(AOTexture, outTexCoord).x;
	ao = pow(ao, AOintensity);

	color.xyz *= ao;

	gl_FragColor = color;
}
