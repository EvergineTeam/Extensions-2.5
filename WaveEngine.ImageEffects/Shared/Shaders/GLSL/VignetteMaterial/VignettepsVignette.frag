//-----------------------------------------------------------------------------
// Vignette.fx
//
// Copyright © 2015 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform float Power;
uniform float Radio;
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec4 color = texture2D(Texture, outTexCoord);
	vec2 dist = (outTexCoord - 0.5) * Radio;	
	dist.x = 1.0 - dot(dist, dist) * Power;
	color *= dist.x;

	gl_FragColor = color;
}
