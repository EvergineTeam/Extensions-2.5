//-----------------------------------------------------------------------------
// ImageEffect.fx
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Matrices : register(b0)
{
    float4x4	WorldViewProj				: packoffset(c0);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

VS_OUT_TEXTURE vsImageEffect(VS_IN_TEXTURE input)
{
	VS_OUT_TEXTURE output = (VS_OUT_TEXTURE)0;

    output.Position = mul(input.Position, WorldViewProj);
	output.TexCoord = input.TexCoord;

    return output;
}

float4 psImageEffect(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;
	return float4(color.rgb, 1.0);
}