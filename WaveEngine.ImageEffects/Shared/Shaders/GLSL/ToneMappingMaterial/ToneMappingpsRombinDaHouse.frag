//-----------------------------------------------------------------------------
// ToneMapping.fx
//
// Copyright © 2016 Wave Corporation
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
	
	color *= Exposure;
	color = exp( -1.0 / (2.72 * color + 0.15) );
	color = pow(color, vec3(1.0 / Gamma));
	
	gl_FragColor = vec4(color, 1.0);
}
