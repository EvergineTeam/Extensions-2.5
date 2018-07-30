//-----------------------------------------------------------------------------
// ScreenOverlay.fx
//
// Copyright © 2018 Wave Corporation
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
	
	color += overlay;
	
	gl_FragColor = vec4(color.rgb * Intensity, 1.0);
}
