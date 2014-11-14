//-----------------------------------------------------------------------------
// ScreenOverlay.fx
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	float Intensity			: packoffset(c0.x);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

Texture2D OverlayTexture : register(t1);
SamplerState OverlayTextureSampler : register(s1);

float4 psScreenOverlayAdditive(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;
	float3 overlay = OverlayTexture.Sample(OverlayTextureSampler, input.TexCoord).rgb;

	color += overlay;

	return float4(color.xyz * Intensity, 1.0);
}

float4 psScreenOverlayMultiply(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;
	float3 overlay = OverlayTexture.Sample(OverlayTextureSampler, input.TexCoord).rgb;

	color *= overlay;

	return float4(color.xyz * Intensity, 1.0);
}

float4 psScreenOverlayScreenBlend(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;
	float3 overlay = OverlayTexture.Sample(OverlayTextureSampler, input.TexCoord).rgb;

	color = 1.0 - ((1.0 - color) * (1.0 - overlay));

	return float4(color.xyz * Intensity, 1.0);
}

float Luminance(float3 LinearColor)
{
	return dot(LinearColor, float3(0.3, 0.59, 0.11));
}

float4 psScreenOverlayOverlay(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;
	float3 overlay = OverlayTexture.Sample(OverlayTextureSampler, input.TexCoord).rgb;

	if (Luminance(overlay) < 0.5)
	{
		color = 2 * color * overlay;
	}
	else
	{
		color = 1.0 - ((1.0 - color) * (1.0 - overlay));
	}

	return float4(color.xyz * Intensity, 1.0);
}

float4 psScreenOverlayAlphaBlend(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;
	float4 overlay = OverlayTexture.Sample(OverlayTextureSampler, input.TexCoord).rgba;

	color.rgb = (overlay.a * color.rgb) + ((1.0 - overlay.a) * overlay.rgb);

	return float4(color.xyz * Intensity, 1.0);
}