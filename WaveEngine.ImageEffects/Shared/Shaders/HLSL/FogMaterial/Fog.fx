//-----------------------------------------------------------------------------
// Fog.fx
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

cbuffer Parameters : register(b1)
{
	float3 FogColor				: packoffset(c0.x);
	float FogDensity			: packoffset(c0.w);
	float ZParamA				: packoffset(c1.x);
	float ZParamB				: packoffset(c1.y);
	float FogStart				: packoffset(c1.z);
	float FogEnd				: packoffset(c1.w);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

Texture2D DepthTexture : register(t1);

sampler DepthTextureSampler =
sampler_state
{
	Texture = <DepthTexture>;
	MipFilter = POINT;
	MinFilter = POINT;
	MagFilter = POINT;
};

struct PS_IN_TEXTURE
{
	float4 Position 	: SV_POSITION;
	float2 TexCoord 	: TEXCOORD0;
};

inline float LinearDepth(float z)
{
	return 1.0 / (ZParamA * z + ZParamB);
}

float4 psFog(PS_IN_TEXTURE input) : SV_Target0
{
	float depth = DepthTexture.Sample(DepthTextureSampler, input.TexCoord).x;
	float3 scene = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;

	// LinearDepth, range 0 - 1
	float distance = LinearDepth(depth);

	// Calculate exponential fog amount
	float fogAmount = (FogEnd - distance) / (FogEnd - FogStart);
	fogAmount = saturate(fogAmount);

	// Compute resultant pixel
	float3 color = lerp(FogColor, scene, fogAmount);

	return float4(color, 1.0);
}

float4 psFogExp(PS_IN_TEXTURE input) : SV_Target0
{
	float depth = DepthTexture.Sample(DepthTextureSampler, input.TexCoord).x;
	float3 scene = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;

	// LinearDepth, range 0 - 1
	float distance = LinearDepth(depth);

	// Calculate exponential fog amount
	float fogAmount = exp2(-distance * FogDensity);

	// Compute resultant pixel
	float3 color = lerp(FogColor, scene, fogAmount);

	return float4(color, 1.0);
}

float4 psFogExp2(PS_IN_TEXTURE input) : SV_Target0
{
	float depth = DepthTexture.Sample(DepthTextureSampler, input.TexCoord).x;
	float3 scene = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;

	// LinearDepth, range 0 - 1
	float distance = LinearDepth(depth);

	// Calculate exponential fog amount
	float fogAmount = exp2(- pow(distance * FogDensity, 2));

	// Compute resultant pixel
	float3 color = lerp(FogColor, scene, fogAmount);

	return float4(color, 1.0);
}
