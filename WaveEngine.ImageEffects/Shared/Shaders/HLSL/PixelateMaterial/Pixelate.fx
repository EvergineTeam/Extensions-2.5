//-----------------------------------------------------------------------------
// Pixelate.fx
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	float2 PixelSize		: packoffset(c0.x);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

float4 psPixel(VS_OUT_TEXTURE input) : SV_Target0
{
	float2 uv = floor(input.TexCoord * PixelSize) / PixelSize;
	return DiffuseTexture.Sample(DiffuseTextureSampler, uv);
}
