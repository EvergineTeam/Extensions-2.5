//-----------------------------------------------------------------------------
// LightShaft.fx
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#define SAMPLES 32

#if HIGH
#define SAMPLES 96
#elif MEDIUM
#define SAMPLES 64
#endif

cbuffer Matrices : register(b0)
{
	float4x4	WorldViewProj				: packoffset(c0);
};

cbuffer Parameters : register(b1)
{
	float2 LightCenter			: packoffset(c0.x);
	float2 TexcoordOffset		: packoffset(c0.z);
	float Density				: packoffset(c1.x);
	float Weight				: packoffset(c1.y);
	float Decay					: packoffset(c1.z);
	float Exposure				: packoffset(c1.w);
	float Blend					: packoffset(c2.x);
	float3 ShaftTint			: packoffset(c2.y);
	float Radius				: packoffset(c3.x);
	float EdgeSharpness			: packoffset(c3.y);
	float SunIntensity			: packoffset(c3.z);
	float DepthThreshold		: packoffset(c3.w);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

Texture2D DiffuseTexture1 : register(t1);
SamplerState DiffuseTextureSampler1 : register(s1);

Texture2D DepthTexture : register(t2);

sampler DepthTextureSampler =
sampler_state
{
	Texture = <DepthTexture>;
	MipFilter = POINT;
	MinFilter = POINT;
	MagFilter = POINT;
};

struct VS_IN_TEXTURE
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

struct PS_IN_TEXTURE
{
	float4 Position 	: SV_POSITION;
	float2 TexCoord 	: TEXCOORD0;
};

// Down sampler
float4 psBlackMask(PS_IN_TEXTURE input) : SV_Target0
{
	half depth = DepthTexture.Sample(DepthTextureSampler, input.TexCoord).x;
	depth = (depth - DepthThreshold);
	depth = max(depth, 0) * SunIntensity;

	float light = depth;

#if HALO
	float dis = distance(LightCenter, input.TexCoord);
	float sun = 1.0 - pow(dis * (1.0 / Radius), EdgeSharpness);

	light *= sun;
#endif

	// Linear Filtering
	half3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;
	color += DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord + float2(0, TexcoordOffset.y)).rgb;
	color += DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord + float2(TexcoordOffset.x, 0)).rgb;
	color += DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord + TexcoordOffset).rgb;

	color.rgb * 0.25;

	return float4(color.rgb * light, light);
}

float4 psLightShaft(PS_IN_TEXTURE input) : SV_Target0
{
	half2 texCoord = input.TexCoord;

	// Calculate vector from pixel to light source in screen space.  
	half2 deltaTexCoord = (texCoord - LightCenter);

	// Divide by number of samples and scale by control factor.  
	deltaTexCoord *= 1.0 / float(SAMPLES) * Density;

	// Store initial sample.  
	half4 color = DiffuseTexture.Sample(DiffuseTextureSampler, texCoord);

	// Set up illumination decay factor.  
	half illuminationDecay = 1.0f;

	[unroll(SAMPLES)]
	for (int i = 0; i < SAMPLES; i++)
	{
		// Step sample location along ray.  
		texCoord -= deltaTexCoord;

		// Retrieve sample at new location.  
		half4 colorSample = DiffuseTexture.Sample(DiffuseTextureSampler, texCoord).rgba;

		// Apply sample attenuation scale/decay factors.  
		colorSample *= illuminationDecay * Weight;

		// Accumulate combined color.   
		color.rgba += colorSample;

		// Update exponential decay factor.  
		illuminationDecay *= Decay;
	}

	// Output final color with a further scale control factor.  
	return color * Exposure;
}

// Up sampler and combine
float4 psLSCombine(PS_IN_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;
	float4 lightShaft = DiffuseTexture1.Sample(DiffuseTextureSampler1, input.TexCoord);
	
	lightShaft.rgb *= ShaftTint;
	
	float faceShaft = lightShaft.a * Blend;
	lightShaft.rgb *= faceShaft;

	return float4(color.rgb + lightShaft.rgb, 1.0);
}
