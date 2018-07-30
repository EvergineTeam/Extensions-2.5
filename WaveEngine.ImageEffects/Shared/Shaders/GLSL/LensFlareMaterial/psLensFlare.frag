//-----------------------------------------------------------------------------
// LensFlare.fx
//
// Copyright © 2018 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

#define numGhosts 8

// Paramenters
uniform float HaloWidth;
uniform float GhostDispersal;
uniform float Distortion;
uniform vec2 TexcoordOffset;

uniform sampler2D Texture;
uniform sampler2D LensColorTexture;

// Input
varying vec2 outTexCoord;
varying vec4 OutUV[8];

// Helper functions
vec3 TextureDistorted(
	in vec2 texcoord,
	in vec2 direction,
	in vec3 distortion
	) 
{
	return vec3(texture2D(Texture, texcoord + direction * distortion.r).r,
				texture2D(Texture, texcoord + direction * distortion.b).g,
				texture2D(Texture, texcoord + direction * distortion.b).b);
}

void main()
{
	vec2 texcoord = -outTexCoord + 1.0;

	vec2 ghostVec = (0.5 - texcoord) * GhostDispersal;
	vec2 haloVec = normalize(ghostVec) * HaloWidth;

	vec3 distortion = vec3(-TexcoordOffset.x * Distortion, 0.0, TexcoordOffset.x * Distortion);

	// Sample ghosts:  
	vec3 color = vec3(0.0);
	vec2 center = vec2(0.5);
	
	for (int i = 0; i < numGhosts; ++i)
	{
		vec2 offset = fract(texcoord + ghostVec * float(i));

		float weight = length(center - offset) / length(center);
		weight = pow(1.0 - weight, 10.0);

		color += TextureDistorted(offset, normalize(ghostVec), distortion) * weight;
	}

	// Apply color
	vec2 uv = vec2(length(center - texcoord) / length(center));
	color *= texture2D(LensColorTexture, uv).rgb;

	// Sample halo:
	float weight = length(center - fract(texcoord + haloVec)) / length(center);
	weight = pow(1.0 - weight, 5.0);
	color += TextureDistorted(fract(texcoord + haloVec), normalize(ghostVec), distortion) * weight;

	gl_FragColor = vec4(color.rgb, 1.0);
}
