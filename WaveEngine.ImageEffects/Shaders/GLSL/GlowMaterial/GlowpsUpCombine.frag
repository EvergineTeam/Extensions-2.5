// BasicEffectpsDiffuseColorTexture.frag
//
// Copyright 2012 Weekend Game Studios. All rights reserved.
// Use is subject to license terms.

#ifdef GL_ES
precision mediump float;
#endif

// Paramenters
uniform float Intensity;
uniform sampler2D Texture;
uniform sampler2D Texture1;

// Input
varying vec2 outTexCoord;

void main()
{
	vec3 color = texture2D(Texture, outTexCoord).rgb;
	vec3 glow = texture2D(Texture1, outTexCoord).rgb;

	color += glow * Intensity;
	
	gl_FragColor = vec4(color.rgb, 1.0);
}
