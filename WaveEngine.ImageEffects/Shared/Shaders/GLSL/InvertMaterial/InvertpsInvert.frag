//-----------------------------------------------------------------------------
// Invert.fx
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
	gl_FragColor = 1.0 - texture2D(Texture, outTexCoord);
}
