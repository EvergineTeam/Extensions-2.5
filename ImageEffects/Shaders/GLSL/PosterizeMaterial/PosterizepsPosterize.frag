//-----------------------------------------------------------------------------
// Posterize.fx
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform float Gamma;
uniform float Regions;
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec3 color = texture2D(Texture, outTexCoord).rgb;
	color = pow(color, vec3(Gamma));
	color = floor(color * Regions) / Regions;
	color = pow(color, vec3(1.0 / Gamma));

	gl_FragColor = vec4(color.rgb, 1.0);
}
