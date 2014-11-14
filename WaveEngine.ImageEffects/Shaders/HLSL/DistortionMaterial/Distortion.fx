//-----------------------------------------------------------------------------
// Distortion.fx
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	float Power			: packoffset(c0.x);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

Texture2D NormalTexture : register(t1);
SamplerState NormalTextureSampler : register(s1);

float4 psDistortion(VS_OUT_TEXTURE input) : SV_Target0
{
	float2 normal = NormalTexture.Sample(NormalTextureSampler, input.TexCoord).rg - 0.5;
	float2 uv = input.TexCoord + normal.xy * Power;
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, uv).rgb;

	return float4(color.xyz, 1.0);
}
