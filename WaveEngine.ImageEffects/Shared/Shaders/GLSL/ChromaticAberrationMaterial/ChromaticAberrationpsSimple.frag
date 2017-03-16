//-----------------------------------------------------------------------------
// ChromaticAberration.fx
//
// Copyright © 2017 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform float AberrationStrength;
uniform vec2 TexcoordOffset;
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec2 coords = (outTexCoord - 0.5) * 2.0;
	float coordDot = dot(coords, coords);

	vec2 compute = TexcoordOffset.xy * AberrationStrength * coordDot * coords;
	vec2 uvR = outTexCoord - compute;
	vec2 uvB = outTexCoord + compute;

	vec3 color = vec3(0,0,0);

	color.r = texture2D(Texture, uvR).r;
	color.g = texture2D(Texture, outTexCoord).g;
	color.b = texture2D(Texture, uvB).b;

	gl_FragColor = vec4(color.xyz, 1.0);
}
