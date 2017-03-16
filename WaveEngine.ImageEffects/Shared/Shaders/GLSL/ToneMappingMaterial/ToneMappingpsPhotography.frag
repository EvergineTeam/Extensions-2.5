//-----------------------------------------------------------------------------
// ToneMapping.fx
//
// Copyright © 2017 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform float Gamma;
uniform float Exposure;
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec3 color = texture2D(Texture, outTexCoord).xyz;
	color = 1.0 - exp2(-Exposure * color);
	
	gl_FragColor = vec4(color, 1.0);
}
