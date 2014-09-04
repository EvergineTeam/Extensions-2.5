//-----------------------------------------------------------------------------
// Bloom.fx
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Paramenters
uniform float BloomThreshold;
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

float Luminance(vec3 LinearColor)
{
	return dot(LinearColor, vec3(0.3, 0.59, 0.11));
}

void main()
{
	vec3 color = texture2D(Texture, outTexCoord).rgb;

	float TotalLuminance = Luminance(color.rgb);
	float BloomLuminance = TotalLuminance - BloomThreshold;
	float Amount = clamp(BloomLuminance * 0.5, 0.0, 1.0);
	color.rgb *= Amount;

	gl_FragColor = vec4(color.xyz, 1.0);
}
