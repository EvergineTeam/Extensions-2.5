//-----------------------------------------------------------------------------
// LensFlare.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#define numGhosts 8

cbuffer Matrices : register(b0)
{
	float4x4	WorldViewProj				: packoffset(c0);
};

cbuffer Parameters : register(b1)
{
	float3 Bias				: packoffset(c0.x);
	float HaloWidth			: packoffset(c0.w);
	float3 Scale			: packoffset(c1.x);
	float GhostDispersal	: packoffset(c1.w);
	float Distortion		: packoffset(c2.x);
	float2 TexcoordOffset	: packoffset(c2.y);
	float Intensity			: packoffset(c2.w);
	float2 StarRotation		: packoffset(c3.x);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

Texture2D LensColorTexture : register(t1);
SamplerState LensColorTextureSampler : register(s1);

Texture2D LensDirtTexture : register(t2);
SamplerState LensDirtTextureSampler : register(s2);

Texture2D LensStarTexture : register(t3);
SamplerState LensStarTextureSampler : register(s3);

Texture2D LensFlareTexture : register(t4);
SamplerState LensFlareTextureSampler : register(s4);

// Structs
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
	float4 Position 		: SV_POSITION;
	float2 TexCoord 		: TEXCOORD0;
};

struct PS_IN_TEXTUREWEIGTH
{
	float4 OutUV[8]			: TEXCOORD0;
};


// Helper functions
float3 TextureDistorted(
	in float2 texcoord,
	in float2 direction,
	in float3 distortion
	) 
{
	return float3(DiffuseTexture.Sample(DiffuseTextureSampler, texcoord + direction * distortion.r).r,
				  DiffuseTexture.Sample(DiffuseTextureSampler, texcoord + direction * distortion.g).g,
				  DiffuseTexture.Sample(DiffuseTextureSampler, texcoord + direction * distortion.b).b);
}

float2 Circle(float Start, float Points, float Point)
{
	float Rad = (3.141592 * 2.0 * (1.0 / Points)) * (Point + Start);
	return float2(sin(Rad), cos(Rad));
}


// Downsampler
float4 psLensFlareDown(PS_IN_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb + Bias;
	color = max(float3(0, 0, 0), color) * Scale;

	return float4(color, 1.0);
}


// LensFlare
float4 psLensFlare(PS_IN_TEXTURE input) : SV_Target0
{
	float2 texcoord = -input.TexCoord + 1.0;

	float2 ghostVec = (0.5 - texcoord) * GhostDispersal;
	float2 haloVec = normalize(ghostVec) * HaloWidth;

	float3 distortion = float3(-TexcoordOffset.x * Distortion, 0.0, TexcoordOffset.x * Distortion);

	// Sample ghosts:  
	float3 color = 0;
	float2 center = float2(0.5, 0.5);
	
	for (int i = 0; i < numGhosts; ++i)
	{
		float2 offset = frac(texcoord + ghostVec * float(i));

		float weight = length(center - offset) / length(center);
		weight = pow(1.0 - weight, 10.0);

		color += TextureDistorted(offset, normalize(ghostVec), distortion) * weight;
	}

	// Apply color
	color *= LensColorTexture.Sample(LensColorTextureSampler, length(center - texcoord) / length(center)).rgb;

	// Sample halo:
	float weight = length(center - frac(texcoord + haloVec)) / length(center);
	weight = pow(1.0 - weight, 5.0);
	color += TextureDistorted(frac(texcoord + haloVec), normalize(ghostVec), distortion) * weight;

	return float4(color, 1.0);
}

// Blur
VS_OUT_TEXTURE vsLensFlareBlur(VS_IN_TEXTURE input)
{
	VS_OUT_TEXTURE output = (VS_OUT_TEXTURE)0;

	output.Position = mul(input.Position, WorldViewProj);

	float Start = 2.0 / 14.0;
	float2 Scale = 7.0 * TexcoordOffset.xy;

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

float4 psLensFlareBlur(PS_IN_TEXTUREWEIGTH input) : SV_Target0
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

// CombineUp
float4 psLensFlareCombine(PS_IN_TEXTURE input) : SV_Target0
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord).rgb;

	float2 center = float2(0.5, 0.5);
	float2 uv = input.TexCoord - center;
	float2 texcoord = float2(uv.x * StarRotation.x - uv.y * StarRotation.y,
							 uv.x * StarRotation.y + uv.y * StarRotation.x);
	texcoord += center;

	float3 lensMod = LensDirtTexture.Sample(LensDirtTextureSampler, input.TexCoord).rgb;
	lensMod += LensStarTexture.Sample(LensStarTextureSampler, texcoord).rgb;

	float3 lensFlare = LensFlareTexture.Sample(LensFlareTextureSampler, input.TexCoord).rgb;
	color += lensFlare * lensMod * Intensity;

	return float4(color, 1.0);
}
