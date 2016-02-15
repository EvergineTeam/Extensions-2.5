//-----------------------------------------------------------------------------
// Posterize.fx
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	float Gamma			: packoffset(c0.x);
	float Regions		: packoffset (c0.y);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

float4 psPosterize(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;

	color = pow(color, Gamma);
	color = floor(color * Regions) / Regions;
	color = pow(color, 1.0 / Gamma);

	return float4(color.xyz, 1.0);
}
