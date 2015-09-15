//-----------------------------------------------------------------------------
// Convolution.fx
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	float2 TexcoordOffset : packoffset(c0.x);
	float Scale : packoffset(c0.z);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

float4 Color(float2 texcoord) : COLOR0
{
	return DiffuseTexture.Sample(DiffuseTextureSampler, texcoord);
}

float4 psLaplace(VS_OUT_TEXTURE input) : SV_Target0
{
	//  0  1  0
	//  1 -4  1
	//  0  1  0
	float2 samples[4] =
	{
		 0, -1,
		-1, -0,
		 1,  0,
		 0,  1,
	};

	float4 laplace = -4 * Color(input.TexCoord);

	for (int i = 0; i < 4; i++)
	{
		laplace += Color(input.TexCoord + (samples[i] * TexcoordOffset));
	}

	return Scale * laplace;
}

float4 psLaplaceGreyScale(VS_OUT_TEXTURE input) : SV_Target0
{
	//  0  1  0
	//  1 -4  1
	//  0  1  0
	float2 samples[4] =
	{
		 0, -1,
		-1, -0,
		 1,  0,
		 0,  1,
	};

	float4 laplace = -4 * Color(input.TexCoord);

		for (int i = 0; i < 4; i++)
		{
		laplace += Color(input.TexCoord + (samples[i] * TexcoordOffset));
		}

	return 0.5 + Scale * laplace;
}

float4 psSharpen(VS_OUT_TEXTURE input) : SV_Target0
{
	//  0 -1  0
	// -1  5 -1
	//  0 -1  0
	float2 samples[4] =
	{
		 0, -1,
		-1, -0,
		 1,  0,
		 0,  1,
	};

	float4 sharpen = 5 * Color(input.TexCoord);

	for (int i = 0; i < 4; i++)
	{
		sharpen -= Color(input.TexCoord + (samples[i] * TexcoordOffset));
	}

	return sharpen;
}

float4 psBlur3x3(VS_OUT_TEXTURE input) : SV_Target0
{
	//  1  1  1
	//  1  1  1
	//  1  1  1
	float2 samples[9] =
	{
		-1, -1,
		 0, -1,
		 1, -1,
		-1,  0,
		 0,  0,
		 1,  0,
		-1,  1,
		 0,  1,
		 1,  1,
	};

	float4 blur;
	blur.xyzw = 0;

	for (int i = 0; i < 9; i++)
	{
		blur += Color(input.TexCoord + (samples[i] * TexcoordOffset));
	}
	
	blur.rgb /= 9.0;
	return blur;
}

float4 psBlur5x5(VS_OUT_TEXTURE input) : SV_Target0
{
	//  1  1  1  1  1
	//  1  1  1  1  1
	//  1  1  1  1  1
	//  1  1  1  1  1
	//  1  1  1  1  1

	float2 samples[25] =
	{
		-2, -2,
		-1, -2,
		 0, -2,
		 1, -2,
		 2, -2,
		-2, -1,
		-1, -1,
		 0, -1,
		 1, -1,
		 2, -1,
		-2,  0,
		-1,  0,
		 0,  0,
		 1,  0,
		 2,  0,
		-2,  1,
		-1,  1,
		 0,  1,
		 1,  1,
		 2,  1,
		-2,  2,
		-1,  2,
		 0,  2,
		 1,  2,
		 2,  2,
	};

	float4 blur;
	blur.xyzw = 0;

	for (int i = 0; i < 25; i++)
	{
		blur += Color(input.TexCoord + (samples[i] * TexcoordOffset));
	}
	
	blur.rgb /= 25.0;
	return blur;
}

float4 psEmboss(VS_OUT_TEXTURE input) : SV_Target0
{
	float4 emboss;
	emboss.a = 1.0;
	emboss.rgb = 0.5;

	emboss -= 2 * Color(input.TexCoord + (TexcoordOffset * float2(-1, -1)));
	emboss += 2 * Color(input.TexCoord + (TexcoordOffset * float2(1, 1)));
	emboss.rgb = (emboss.r + emboss.g + emboss.b) / 3.0;

	return emboss;
}
