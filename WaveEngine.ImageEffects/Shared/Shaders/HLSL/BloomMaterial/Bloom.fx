//-----------------------------------------------------------------------------
// Bloom.fx
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

cbuffer Matrices : register(b0)
{
	float4x4	WorldViewProj				: packoffset(c0);
};

cbuffer Parameters : register(b1)
{
	float2 TexcoordOffset		: packoffset(c0.x);
	float BloomThreshold		: packoffset(c0.z);
	float BloomScale			: packoffset(c0.w);
	float3 BloomTint			: packoffset(c1.x);
	float Intensity				: packoffset(c1.w);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

Texture2D DiffuseTexture1 : register(t1);
SamplerState DiffuseTextureSampler1 : register(s1);

struct VS_IN_TEXTURE
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

struct VS_OUT_TEXTURE
{
	float4 OutUV[8]			: TEXCOORD0;
	float4 Position 		: SV_POSITION;
};

struct PS_IN_TEXTURE
{
	float4 Position 	: SV_POSITION;
	float2 TexCoord 	: TEXCOORD0;
};

struct PS_IN_TEXTUREWEIGTH
{
	float4 OutUV[8]			: TEXCOORD0;
};

float2 Circle(float Start, float Points, float Point)
{
	float Rad = (3.141592 * 2.0 * (1.0 / Points)) * (Point + Start);
	return float2(sin(Rad), cos(Rad));
}

float Luminance(float3 LinearColor)
{
	return dot(LinearColor, float3(0.3, 0.59, 0.11));
}

// Down sampler
float4 psBloomDown(PS_IN_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;

	half TotalLuminance = Luminance(color.rgb);
	half BloomLuminance = TotalLuminance - BloomThreshold;
	half Amount = saturate(BloomLuminance * 0.5);
	color.rgb *= Amount;

	return float4(color.rgb, 1.0);
}

// Bloom
VS_OUT_TEXTURE vsBloom(VS_IN_TEXTURE input)
{
	VS_OUT_TEXTURE output = (VS_OUT_TEXTURE)0;

	output.Position = mul(input.Position, WorldViewProj);

	float Start = 2.0 / 14.0;
	float2 Scale = 0.66 * BloomScale * 2.0 * TexcoordOffset.xy;

	output.OutUV[0].xy = input.TexCoord.xy;
	output.OutUV[0].zw = input.TexCoord.xy + Circle(Start, 14.0, 0.0) * Scale; 
	output.OutUV[1].xy = input.TexCoord.xy + Circle(Start, 14.0, 1.0) * Scale; 
	output.OutUV[1].zw = input.TexCoord.xy + Circle(Start, 14.0, 2.0) * Scale; 
	output.OutUV[2].xy = input.TexCoord.xy + Circle(Start, 14.0, 3.0) * Scale; 
	output.OutUV[2].zw = input.TexCoord.xy + Circle(Start, 14.0, 4.0) * Scale; 
	output.OutUV[3].xy = input.TexCoord.xy + Circle(Start, 14.0, 5.0) * Scale; 
	output.OutUV[3].zw = input.TexCoord.xy + Circle(Start, 14.0, 6.0) * Scale; 
	output.OutUV[4].xy = input.TexCoord.xy + Circle(Start, 14.0, 7.0) * Scale; 
	output.OutUV[4].zw = input.TexCoord.xy + Circle(Start, 14.0, 8.0) * Scale; 
	output.OutUV[5].xy = input.TexCoord.xy + Circle(Start, 14.0, 9.0) * Scale; 
	output.OutUV[5].zw = input.TexCoord.xy + Circle(Start, 14.0, 10.0) * Scale;
	output.OutUV[6].xy = input.TexCoord.xy + Circle(Start, 14.0, 11.0) * Scale;
	output.OutUV[6].zw = input.TexCoord.xy + Circle(Start, 14.0, 12.0) * Scale;
	output.OutUV[7].xy = input.TexCoord.xy + Circle(Start, 14.0, 13.0) * Scale;
	output.OutUV[7].zw = float2(0.0, 0.0);

	return output;
}

float4 psBloom(PS_IN_TEXTUREWEIGTH input) : SV_Target0
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
	half3 N14 = DiffuseTexture.Sample(DiffuseTextureSampler, input.OutUV[7].xy).rgb;

	half W = 1.0 / 15.0;
	
	float3 color = (float3)0;

	color.rgb =
		(N0 * W) +
		(N1 * W) +
		(N2 * W) +
		(N3 * W) +
		(N4 * W) +
		(N5 * W) +
		(N6 * W) +
		(N7 * W) +
		(N8 * W) +
		(N9 * W) +
		(N10 * W) +
		(N11 * W) +
		(N12 * W) +
		(N13 * W) +
		(N14 * W);

	return float4(color.rgb, 1.0);
}

// Up sampler and combine
float4 psUpCombine(PS_IN_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;
	float3 bloom = DiffuseTexture1.Sample(DiffuseTextureSampler1, input.TexCoord).rgb;

	color += bloom * BloomTint * Intensity;
	
	return float4(color.rgb, 1.0);
}
