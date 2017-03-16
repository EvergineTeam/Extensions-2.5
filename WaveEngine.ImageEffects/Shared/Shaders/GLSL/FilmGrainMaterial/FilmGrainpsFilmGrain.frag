//-----------------------------------------------------------------------------
// FilmGrain.fx
//
// Copyright © 2017 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform float Intensity;
uniform sampler2D Texture;
uniform sampler2D GrainTexture;

// Input
varying vec2 outTexCoord;
varying vec2 Grain;

void main()
{
	vec3 color = texture2D(Texture, outTexCoord).xyz;

	vec3 grain = texture2D(GrainTexture, Grain).xyz * 2.0 - 1.0;
	color.rgb += grain * Intensity;

	gl_FragColor = vec4(color.xyz, 1.0);
}
