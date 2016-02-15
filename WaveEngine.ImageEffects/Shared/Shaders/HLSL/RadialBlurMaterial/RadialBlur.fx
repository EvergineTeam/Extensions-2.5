//-----------------------------------------------------------------------------
// RadialBlur.fx
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	int Nsamples			: packoffset(c0.x);
	float BlurWidth			: packoffset(c0.y);
	float2 Center			: packoffset(c0.z);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

float4 psRadialBlur(VS_OUT_TEXTURE input) : SV_Target0
{
	half2 uv = input.TexCoord - Center;
	half precompute = BlurWidth * (1.0 / half(Nsamples - 1));

	half3 color = (half3)0;

	[unroll(32)]
	for (int i = 0; i < Nsamples; i++)
	{
		half scale = 1.0 + (half(i) * precompute);
		color += DiffuseTexture.Sample(DiffuseTextureSampler, uv * scale + Center).xyz;
	}

	color /= half(Nsamples);

	return float4(color, 1.0);
}
