//-----------------------------------------------------------------------------
// FishEye.fx
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	float2 Intensity		: packoffset(c0.x);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

float4 psFishEye(VS_OUT_TEXTURE input) : SV_Target0
{
	half2 coords = (input.TexCoord - 0.5) * 2.0;

	half2 realCoordOffs;
	realCoordOffs.x = (1.0 - coords.y * coords.y) * Intensity.y * (coords.x);
	realCoordOffs.y = (1.0 - coords.x * coords.x) * Intensity.x * (coords.y);

	half3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord - realCoordOffs).rgb;

	return float4(color.rgb, 1.0);
}
