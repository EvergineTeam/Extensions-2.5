//-----------------------------------------------------------------------------
// FastBlur.fx
//
// Copyright © 2018 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Matrices
uniform mat4	WorldViewProj;
uniform mat4	World;
uniform mat4	WorldInverseTranspose;

// Parameters
uniform vec2 TexcoordOffset;
uniform float BlurScale;
uniform sampler2D Texture;

// Input
attribute vec3 Position0;
attribute vec2 TextureCoordinate0;

// Output
varying vec4 OutUV[8];

vec2 Circle(float Start, float Points, float Point)
{
	float Rad = (3.141592 * 2.0 * (1.0 / Points)) * (Point + Start);
	return vec2(sin(Rad), cos(Rad));
}

void main()
{
	gl_Position = WorldViewProj * vec4(Position0, 1.0);

	float Start = 2.0 / 14.0;
	vec2 Scale = 0.66 * BlurScale * 2.0 * TexcoordOffset.xy;

	OutUV[0].xy = TextureCoordinate0.xy;
	OutUV[0].zw = TextureCoordinate0.xy + Circle(Start, 14.0, 0.0) * Scale; 
	OutUV[1].xy = TextureCoordinate0.xy + Circle(Start, 14.0, 1.0) * Scale; 
	OutUV[1].zw = TextureCoordinate0.xy + Circle(Start, 14.0, 2.0) * Scale; 
	OutUV[2].xy = TextureCoordinate0.xy + Circle(Start, 14.0, 3.0) * Scale; 
	OutUV[2].zw = TextureCoordinate0.xy + Circle(Start, 14.0, 4.0) * Scale; 
	OutUV[3].xy = TextureCoordinate0.xy + Circle(Start, 14.0, 5.0) * Scale; 
	OutUV[3].zw = TextureCoordinate0.xy + Circle(Start, 14.0, 6.0) * Scale; 
	OutUV[4].xy = TextureCoordinate0.xy + Circle(Start, 14.0, 7.0) * Scale; 
	OutUV[4].zw = TextureCoordinate0.xy + Circle(Start, 14.0, 8.0) * Scale; 
	OutUV[5].xy = TextureCoordinate0.xy + Circle(Start, 14.0, 9.0) * Scale; 
	OutUV[5].zw = TextureCoordinate0.xy + Circle(Start, 14.0, 10.0) * Scale;
	OutUV[6].xy = TextureCoordinate0.xy + Circle(Start, 14.0, 11.0) * Scale;
	OutUV[6].zw = TextureCoordinate0.xy + Circle(Start, 14.0, 12.0) * Scale;
	OutUV[7].xy = TextureCoordinate0.xy + Circle(Start, 14.0, 13.0) * Scale;
	OutUV[7].zw = vec2(0.0, 0.0);
}
