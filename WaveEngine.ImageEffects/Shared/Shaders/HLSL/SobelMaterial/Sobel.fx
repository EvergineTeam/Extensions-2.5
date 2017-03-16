//-----------------------------------------------------------------------------
// Sobel.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

cbuffer Parameters : register(b1)
{
	float Threshold		: packoffset(c0.x);
	float2 TexcoordOffset : packoffset (c0.y);
};

float Color(float2 texcoord)
{
	float3 color = DiffuseTexture.Sample(DiffuseTextureSampler, texcoord).xyz;
	
	// Luma Operator
	return dot(color, float3(0.2126, 0.7152, 0.0722));
}

float4 psSobel(VS_OUT_TEXTURE input) : SV_Target0
{
	// Sample the neighbor pixels
	float s00 = Color(input.TexCoord + (float2(-1, -1) * TexcoordOffset));
	float s01 = Color(input.TexCoord + (float2(0, -1) * TexcoordOffset));
	float s02 = Color(input.TexCoord + (float2(1, -1) * TexcoordOffset));
	float s10 = Color(input.TexCoord + (float2(-1, 0) * TexcoordOffset));
	float s12 = Color(input.TexCoord + (float2(1, 0) * TexcoordOffset));
	float s20 = Color(input.TexCoord + (float2(-1, 1) * TexcoordOffset));
	float s21 = Color(input.TexCoord + (float2(0, 1) * TexcoordOffset));
	float s22 = Color(input.TexCoord + (float2(1, 1) * TexcoordOffset));

	// Sobel filter in X and Y directions
	float sobelX = s00 + 2 * s10 + s20 - s02 - 2 * s12 - s22;
	float sobelY = s00 + 2 * s01 + s02 - s20 - 2 * s21 - s22;

	// Find edge using a threshold to detect most edges.
	return sqrt(sobelX * sobelX + sobelY * sobelY);
}

float4 psSobelEdge(VS_OUT_TEXTURE input) : SV_Target0
{
	// Sample the neighbor pixels
	float s00 = Color(input.TexCoord + (float2(-1, -1) * TexcoordOffset));
	float s01 = Color(input.TexCoord + (float2(0, -1) * TexcoordOffset));
	float s02 = Color(input.TexCoord + (float2(1, -1) * TexcoordOffset));
	float s10 = Color(input.TexCoord + (float2(-1, 0) * TexcoordOffset));
	float s12 = Color(input.TexCoord + (float2(1, 0) * TexcoordOffset));
	float s20 = Color(input.TexCoord + (float2(-1, 1) * TexcoordOffset));
	float s21 = Color(input.TexCoord + (float2(0, 1) * TexcoordOffset));
	float s22 = Color(input.TexCoord + (float2(1, 1) * TexcoordOffset));

	// Sobel filter in X and Y directions
	float sobelX = s00 + 2 * s10 + s20 - s02 - 2 * s12 - s22;
	float sobelY = s00 + 2 * s01 + s02 - s20 - 2 * s21 - s22;

	// Find edge using a threshold to detect most edges.
	float edgeSqr = (sobelX * sobelX + sobelY * sobelY);
	
	if(edgeSqr > Threshold)
	{
		return float4(float3(0,0,0), 1.0);
	}
	else
	{
		return float4(float3(1,1,1), 1.0);
	}
}

float4 psSobelEdgeColor(VS_OUT_TEXTURE input) : SV_Target0
{
	float3 diffuseColor = DiffuseTexture.Sample(DiffuseTextureSampler, input.TexCoord ).xyz;

	// Sample the neighbor pixels
	float s00 = Color(input.TexCoord + (float2(-1, -1) * TexcoordOffset));
	float s01 = Color(input.TexCoord + (float2(0, -1) * TexcoordOffset));
	float s02 = Color(input.TexCoord + (float2(1, -1) * TexcoordOffset));
	float s10 = Color(input.TexCoord + (float2(-1, 0) * TexcoordOffset));
	float s12 = Color(input.TexCoord + (float2(1, 0) * TexcoordOffset));
	float s20 = Color(input.TexCoord + (float2(-1, 1) * TexcoordOffset));
	float s21 = Color(input.TexCoord + (float2(0, 1) * TexcoordOffset));
	float s22 = Color(input.TexCoord + (float2(1, 1) * TexcoordOffset));

	// Sobel filter in X and Y directions
	float sobelX = s00 + 2 * s10 + s20 - s02 - 2 * s12 - s22;
	float sobelY = s00 + 2 * s01 + s02 - s20 - 2 * s21 - s22;

	// Find edge using a threshold to detect most edges.
	float edgeSqr = (sobelX * sobelX + sobelY * sobelY);
	
	if (edgeSqr > Threshold)
	{
		return float4(float3(0,0,0), 1.0);
	}
	else
	{
		return float4(diffuseColor, 1.0);
	}
}
