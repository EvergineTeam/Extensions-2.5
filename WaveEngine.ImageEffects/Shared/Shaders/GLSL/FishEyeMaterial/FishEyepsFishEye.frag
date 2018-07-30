//-----------------------------------------------------------------------------
// FishEye.fx
//
// Copyright © 2018 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------


#ifdef GL_ES
precision mediump float;
#endif

// Paramenters
uniform vec2 Intensity;
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec2 coords = (outTexCoord - 0.5) * 2.0;

	vec2 realCoordOffs;
	realCoordOffs.x = (1.0 - coords.y * coords.y) * Intensity.y * (coords.x);
	realCoordOffs.y = (1.0 - coords.x * coords.x) * Intensity.x * (coords.y);

	vec3 color = texture2D(Texture, outTexCoord - realCoordOffs).rgb;

	gl_FragColor = vec4(color.rgb, 1.0);
}
