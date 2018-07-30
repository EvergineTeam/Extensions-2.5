//-----------------------------------------------------------------------------
// LightShaft.fx
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision highp float;
#endif

// Parameters
uniform vec2 LightCenter;
uniform vec2 TexcoordOffset;
uniform float Density;	
uniform float Weight;		
uniform float Decay;			
uniform float Exposure;
uniform float Blend;		
uniform vec2 ShaftTint;
uniform float Radius;		
uniform float EdgeSharpness;
uniform float SunIntensity;	
uniform float DepthThreshold;

uniform sampler2D Texture;
uniform sampler2D Texture1;
uniform sampler2D DepthTexture;


// Input
varying vec2 outTexCoord;

void main()
{
	float depth = texture2D(DepthTexture, outTexCoord).x;
	depth = (depth - DepthThreshold);
	depth = max(depth, 0.0) * SunIntensity;

	float light = depth;

#ifdef HALO
	float dis = distance(LightCenter, outTexCoord);
	float sun = 1.0 - pow(dis * (1.0 / Radius), EdgeSharpness);

	light *= sun;
#endif

	// Linear Filtering
	vec3 color = texture2D(Texture, outTexCoord).rgb;
	color += texture2D(Texture, outTexCoord + vec2(0, TexcoordOffset.y)).rgb;
	color += texture2D(Texture, outTexCoord + vec2(TexcoordOffset.x, 0)).rgb;
	color += texture2D(Texture, outTexCoord + TexcoordOffset).rgb;

	color.rgb * 0.25;

	gl_FragColor = vec4(color.rgb * light, light);
}
