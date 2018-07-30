//-----------------------------------------------------------------------------
// LightShaft.fx
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision highp float;
#endif

#ifdef LOW
#define SAMPLES 32
#endif

#ifdef MEDIUM
#define SAMPLES 64
#endif

#ifdef HIGH
#define SAMPLES 96
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

uniform sampler2D Texture;
uniform sampler2D Texture1;
uniform sampler2D DepthTexture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec2 texCoord = outTexCoord;

	// Calculate vector from pixel to light source in screen space.  
	vec2 deltaTexCoord = (texCoord - LightCenter);

	// Divide by number of samples and scale by control factor.  
	deltaTexCoord *= 1.0 / float(SAMPLES) * Density;

	// Store initial sample.  
	vec4 color = texture2D(Texture, texCoord);

	// Set up illumination decay factor.  
	float illuminationDecay = 1.0;

	for (int i = 0; i < SAMPLES; i++)
	{
		// Step sample location along ray.  
		texCoord -= deltaTexCoord;

		// Retrieve sample at new location.  
		vec4 colorSample = texture2D(Texture, texCoord).rgba;

		// Apply sample attenuation scale/decay factors.  
		colorSample *= illuminationDecay * Weight;

		// Accumulate combined color.   
		color.rgba += colorSample;

		// Update exponential decay factor.  
		illuminationDecay *= Decay;
	}

	// Output final color with a further scale control factor.  
	gl_FragColor = color * Exposure;
}
