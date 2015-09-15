//-----------------------------------------------------------------------------
// Antialiasing.fx
//
// Copyright © 2015 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

//Paramenters
uniform vec2 TexcoordOffset;
uniform float Span_max;
uniform	float Reduce_Mul;
uniform	float Reduce_Min;
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec3 rgbNW = texture2D(Texture, outTexCoord + (vec2(-1.0, -1.0) * TexcoordOffset)).xyz;
	vec3 rgbNE = texture2D(Texture, outTexCoord + (vec2(+1.0, -1.0) * TexcoordOffset)).xyz;
	vec3 rgbSW = texture2D(Texture, outTexCoord + (vec2(-1.0, +1.0) * TexcoordOffset)).xyz;
	vec3 rgbSE = texture2D(Texture, outTexCoord + (vec2(+1.0, +1.0) * TexcoordOffset)).xyz;
	vec3 rgbM = texture2D(Texture, outTexCoord).xyz;

	vec3 luma = vec3(0.299, 0.587, 0.114);
	float lumaNW = dot(rgbNW, luma);
	float lumaNE = dot(rgbNE, luma);
	float lumaSW = dot(rgbSW, luma);
	float lumaSE = dot(rgbSE, luma);
	float lumaM = dot(rgbM, luma);

	float lumaMin = min(lumaM, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
	float lumaMax = max(lumaM, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));

	vec2 dir = vec2(-((lumaNW + lumaNE) - (lumaSW + lumaSE)), 
					((lumaNW + lumaSW) - (lumaNE + lumaSE)));

	float dirReduce = max((lumaNW + lumaNE + lumaSW + lumaSE) * (0.25 * Reduce_Mul), Reduce_Min);

	float rcpDirMin = 1.0 / (min(abs(dir.x), abs(dir.y)) + dirReduce);

	dir = min(vec2(Span_max, Span_max), max(vec2(-Span_max, -Span_max), dir * rcpDirMin)) * TexcoordOffset;

	vec3 rgbA = (1.0 / 2.0) * (
		texture2D(Texture, outTexCoord + dir * (1.0 / 3.0 - 0.5)).xyz +
		texture2D(Texture, outTexCoord + dir * (2.0 / 3.0 - 0.5)).xyz);
	vec3 rgbB = rgbA * (1.0 / 2.0) + (1.0 / 4.0) * (
		texture2D(Texture, outTexCoord + dir * (0.0 / 3.0 - 0.5)).xyz +
		texture2D(Texture, outTexCoord + dir * (3.0 / 3.0 - 0.5)).xyz);

	float lumaB = dot(rgbB, luma);

	if ((lumaB < lumaMin) || (lumaB > lumaMax))
	{
		gl_FragColor = vec4(rgbA, 1.0);
	}
	else 
	{
		gl_FragColor = vec4(rgbB, 1.0);
	}
}
