//-----------------------------------------------------------------------------
// DepthOfField.fx
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Paramenters
uniform float FocusDistance;
uniform float FocusRange;
uniform float NearPlane;
uniform float FarParam;
	
uniform sampler2D Texture;
uniform sampler2D Texture1;
uniform sampler2D DepthTexture;

// Input
varying vec2 outTexCoord;

void main()
{
	float depth = texture2D(DepthTexture, outTexCoord).x;
	vec3 scene = texture2D(Texture, outTexCoord).rgb;
	vec3 blur = texture2D(Texture1, outTexCoord).rgb;

	// Back-transform depth into camera space
	float sceneZ = (-NearPlane * FarParam) / (depth - FarParam);

	// Compute blur factor
	float blurFactor = clamp(abs(sceneZ - FocusDistance) / FocusRange, 0.0, 1.0);

	// Compute resultant pixel
	vec3 color = mix(scene, blur, blurFactor);
	
	gl_FragColor = vec4(color.rgb, 1.0);
}
