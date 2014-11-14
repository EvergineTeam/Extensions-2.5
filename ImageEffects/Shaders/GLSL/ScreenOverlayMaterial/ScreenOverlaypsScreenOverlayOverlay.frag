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

float Luminance(vec3 LinearColor)
{
	return dot(LinearColor, vec3(0.3, 0.59, 0.11));
}

void main()
{
	vec3 color = texture2D(Texture, outTexCoord).rgb;
	vec3 overlay = texture2D(OverlayTexture, outTexCoord).rgb;
	
	if(Luminance(overlay) < 0.5)
	{
		color = vec3(2) * color * overlay;
	}
	else
	{
		color = vec3(1.0) - ((vec3(1.0) - color) * (vec3(1.0) - overlay));
	}
	
	gl_FragColor = vec4(color.rgb * Intensity, 1.0);
}
