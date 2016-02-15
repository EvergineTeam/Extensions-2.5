//-----------------------------------------------------------------------------
// Helpers.fxh
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

////////////////////////////////
// Light structures
////////////////////////////////

struct LightStruct
{
	float3		Position;
	float		ConeAngle;
    float3		Color;
	float		LightRange;
	float3		Direction;
	float		Intensity;
};

struct Material
{
	float3		Diffuse;
	float3 		Specular;
};

// HELPERS
inline float2 ComputeScreenPosition(float4 pos)
{
	float2 screenPos = pos.xy / pos.w;
	return (0.5f * (float2(screenPos.x, -screenPos.y) + 1));
}

inline float4 EncodeFloatRGBA(float v)
{
	float4 kEncodeMul = float4(1.0, 255.0, 65025.0, 160581375.0);
	float kEncodeBit = 1.0 / 255.0;
	float4 enc = kEncodeMul * v;
	enc = frac(enc);
	float r = 1.0 / 255.0;
	enc -= enc.yzww * kEncodeBit;
	return enc;
}

inline float DecodeFloatRGBA(float4 enc)
{
	float4 kDecodeDot = float4(1.0, 1.0 / 255.0, 1.0 / 65025.0, 1.0 / 160581375.0);
	return dot(enc, kDecodeDot);
}

inline float2 EncodeFloatRG(float v)
{
	float2 kEncodeMul = float2(1.0, 255.0);
	float kEncodeBit = 1.0 / 255.0;
	float2 enc = kEncodeMul * v;
	enc = frac(enc);
	enc.x -= enc.y * kEncodeBit;
	return enc;
}

inline float DecodeFloatRG(float2 enc)
{
	float2 kDecodeDot = float2(1.0, 1.0 / 255.0);
	return dot(enc, kDecodeDot);
}

//// NORMALS
inline float2 EncodeViewNormalStereo(float3 n)
{
    float scale = 1.7777;
    float2 enc = n.xy / (n.z + 1);
    enc /= scale;
    return enc * 0.5 + 0.5;
}

inline float3 DecodeViewNormalStereo(float2 enc)
{
    float scale = 1.7777;
    float3 nn = float3(enc.xy * (2 * scale), 0) + float3(-scale, -scale, 1);
    float g = 2.0 / dot(nn.xyz, nn.xyz);
    float3 n;
    n.xy = g * nn.xy;
    n.z = g - 1;
    return n;
}

//inline half2 EncodeNormalLambert(half3 n)
//{
//    half p = sqrt(n.z * 8 + 8);
//    return half2(n.xy / p + 0.5);
//}
//
//inline half3 DecodeNormalLambert(half2 enc)
//{
//    half2 fenc = enc * 4 - 2;
//    half f = dot(fenc, fenc);
//    half g = sqrt(1 - f / 4);
//    half3 n;
//    n.xy = fenc * g;
//    n.z = 1 - f / 2;
//    return n;
//}

inline float4 EncodeDepthNormal(float depth, float3 normal)
{
	float4 enc;
	enc.xy = EncodeViewNormalStereo(normal);
	enc.zw = EncodeFloatRG(depth);
	return enc;
}

inline void DecodeDepthNormal(float4 enc, out float depth, out float3 normal)
{
	normal = DecodeViewNormalStereo(enc.xy);
	depth = DecodeFloatRG(enc.zw);
}

inline half3 EncodeNormalBase(half3 n)
{
	return half3(n.xyz*0.5+0.5);
}

inline half3 DecodeNormalBase(half3 enc)
{
	enc = enc*2-1;
	return normalize(enc);
}

inline half4 EncodeNormalGlossiness(half3 normal, half glossiness)
{
	float4 enc;
	enc.xyz = EncodeNormalBase(normal);
	enc.w = glossiness / 255;
	return enc;
}

inline void DecodeNormalGlossiness(half4 enc, out half3 normal, out half glossiness)
{
	normal = DecodeNormalBase(enc.xyz);
	glossiness = enc.w * 255;
}

inline float4 EncodeLightDiffuseSpecular(float3 diffuse, float specular)
{
	diffuse = diffuse / 2;
	return float4(diffuse, specular);
}

inline void DecodeLightDiffuseSpecular(float4 enc, out float3 diffuse, out float specular)
{
	diffuse = enc.xyz;
	specular = enc.w;
	
	diffuse = diffuse * 2;	
}
