//-----------------------------------------------------------------------------
// Bloom.fx
//
// Copyright © 2017 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Paramenters
uniform vec3 BloomTint;
uniform float Intensity;	
uniform sampler2D Texture;
uniform sampler2D Texture1;

// Input
varying vec2 outTexCoord;

void main()
{
	vec3 color = texture2D(Texture, outTexCoord).rgb;
	vec3 bloom = texture2D(Texture1, outTexCoord).rgb;

	color += bloom * BloomTint * Intensity;
	
	gl_FragColor = vec4(color.rgb, 1.0);
}
