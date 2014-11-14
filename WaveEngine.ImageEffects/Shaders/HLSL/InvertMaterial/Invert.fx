//-----------------------------------------------------------------------------
// Invert.fx
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

float4 psInvert(VS_OUT_TEXTURE input) : SV_Target0
{
	return 1 - DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord);
}
