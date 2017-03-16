//-----------------------------------------------------------------------------
// GaussianBlur.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Matrices
uniform mat4	WorldViewProj;

// Parameters
uniform vec2 SampleOffsets0;
uniform vec2 SampleOffsets1;
uniform vec2 SampleOffsets2;
uniform vec2 SampleOffsets3;
uniform vec2 SampleOffsets4;
uniform vec2 SampleOffsets5;
uniform vec2 SampleOffsets6;
uniform vec2 SampleOffsets7;
uniform vec2 SampleOffsets8;
uniform vec2 SampleOffsets9;
uniform vec2 SampleOffsets10;
uniform vec2 SampleOffsets11;
uniform vec2 SampleOffsets12;
uniform vec2 SampleOffsets13;

uniform sampler2D Texture;

// Input
attribute vec3 Position0;
attribute vec2 TextureCoordinate0;

// Output
varying vec4 OutUV[7];

void main()
{
	gl_Position = WorldViewProj * vec4(Position0, 1.0);

	OutUV[0].xy = TextureCoordinate0.xy + SampleOffsets0;
	OutUV[0].zw = TextureCoordinate0.xy + SampleOffsets1;
	OutUV[1].xy = TextureCoordinate0.xy + SampleOffsets2;
	OutUV[1].zw = TextureCoordinate0.xy + SampleOffsets3;
	OutUV[2].xy = TextureCoordinate0.xy + SampleOffsets4;
	OutUV[2].zw = TextureCoordinate0.xy + SampleOffsets5;
	OutUV[3].xy = TextureCoordinate0.xy + SampleOffsets6;
	OutUV[3].zw = TextureCoordinate0.xy + SampleOffsets7;
	OutUV[4].xy = TextureCoordinate0.xy + SampleOffsets8;
	OutUV[4].zw = TextureCoordinate0.xy + SampleOffsets9;
	OutUV[5].xy = TextureCoordinate0.xy + SampleOffsets10;
	OutUV[5].zw = TextureCoordinate0.xy + SampleOffsets11;
	OutUV[6].xy = TextureCoordinate0.xy + SampleOffsets12;
	OutUV[6].zw = TextureCoordinate0.xy + SampleOffsets13;
}
