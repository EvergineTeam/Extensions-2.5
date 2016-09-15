//-----------------------------------------------------------------------------
// Bokeh.fx
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision highp float;
#endif

// Parameters
uniform vec2 BlurDisp;
uniform float Aperture;  
uniform float LastCoeff;		
uniform float FocalDistance;			
uniform float NearPlane;
uniform float FarParam;		
uniform float FilmWidth;

uniform sampler2D Texture;
uniform sampler2D DepthTexture;

// Input
varying vec2 outTexCoord;

void main()
{
	float depth = texture2D(DepthTexture, outTexCoord).x;
	float S2 = (-NearPlane * FarParam) / (depth - FarParam);

	// Calculate circle of confusion diameter
	// https://en.wikipedia.org/wiki/Circle_of_confusion
	float coc = abs(Aperture * ((S2 - FocalDistance) / S2) * LastCoeff); // (f / (S1 - f)));

	// put CoC into a % of the image sensor height
	float blurFactor = coc / FilmWidth;

	vec3 color = texture2D(Texture, outTexCoord).rgb;

	gl_FragColor = vec4(color, blurFactor);
}
