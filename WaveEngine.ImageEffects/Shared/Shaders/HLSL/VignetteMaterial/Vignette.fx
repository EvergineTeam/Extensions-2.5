//-----------------------------------------------------------------------------
// Vignette.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	float Power			: packoffset(c0.x);
	float Radio			: packoffset(c0.y);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

float4 psVignette(VS_OUT_TEXTURE input) : SV_Target0
{
	float4 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord);
	float2 dist = (input.TexCoord - 0.5f) * Radio;
	dist.x = 1 - dot(dist, dist) * Power;
	color *= dist.x;

	return color;
}
