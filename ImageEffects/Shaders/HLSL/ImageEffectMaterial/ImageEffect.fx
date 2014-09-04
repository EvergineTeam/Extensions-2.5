//-----------------------------------------------------------------------------
// ImageEffect.fx
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#include "../Structures.fxh"

cbuffer Matrices : register(b0)
{
    float4x4	WorldViewProj				: packoffset(c0);
};

// Vertex Shader
VS_OUT_TEXTURE vsImageEffect(VS_IN_TEXTURE input)
{
	VS_OUT_TEXTURE output = (VS_OUT_TEXTURE)0;

    output.Position = mul(input.Position, WorldViewProj);
	output.TexCoord = input.TexCoord;

    return output;
}
