//-----------------------------------------------------------------------------
// MotionBlur.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

#if HIGH
#define numSamples 9
#elif MEDIUM
#define numSamples 6
#elif LOW
#define numSamples 4
#endif

cbuffer Parameters : register(b1)
{
	float BlurLength						: packoffset(c0.x);
	float4x4 ViewProjectionInverse			: packoffset(c1);
	float4x4 PreviousViewProjection			: packoffset(c5);
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


float4 psMotionBlur(VS_OUT_TEXTURE input) : SV_Target0
{
	float2 texCoord = input.TexCoord;
	float depth = DepthTexture.Sample(DepthTextureSampler, texCoord).x;

	// H is the viewport position at this pixel in the range -1 to 1.
	float4 H = float4(texCoord.x * 2.0 - 1.0, (1.0 - texCoord.y) * 2.0 - 1.0, depth, 1.0);

	// Transform by the view-projection inverse.
	float4 D = mul(H, ViewProjectionInverse);
	float4 worldPos = D / D.w;

	// Current viewport position
	float4 currentPos = H;

	// Use the world position and transform by the previous view-projection matrix.
	float4 previousPos = mul(worldPos, PreviousViewProjection);

	// Convert to nonhomegeneous points [-1, 1] by dividing by w.
	previousPos.xy /= previousPos.w;

	// Use this frame's position and last frame's to compute the pixel velocity
	float2 velocity = (currentPos.xy - previousPos.xy) * BlurLength;
	velocity.y = -velocity.y;
	
	velocity /= numSamples;

	// Get the initial color at this pixel
	float4 color = DiffuseTexture.Sample(DiffuseTextureSampler, texCoord);
	texCoord += velocity;
	
	// Accumulate in the color.
	for (int i = 1; i < numSamples; ++i, texCoord += velocity)
	{
		color += DiffuseTexture.Sample(DiffuseTextureSampler, texCoord);
	}

	// Average all of the samples to get the final blur color
	return color / numSamples;
}
