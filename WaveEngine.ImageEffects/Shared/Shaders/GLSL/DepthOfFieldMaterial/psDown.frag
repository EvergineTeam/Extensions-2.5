//-----------------------------------------------------------------------------
// DepthOfField.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
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
	vec4 color = texture2D(Texture, outTexCoord).rgba;
	vec3 result = color.rgb * color.a;
	
	gl_FragColor = vec4(result.rgb, 1.0);
}
