//-----------------------------------------------------------------------------
// ImageEffect.fx
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------


struct VS_IN_TEXTURE
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

struct VS_OUT_TEXTURE
{
	float4 Position 	: SV_POSITION;
	float2 TexCoord 	: TEXCOORD0;
};

cbuffer Matrices : register(b0)
{
    float4x4	WorldViewProj				: packoffset(c0);
};

Texture2D LuminanceTexture : register(t0);
Texture2D ChromaTexture: register(t1);
Texture2D FinalTexture: register(t2);

SamplerState Sampler : register(s0);

VS_OUT_TEXTURE vsYUVMaterial(VS_IN_TEXTURE input)
{
	VS_OUT_TEXTURE output = (VS_OUT_TEXTURE)0;

    output.Position = mul(input.Position, WorldViewProj);
	output.TexCoord = input.TexCoord;

    return output;
}

float4 psYUVMaterial(VS_OUT_TEXTURE input) : SV_Target0
{
	float Y= LuminanceTexture.Sample(Sampler, input.TexCoord).r;
	float2 map = float2(input.TexCoord.x/2,input.TexCoord.y/2 );

	float4 v = ChromaTexture.Sample(Sampler, map);
	float cr, cb;
	
	cr = v.r - 0.5;
	cb = v.g - 0.5;

	float3x3 mat = float3x3(1,        1,       1,
							0, -0.343, 1.765,
							1.4, -0.711,   0);

	float3 rgb = mul( float3(Y, cr, cb), mat);

	return float4(rgb, 1);
}

