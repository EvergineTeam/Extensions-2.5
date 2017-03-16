//-----------------------------------------------------------------------------
// Bokeh.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#define SAMPLES 6

#if HIGH
#define SAMPLES 10
#elif MEDIUM
#define SAMPLES 8
#endif

float bleedingBias = 0.02f;

cbuffer Parameters : register(b1)
{
	float2 BlurDisp				: packoffset(c0.x);
	float Aperture				: packoffset(c0.z);
	float LastCoeff				: packoffset(c0.w);
	float FocalDistance			: packoffset(c1.x);
	float NearPlane				: packoffset(c1.y);
	float FarParam				: packoffset(c1.z);
	float FilmWidth				: packoffset(c1.w);
	float ShineThreshold		: packoffset(c2.x);
	float ShineAmount			: packoffset(c2.y);
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

// CoC Map
// http://ivizlab.sfu.ca/papers/cgf2012.pdf
float4 psCoCMap(PS_IN_TEXTURE input) : SV_Target0
{
	// Reconstruct scene depth at this pixel
	const half depth = DepthTexture.Sample(DepthTextureSampler, input.TexCoord).x;
	const half S2 = (-NearPlane * FarParam) / (depth - FarParam);

	// Calculate circle of confusion diameter
	// https://en.wikipedia.org/wiki/Circle_of_confusion
	const half coc = abs(Aperture * ((S2 - FocalDistance) / S2) * LastCoeff); // (f / (S1 - f)));

	// put CoC into a % of the image sensor height
	const half blurFactor = coc / FilmWidth;

	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;		

	return float4(color, blurFactor);
}

inline float3 Blur(float4 c0, half2 uv, half2 step)
{
	// Accumulation
	float3 acc = c0.xyz;

	// Total weight
	half totalweight = 1;	

	[unroll(SAMPLES)]
	for (int i = 1; i < SAMPLES; i++)
	{
		[unroll]
		for (int j = -1.0; j <= 1.0; j += 2.0)
		{
			float4 c1 = DiffuseTexture.Sample(DiffuseTextureSampler, uv + j * i * step);			

			half w = c0.a > c1.a + bleedingBias ? 0.0 : 1.0;

			acc += c1.xyz * w;
			totalweight += w;
		}
	}

	float3 color = acc / totalweight;	

	return color;
}

float4 psHorizontalBlur(PS_IN_TEXTURE input) : SV_Target0
{
	// Get Color and Coc value
	float4 c0 = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord);

	// Calculate the step
	half2 step = (BlurDisp * c0.a) / float(SAMPLES);

	// Horizontal Blur
	float3 color = Blur(c0, input.TexCoord, step);

	return float4(color, c0.a);
}

// Combine 
float4 psDiagonalBlurCombine(PS_IN_TEXTURE input) : SV_Target0
{
	// Get Color and Coc value
	float4 c0 = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord);		

	// Calculate the step1
	half2 step = (BlurDisp * c0.a) / float(SAMPLES);

	// First diagonal
	float3 color1 = Blur(c0, input.TexCoord, step);

	// Calculate the step1
	step.x *= -1;

	// Second diagonal
	float3 color2 = Blur(c0, input.TexCoord, step);

	// Bleending color
	float3 sumCol = min(color1, color2);	

	half luminance = dot(sumCol, float3(0.3, 0.59, 0.11));
	half amount = saturate((luminance - ShineThreshold) * ShineAmount);	

	return float4(sumCol + amount * c0.a, 1.0);
}