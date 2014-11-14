//-----------------------------------------------------------------------------
// ColorCorrection.fx
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	float Scale			: packoffset(c0.x);
	float Offset		: packoffset(c0.y);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

Texture3D LUTTexture : register(t1);
SamplerState LUTTextureSampler : register(s1);

float4 psColorCorrection(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;
	color = LUTTexture.Sample(LUTTextureSampler, color * Scale + Offset).rgb;

	return float4(color, 1.0);
}

float4 psColorCorrectionLinear(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;
	color = sqrt(color);
	color = LUTTexture.Sample(LUTTextureSampler, color * Scale + Offset).rgb;
	color *= color;

	return float4(color, 1.0);
}