//-----------------------------------------------------------------------------
// Distortion.fx
//
// Copyright © 2018 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform float Power;
uniform sampler2D Texture;
uniform sampler2D Normal;

// Input
varying vec2 outTexCoord;

void main()
{
	vec2 normal = texture2D(Normal, outTexCoord).rg - 0.5;
	vec2 uv = outTexCoord + normal.xy * Power;
	vec3 color = texture2D(Texture, uv).rgb;

	gl_FragColor = vec4(color.rgb, 1.0);
}
