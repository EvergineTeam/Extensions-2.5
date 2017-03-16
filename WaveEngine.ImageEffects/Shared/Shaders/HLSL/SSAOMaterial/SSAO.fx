//-----------------------------------------------------------------------------
// SSAO.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#include "../Structures.fxh"

cbuffer Matrices : register(b0)
{
	float4x4	WorldViewProj				: packoffset(c0);
};

cbuffer Parameters : register(b1)
{
	float DistanceThreshold					: packoffset(c0.x);
	float AOintensity						: packoffset(c0.y);
	float2 FilterRadius						: packoffset(c0.z);
	float4x4 ViewProjectionInverse			: packoffset(c1);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

Texture2D AOTexture : register(t1);
SamplerState AOTextureSampler : register(s1);

Texture2D Gbuffer : register(t2);

sampler GbufferSampler =
sampler_state
{
	Texture = <GBuffer>;
	MipFilter = POINT;
	MinFilter = POINT;
	MagFilter = POINT;
};

Texture2D DepthTexture : register(t3);

sampler DepthTextureSampler =
sampler_state
{
	Texture = <DepthTexture>;
	MipFilter = POINT;
	MinFilter = POINT;
	MagFilter = POINT;
};

static int sample_count = 16;
static float2 poisson16[16] = {  // These are the Poisson Disk Samples
	float2(-0.94201624, -0.39906216),
	float2(0.94558609, -0.76890725),
	float2(-0.094184101, -0.92938870),
	float2(0.34495938, 0.29387760),
	float2(-0.91588581, 0.45771432),
	float2(-0.81544232, -0.87912464),
	float2(-0.38277543, 0.27676845),
	float2(0.97484398, 0.75648379),
	float2(0.44323325, -0.97511554),
	float2(0.53742981, -0.47373420),
	float2(-0.26496911, -0.41893023),
	float2(0.79197514, 0.19090188),
	float2(-0.24188840, 0.99706507),
	float2(-0.81409955, 0.91437590),
	float2(0.19984126, 0.78641367),
	float2(0.14383161, -0.14100790)
	};

inline float3 CalculatePosition(in float2 texCoord, in float depth)
{
	// H is the viewport position at this pixel in the range -1 to 1.
	float4 H = float4(texCoord.x * 2.0 - 1.0, (1.0 - texCoord.y) * 2.0 - 1.0, depth, 1.0);

	// Transform by the view-projection inverse.
	float4 D = mul(H, ViewProjectionInverse);
	return D.xyz / D.w;
}

VS_OUT_TEXTURE vsSSAO(VS_IN_TEXTURE input)
{
	VS_OUT_TEXTURE output = (VS_OUT_TEXTURE)0;

	output.Position = mul(input.Position, WorldViewProj);
	output.TexCoord = input.TexCoord;

	return output;
}

float4 psSSAO(VS_OUT_TEXTURE input) : SV_Target0
{
	float2 texCoord = input.TexCoord;
	float4 gbuffer = Gbuffer.Sample(GbufferSampler, texCoord);
	float depth = DepthTexture.Sample(DepthTextureSampler, texCoord).x;

	float3 normal = gbuffer.xyz * 2.0 - 1.0;
	float3 position = CalculatePosition(texCoord, depth);

	float ambientOcclusion = 0;

	// perform AO
	for (int i = 0; i < sample_count; ++i)
	{
		// sample at an offset specified by the current Poisson-Disk sample and scale it by a radius (has to be in Texture-Space)
		float2 sampleTexCoord = texCoord + (poisson16[i] * (FilterRadius));
		float depth = DepthTexture.Sample(DepthTextureSampler, sampleTexCoord).x;
		float3 samplePos = CalculatePosition(sampleTexCoord, depth);
		float3 sampleDir = normalize(samplePos - position);

		// angle between SURFACE-NORMAL and SAMPLE-DIRECTION (vector from SURFACE-POSITION to SAMPLE-POSITION)
		float NdotS = dot(normal, sampleDir);

		// distance between SURFACE-POSITION and SAMPLE-POSITION
		float VPdistSP = distance(position, samplePos);

		// a = distance function
		float a = 1.0 - smoothstep(DistanceThreshold, DistanceThreshold * 2, VPdistSP);
		// b = dot-Product
		float b = max(NdotS, 0.15);

		ambientOcclusion += (a * b);
	}

	float ao = 1.0 - (ambientOcclusion / sample_count);

	return ao.xxxx;
}

float4 psSSAOCombine(VS_OUT_TEXTURE input) : SV_Target0
{
	float4 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord);

	float ao = AOTexture.Sample(AOTextureSampler, input.TexCoord).x;
	ao = pow(ao, AOintensity);

	color.xyz *= ao;

	return color;
}

float4 psOnlyAO(VS_OUT_TEXTURE input) : SV_Target0
{
	float ao = AOTexture.Sample(AOTextureSampler, input.TexCoord).x;
	ao = pow(ao, AOintensity);

	return float4(ao.xxx, 1);
}