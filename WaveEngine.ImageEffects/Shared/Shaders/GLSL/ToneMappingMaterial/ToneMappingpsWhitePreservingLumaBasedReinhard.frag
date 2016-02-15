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
	float white = 2.0;
	float luma = dot(color, vec3(0.2126, 0.7152, 0.0722));
	float toneMappedLuma = luma * (1.0 + luma / (white * white)) / (1.0 + luma);
	color *= toneMappedLuma / luma;
	color = pow(color, vec3(1.0 / Gamma));
	
	gl_FragColor = vec4(color, 1.0);
}
