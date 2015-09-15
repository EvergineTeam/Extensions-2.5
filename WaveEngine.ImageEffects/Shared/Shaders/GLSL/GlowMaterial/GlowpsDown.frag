//-----------------------------------------------------------------------------
// Glow.fx
//
// Copyright © 2015 Wave Corporation
// Use is subject to license terms.
// More info: http://http.developer.nvidia.com/GPUGems/gpugems_ch21.html
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Paramenters
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec4 color = texture2D(Texture, outTexCoord);
	vec3 result = color.rgb * color.a;

	gl_FragColor = vec4(result.xyz, 1.0);
}
