//-----------------------------------------------------------------------------
// ChromaticAberration.fx
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	float AberrationStrength : packoffset(c0.x);
	float2 TexcoordOffset		: packoffset (c0.y);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

float4 psSimple(VS_OUT_TEXTURE input) : SV_Target0
{
	float2 coords = (input.TexCoord - 0.5) * 2.0;
	float coordDot = dot(coords, coords);

	float2 compute = TexcoordOffset.xy * AberrationStrength * coordDot * coords;
	float2 uvR = input.TexCoord - compute;
	float2 uvB = input.TexCoord + compute;

	float3 color = float3(0,0,0);

	color.r = DiffuseTexture.Sample(DiffuseTextureSampler, uvR).r;
	color.g = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).g;
	color.b = DiffuseTexture.Sample(DiffuseTextureSampler, uvB).b;

	return float4(color.xyz, 1.0);
}
