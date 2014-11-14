//-----------------------------------------------------------------------------
// ScreenOverlay.fx
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform float Intensity;
uniform sampler2D Texture;
uniform sampler2D OverlayTexture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec3 color = texture2D(Texture, outTexCoord).rgb;
	vec3 overlay = texture2D(OverlayTexture, outTexCoord).rgb;
	
	color.rgb = vec3(1.0) - ((vec3(1.0) - color.rgb) * (vec3(1.0) - overlay.rgb));
	
	gl_FragColor = vec4(color.rgb * Intensity, 1.0);
}