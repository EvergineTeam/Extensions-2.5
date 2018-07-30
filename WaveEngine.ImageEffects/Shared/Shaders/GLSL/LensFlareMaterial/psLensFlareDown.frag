//-----------------------------------------------------------------------------
// LensFlare.fx
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Paramenters
uniform vec3 Bias;
uniform vec3 Scale;

uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec3 color = texture2D(Texture, outTexCoord).rgb + Bias;
	color = max(vec3(0.0), color) * Scale;

	gl_FragColor = vec4(color.xyz, 1.0);
}
