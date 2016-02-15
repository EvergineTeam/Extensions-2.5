//-----------------------------------------------------------------------------
// GrayScale.fx
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

float4 psGrayScale(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).xyz;
	float gris = dot(color, float3(0.3, 0.59, 0.11));

    return float4(gris.xxx, 1.0);
}
