//-----------------------------------------------------------------------------
// LensFlare.fx
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Paramenters
uniform vec2 StarRotation;
uniform float Intensity;

uniform sampler2D Texture;
uniform sampler2D LensDirtTexture;
uniform sampler2D LensStarTexture;
uniform sampler2D LensFlareTexture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec3 color = texture2D(Texture, outTexCoord).rgb;

	vec2 center = vec2(0.5);
	vec2 uv = outTexCoord - center;
	vec2 texcoord = vec2(uv.x * StarRotation.x - uv.y * StarRotation.y,
						 uv.x * StarRotation.y + uv.y * StarRotation.x);
	texcoord += center;

	vec3 lensMod = texture2D(LensDirtTexture, outTexCoord).rgb;
	lensMod += texture2D(LensStarTexture, texcoord).rgb;

	vec3 lensFlare = texture2D(LensFlareTexture, outTexCoord).rgb;
	color += lensFlare * lensMod * Intensity;	

	gl_FragColor = vec4(color.rgb, 1.0);
}
