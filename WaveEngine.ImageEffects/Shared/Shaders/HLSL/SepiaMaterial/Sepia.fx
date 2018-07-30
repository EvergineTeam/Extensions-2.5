//-----------------------------------------------------------------------------
// Sepia.fx
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	float3 ImageTone		: packoffset(c0.x);
	float Desaturation		: packoffset(c0.w);
	float3 DarkTone			: packoffset(c1.x);
	float Toning			: packoffset(c1.w);
	float3 GreyTransfer		: packoffset(c2.x);
	float GlobalAlpha		: packoffset(c2.w);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

float4 psSepia(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 pixelColor = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;

	float3 scene = pixelColor * ImageTone;

	float grey = dot(GreyTransfer, scene);
	float3 muted = lerp(scene, grey.xxx, Desaturation);
	float3 sepia = lerp(DarkTone, ImageTone, grey);
	pixelColor = lerp(muted, sepia, Toning);

	// return final pixel color
	return float4(pixelColor, GlobalAlpha);
}
