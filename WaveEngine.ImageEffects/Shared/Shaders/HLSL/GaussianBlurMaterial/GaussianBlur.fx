//-----------------------------------------------------------------------------
// GaussianBlur.fx
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

cbuffer Matrices : register(b0)
{
	float4x4	WorldViewProj				: packoffset(c0);
};

cbuffer Parameters : register(b1)
{
	float2 SampleOffsets0		: packoffset(c0.x);
	float2 SampleOffsets1		: packoffset(c0.z);
	float2 SampleOffsets2		: packoffset(c1.x);
	float2 SampleOffsets3		: packoffset(c1.z);	
	float2 SampleOffsets4		: packoffset(c2.x);
	float2 SampleOffsets5		: packoffset(c2.z);
	float2 SampleOffsets6		: packoffset(c3.x);
	float2 SampleOffsets7		: packoffset(c3.z);
	float2 SampleOffsets8		: packoffset(c4.x);
	float2 SampleOffsets9		: packoffset(c4.z);
	float2 SampleOffsets10		: packoffset(c5.x);
	float2 SampleOffsets11		: packoffset(c5.z);
	float2 SampleOffsets12		: packoffset(c6.x);
	float2 SampleOffsets13		: packoffset(c6.z);

	float SampleWeights0		: packoffset(c7.x);
	float SampleWeights1		: packoffset(c7.y);
	float SampleWeights2		: packoffset(c7.z);
	float SampleWeights3		: packoffset(c7.w);
	float SampleWeights4		: packoffset(c8.x);
	float SampleWeights5		: packoffset(c8.y);
	float SampleWeights6		: packoffset(c8.z);
	float SampleWeights7		: packoffset(c8.w);
	float SampleWeights8		: packoffset(c9.x);
	float SampleWeights9		: packoffset(c9.y);
	float SampleWeights10		: packoffset(c9.z);
	float SampleWeights11		: packoffset(c9.w);
	float SampleWeights12		: packoffset(c10.x);
	float SampleWeights13		: packoffset(c10.y);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

struct VS_IN_TEXTURE
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

struct VS_OUT_TEXTURE
{
	float4 OutUV[7]			: TEXCOORD0;
	float4 Position 		: SV_POSITION;
};

struct PS_IN_TEXTURE
{
	float4 OutUV[7]			: TEXCOORD0;
};

VS_OUT_TEXTURE vsGaussianBlur(VS_IN_TEXTURE input)
{
	VS_OUT_TEXTURE output = (VS_OUT_TEXTURE)0;

	output.Position = mul(input.Position, WorldViewProj);

	output.OutUV[0].xy = input.TexCoord.xy + SampleOffsets0;
	output.OutUV[0].zw = input.TexCoord.xy + SampleOffsets1;
	output.OutUV[1].xy = input.TexCoord.xy + SampleOffsets2;
	output.OutUV[1].zw = input.TexCoord.xy + SampleOffsets3;
	output.OutUV[2].xy = input.TexCoord.xy + SampleOffsets4;
	output.OutUV[2].zw = input.TexCoord.xy + SampleOffsets5;
	output.OutUV[3].xy = input.TexCoord.xy + SampleOffsets6;
	output.OutUV[3].zw = input.TexCoord.xy + SampleOffsets7;
	output.OutUV[4].xy = input.TexCoord.xy + SampleOffsets8;
	output.OutUV[4].zw = input.TexCoord.xy + SampleOffsets9;
	output.OutUV[5].xy = input.TexCoord.xy + SampleOffsets10;
	output.OutUV[5].zw = input.TexCoord.xy + SampleOffsets11;
	output.OutUV[6].xy = input.TexCoord.xy + SampleOffsets12;
	output.OutUV[6].zw = input.TexCoord.xy + SampleOffsets13;


	return output;
}

float4 psGaussianBlur(PS_IN_TEXTURE input) : SV_Target0
{
	half3 N0 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[0].xy).rgb;
	half3 N1 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[0].zw).rgb;
	half3 N2 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[1].xy).rgb;
	half3 N3 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[1].zw).rgb;
	half3 N4 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[2].xy).rgb;
	half3 N5 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[2].zw).rgb;
	half3 N6 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[3].xy).rgb;
	half3 N7 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[3].zw).rgb;
	half3 N8 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[4].xy).rgb;
	half3 N9 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[4].zw).rgb;
	half3 N10 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[5].xy).rgb;
	half3 N11 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[5].zw).rgb;
	half3 N12 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[6].xy).rgb;
	half3 N13 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[6].zw).rgb;

	float3 color = (float3)0;

	color.rgb =
	(N0 * SampleWeights0) +
	(N1 * SampleWeights1) +
	(N2 * SampleWeights2) +
	(N3 * SampleWeights3) +
	(N4 * SampleWeights4) +
	(N5 * SampleWeights5) +
	(N6 * SampleWeights6) +
	(N7 * SampleWeights7) +
	(N8 * SampleWeights8) +
	(N9 * SampleWeights9) +
	(N10 * SampleWeights10) +
	(N11 * SampleWeights11) +
	(N12 * SampleWeights12) +
	(N13 * SampleWeights13);

	return float4(color.rgb, 1.0);
}
