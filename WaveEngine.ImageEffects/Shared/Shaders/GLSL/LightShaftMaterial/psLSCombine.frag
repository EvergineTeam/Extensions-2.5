//-----------------------------------------------------------------------------
// LightShaft.fx
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform vec2 LightCenter;
uniform vec2 TexcoordOffset;
uniform float Density;	
uniform float Weight;		
uniform float Decay;			
uniform float Exposure;
uniform float Blend;		
uniform vec3 ShaftTint;
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
	vec3 color = texture2D(Texture, outTexCoord).rgb;
	vec4 lightShaft = texture2D(Texture1, outTexCoord);
	
	lightShaft.rgb *= ShaftTint;
	
	float faceShaft = lightShaft.a * Blend;
	lightShaft.rgb *= faceShaft;

	gl_FragColor = vec4(color.rgb + lightShaft.rgb, 1.0);
}
