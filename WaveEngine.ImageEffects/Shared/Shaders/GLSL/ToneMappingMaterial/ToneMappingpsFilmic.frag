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
	color = max(vec3(0.0, 0.0, 0.0), color - 0.004);
	color = (color * (6.2 * color + 0.5)) / (color * (6.2 * color + 1.7) + 0.06);
	
	gl_FragColor = vec4(color, 1.0);
}
