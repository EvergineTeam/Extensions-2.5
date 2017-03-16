//-----------------------------------------------------------------------------
// ToneMapping.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

// More info:
// https://www.shadertoy.com/view/lslGzl
// http://filmicgames.com/archives/75
// http://filmicgames.com/archives/183
// http://filmicgames.com/archives/190
// http://imdoingitwrong.wordpress.com/2010/08/19/why-reinhard-desaturates-my-blacks-3/
// http://mynameismjp.wordpress.com/2010/04/30/a-closer-look-at-tone-mapping/
// http://renderwonk.com/publications/s2010-color-course/

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	float Gamma 	: packoffset(c0.x);
	float Exposure	: packoffset(c0.y);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

float4 psLinear(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).xyz;
	
	color *= Exposure;
	color = pow(color, 1.0 / Gamma);
	
	return float4(color, 1.0);
}


float4 psSimpleReinhard(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).xyz;
	
	color *= Exposure / (1.0 + (color / Exposure));
	color = pow(color, 1.0 / Gamma);
	
	return float4(color, 1.0);
}

float4 psLumaBasedReinhard(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).xyz;
	
	color *= Exposure;
	float luma = dot(color, float3(0.2126, 0.7152, 0.0722));
	float toneMappedLuma = luma / (1.0 + luma);
	color *= toneMappedLuma / luma;
	color = pow(color, 1.0 / Gamma);
	
	return float4(color, 1.0);
}

float4 psWhitePreservingLumaBasedReinhard(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).xyz;
	
	color *= Exposure;
	float white = 2;
	float luma = dot(color, float3(0.2126, 0.7152, 0.0722));
	float toneMappedLuma = luma * (1.0 + luma / (white * white)) / (1.0 + luma);
	color *= toneMappedLuma / luma;
	color = pow(color, 1.0 / Gamma);
	
	return float4(color, 1.0);
}

float4 psRombinDaHouse(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).xyz;
	
	color *= Exposure;
	color = exp( -1.0 / (2.72 * color + 0.15) );
	color = pow(color, 1.0 / Gamma);
	
	return float4(color, 1.0);
}

float4 psPhotography(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).xyz;

	color = 1.0 - exp2(-Exposure * color);
	
	return float4(color, 1.0);
}

float4 psFilmic(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).xyz;
	
	color *= Exposure;
	color = max(0.0, color - 0.004);
	color = (color * (6.2 * color + 0.5)) / (color * (6.2 * color + 1.7) + 0.06);
	
	return float4(color, 1.0);
}

float4 psUncharted2(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).xyz;
	
	color *= Exposure;
	
	float A = 0.15;
	float B = 0.50;
	float C = 0.10;
	float D = 0.20;
	float E = 0.02;
	float F = 0.30;
	float W = 11.2;
	
	float exposureBias = 2.0f;
	color *= exposureBias;
	color = ((color * (A * color + C * B) + D * E) / (color * (A * color + B) + D * F)) - E / F;
	float white = ((W * (A * W + C * B) + D * E) / (W * (A * W + B) + D * F)) - E / F;
	color /= white;
	color = pow(color, 1.0 / Gamma);
	
	return float4(color, 1.0);
}
