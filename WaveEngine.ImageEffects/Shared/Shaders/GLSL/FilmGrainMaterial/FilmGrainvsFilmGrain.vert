//-----------------------------------------------------------------------------
// FilmGrain.fx
//
// Copyright © 2017 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

uniform mat4	WorldViewProj;

// Parameters
uniform vec4 GrainOffsetScale;

// Input
attribute vec3 Position0;
attribute vec2 TextureCoordinate0;

// Output
varying vec2 outTexCoord;
varying vec2 Grain;

void main()
{
	gl_Position = WorldViewProj * vec4(Position0, 1.0);
	outTexCoord = TextureCoordinate0;
	Grain = TextureCoordinate0.xy * GrainOffsetScale.zw + GrainOffsetScale.xy;
}
