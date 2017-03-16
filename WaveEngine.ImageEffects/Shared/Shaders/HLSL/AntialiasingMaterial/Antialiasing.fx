//-----------------------------------------------------------------------------
// Antialiasing.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	float2 TexcoordOffset	: packoffset(c0.x);
	float Span_max			: packoffset(c0.z);
	float Reduce_Mul		: packoffset(c0.w);
	float Reduce_Min		: packoffset(c1.x);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

// Whitepaper describing the technique
// http://developer.download.nvidia.com/assets/gamedev/files/sdk/11/FXAA_WhitePaper.pdf
float4 psFXAA(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 rgbNW = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord + (float2(-1.0, -1.0) * TexcoordOffset)).xyz;
	float3 rgbNE = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord + (float2(+1.0, -1.0) * TexcoordOffset)).xyz;
	float3 rgbSW = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord + (float2(-1.0, +1.0) * TexcoordOffset)).xyz;
	float3 rgbSE = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord + (float2(+1.0, +1.0) * TexcoordOffset)).xyz;
	float3 rgbM = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).xyz;

	float3 luma = float3(0.299, 0.587, 0.114);
	float lumaNW = dot(rgbNW, luma);
	float lumaNE = dot(rgbNE, luma);
	float lumaSW = dot(rgbSW, luma);
	float lumaSE = dot(rgbSE, luma);
	float lumaM = dot(rgbM, luma);

	float lumaMin = min(lumaM, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
	float lumaMax = max(lumaM, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));

	float2 dir = float2(-((lumaNW + lumaNE) - (lumaSW + lumaSE)), 
						((lumaNW + lumaSW) - (lumaNE + lumaSE)));

	float dirReduce = max((lumaNW + lumaNE + lumaSW + lumaSE) * (0.25 * Reduce_Mul), Reduce_Min);

	float rcpDirMin = 1.0 / (min(abs(dir.x), abs(dir.y)) + dirReduce);

	dir = min(float2(Span_max, Span_max), max(float2(-Span_max, -Span_max), dir * rcpDirMin)) * TexcoordOffset;

	float3 rgbA = (1.0 / 2.0) * (
		DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord + dir * (1.0 / 3.0 - 0.5)).xyz +
		DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord + dir * (2.0 / 3.0 - 0.5)).xyz);
	float3 rgbB = rgbA * (1.0 / 2.0) + (1.0 / 4.0) * (
		DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord + dir * (0.0 / 3.0 - 0.5)).xyz +
		DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord + dir * (3.0 / 3.0 - 0.5)).xyz);

	float lumaB = dot(rgbB, luma);

	if ((lumaB < lumaMin) || (lumaB > lumaMax))
	{
		return float4(rgbA, 1.0);
	}
	else 
	{
		return float4(rgbB, 1.0);
	}
}
