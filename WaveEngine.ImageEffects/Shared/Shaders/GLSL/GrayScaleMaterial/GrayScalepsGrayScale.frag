//-----------------------------------------------------------------------------
// GrayScale.fx
//
// Copyright © 2017 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec3 color = texture2D(Texture, outTexCoord).xyz;
	float gris = dot(color, vec3(0.3, 0.59, 0.11));

    gl_FragColor = vec4(gris, gris, gris, 1.0);
}
