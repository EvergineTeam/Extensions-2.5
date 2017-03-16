//-----------------------------------------------------------------------------
// Pixelate.fx
//
// Copyright © 2017 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform vec2 PixelSize;
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec2 uv = floor(outTexCoord * PixelSize) / PixelSize;
	gl_FragColor = texture2D(Texture, uv);
}
