//-----------------------------------------------------------------------------
// Structures.fxh
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

struct DirectionalLight
{
	float3		Direction;
    float3		Color;
};

struct PointLight
{
	float3		Position;
	float		Attenuation;
    float3		Color;
	float		Falloff;
};

struct Material
{
	float3		Diffuse;
	float3 		Specular;
};

// VERTEX SHADER INPUT
struct VS_IN
{
	float4 Position : POSITION;
};

struct VS_IN_NORMAL
{
	float4 Position	: POSITION;
	float3 Normal	: NORMAL0;
};

struct VS_IN_NORMALTEXTURE
{
	float4 Position	: POSITION;
	float3 Normal	: NORMAL0;
	float2 TexCoord : TEXCOORD0;
};

struct VS_IN_COLOR
{
	float4 Position : POSITION;
	float4 Color	: COLOR;
};

struct VS_IN_NORMALCOLOR
{
	float4 Position : POSITION;
	float3 Normal	: NORMAL0;
	float4 Color	: COLOR;
};

struct VS_IN_TEXTURE
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

struct VS_IN_COLORTEXTURE
{
	float4 Position : POSITION;
	float4 Color	: COLOR;
	float2 TexCoord : TEXCOORD0;
};

struct VS_IN_NORMALCOLORTEXTURE
{
	float4 Position : POSITION;
	float3 Normal	: NORMAL;
	float4 Color	: COLOR;
	float2 TexCoord : TEXCOORD0;
};

struct VS_IN_DUALTEXTURE
{
	float4 Position		: POSITION;
	float2 TexCoord1	: TEXCOORD0;
	float2 TexCoord2	: TEXCOORD1;
};

struct VS_IN_DUALTEXTURECOLOR
{
	float4 Position		: POSITION;
	float4 Color		: COLOR0;
	float2 TexCoord1	: TEXCOORD0;
	float2 TexCoord2	: TEXCOORD1;
};

struct VS_IN_TBNTEXTURE
{
	float4 Position		: POSITION;
	float3 Normal		: NORMAL0;
	float3 Binormal		: BINORMAL0;
	float3 Tangent		: TANGENT0;
	float2 TexCoord		: TEXCOORD0;
};

struct VS_IN_TBN2TEXTURE
{
	float4 Position		: POSITION;
	float3 Normal		: NORMAL0;
	float3 Binormal		: BINORMAL0;
	float3 Tangent		: TANGENT0;
	float2 TexCoord		: TEXCOORD0;
	float2 TexCoord2	: TEXCOORD1;
};

// VERTEX SHADER OUTPUT
struct VS_OUT
{
	float4 Position : SV_POSITION;
};

struct VS_OUT_COLOR
{
	float4 Position : SV_POSITION;
	float4 Color 	: COLOR0;
};

struct VS_OUT_PWSNWS
{
	float4 Position 	: SV_POSITION;
	float3 PositionWS	: TEXCOORD0;
	float3 NormalWS		: TEXCOORD1;
};

struct VS_OUT_PWSNWSCOLOR
{
	float4 Position 	: SV_POSITION;
	float3 PositionWS	: TEXCOORD0;
	float3 NormalWS		: TEXCOORD1;
	float4 Color 		: COLOR0;
};

struct VS_OUT_PWSNWSTEXTURE
{
	float4 Position 	: SV_POSITION;
	float3 PositionWS	: TEXCOORD0;
	float3 NormalWS		: TEXCOORD1;
	float2 TexCoord		: TEXCOORD2;
};

struct VS_OUT_TEXTURE
{
	float4 Position 	: SV_POSITION;
	float2 TexCoord 	: TEXCOORD0;
};

struct VS_OUT_COLORTEXTURE
{
	float4 Position 	: SV_POSITION;
	float4 Color		: COLOR;
	float2 TexCoord 	: TEXCOORD0;
};

struct VS_OUT_ENVIRONMENT
{
	float4 Position 	: SV_POSITION;
	float3 EnvCoord		: TEXCOORD0;
};

struct VS_OUT_TEXTUREENVIRONMENT
{
	float4 Position 	: SV_POSITION;
	float2 TexCoord 	: TEXCOORD0;
	float3 EnvCoord		: TEXCOORD1;
	float3 Specular		: COLOR0;
};

struct VS_OUT_PWSNWSTEXTUREENVIRONMENT
{
	float4 Position 	: SV_POSITION;
	float3 PositionWS	: TEXCOORD0;
	float3 NormalWS		: TEXCOORD1;
	float2 TexCoord 	: TEXCOORD2;
	float3 EnvCoord		: TEXCOORD3;
	float3 EyeVector	: TEXCOORD4;
	float3 Specular		: COLOR0;
};
struct VS_OUT_PWSNWSCOLORTEXTURE
{
	float4 Position 	: SV_POSITION;
	float3 PositionWS	: TEXCOORD1;
	float3 NormalWS		: TEXCOORD2;
	float4 Color		: COLOR0;
	float2 TexCoord 	: TEXCOORD0;
};

struct VS_OUT_DUALTEXTURECOLOR
{
	float4 Position		: SV_POSITION;
	float4 Color		: COLOR0;
	float2 TexCoord1	: TEXCOORD0;
	float2 TexCoord2	: TEXCOORD1;
};

struct VS_OUT_VIEWLIGHTATTTEXTURE
{
	float4 Position		: SV_POSITION;
	float2 TexCoord		: TEXCOORD0;
	float3 PositionWS	: TEXCOORD1;
	float3 ViewTS		: TEXCOORD2;
	float3 LightTS		: TEXCOORD3;
	float3 Att			: TEXCOORD4;
};

struct VS_OUT_VIEWLIGHTATT2TEXTURE
{
	float4 Position		: SV_POSITION;
	float2 TexCoord		: TEXCOORD0;
	float2 TexCoord2	: TEXCOORD1;
	float3 PositionWS	: TEXCOORD2;
	float3 ViewTS		: TEXCOORD3;
	float3 LightTS		: TEXCOORD4;
	float3 Att			: TEXCOORD5;
};