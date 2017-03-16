//-----------------------------------------------------------------------------
// Tiling.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Parameters : register(b1)
{
	float3 EdgeColor		: packoffset(c0.x);
	float NumTiles			: packoffset(c0.w);
	float Threshhold		: packoffset(c1.x);
};

Texture2D DiffuseTexture : register(t0);
SamplerState DiffuseTextureSampler : register(s0);

float4 psTiling(VS_OUT_TEXTURE input) : SV_Target0
{
	half size = 1.0 / NumTiles;

	half2 Pbase = input.TexCoord - fmod(input.TexCoord, size.xx);
	half2 PCenter = Pbase + (size / 2.0).xx;
	half2 st = (input.TexCoord - Pbase) / size;
	half4 c1 = (half4)0;
	half4 c2 = (half4)0;
	half4 invOff = half4((1 - EdgeColor), 1);

	if (st.x > st.y)
	{
		c1 = invOff; 
	}

	half threshholdB = 1.0 - Threshhold;

	if (st.x > threshholdB) 
	{ 
		c2 = c1; 
	}

	if (st.y > threshholdB) 
	{ 
		c2 = c1; 
	}

	half4 cBottom = c2;
	c1 = (half4)0;
	c2 = (half4)0;
	
	if (st.x > st.y)
	{ 
		c1 = invOff; 
	}

	if (st.x < Threshhold) 
	{ 
		c2 = c1;
	}
	
	if (st.y < Threshhold) 
	{ 
		c2 = c1; 
	}

	half4 cTop = c2;
	half4 tileColor = DiffuseTexture.Sample(DiffuseTextureSampler, PCenter);
	half4 result = tileColor + cTop - cBottom;

	return result;
}
