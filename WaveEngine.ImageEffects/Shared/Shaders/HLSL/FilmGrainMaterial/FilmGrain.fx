//-----------------------------------------------------------------------------
// FilmGrain.fx
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

cbuffer Matrices : register(b0)
{
	float4x4	WorldViewProj	: packoffset(c0);
};

cbuffer Parameters : register(b1)
{
	float4 GrainOffsetScale		: packoffset(c0);
	float Intensity				: packoffset(c1.x);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

Texture2D GrainTexture : register(t1);
SamplerState GrainTextureSampler : register(s1);

struct VS_IN_TEXTURE
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

struct VS_OUT_TEXTURE
{
	float4 Position 	: SV_POSITION;
	float2 TexCoord 	: TEXCOORD0;
	float2 Grain		: TEXCOORD1;
};

// Vertex Shader
VS_OUT_TEXTURE vsFilmGrain(VS_IN_TEXTURE input)
{
	VS_OUT_TEXTURE output = (VS_OUT_TEXTURE)0;

	output.Position = mul(input.Position, WorldViewProj);
	output.TexCoord = input.TexCoord;
	output.Grain = input.TexCoord.xy * GrainOffsetScale.zw + GrainOffsetScale.xy;
	return output;
}


float4 psFilmGrain(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).xyz;

	float3 grain = GrainTexture.Sample(GrainTextureSampler, input.Grain).xyz * 2 - 1;
	color.rgb += grain * Intensity;

	return float4(color.xyz, 1.0);
}
