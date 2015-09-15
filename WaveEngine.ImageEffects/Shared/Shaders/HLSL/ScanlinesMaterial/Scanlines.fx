//-----------------------------------------------------------------------------
// Scanlines.fx
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	float LinesFactor		: packoffset(c0.x);
	float Attenuation		: packoffset (c0.y);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

float4 psScanlines(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord);

	float scanline = sin(input.TexCoord.y * LinesFactor) * Attenuation;
	color -= scanline;

	return float4(color.xyz, 1.0);
}
