//-----------------------------------------------------------------------------
// LightShaft.fx
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision highp float;
#endif

#ifdef HIGH
#define numSamples 9
#endif

#ifdef MEDIUM
#define numSamples 6
#endif

#ifdef LOW
#define numSamples 4
#endif

// Parameters
uniform float BlurLength;			
uniform mat4 ViewProjectionInverse;
uniform mat4 PreviousViewProjection;

uniform sampler2D Texture;
uniform sampler2D DepthTexture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec2 texCoord = outTexCoord;
	float depth = texture2D(DepthTexture, texCoord).x;

	// H is the viewport position at this pixel in the range -1 to 1.
	vec4 H = vec4(texCoord.x * 2.0 - 1.0, (1.0 - texCoord.y) * 2.0 - 1.0, depth, 1.0);

	// Transform by the view-projection inverse.
	vec4 D = ViewProjectionInverse * H;
	vec4 worldPos = D / D.w;

	// Current viewport position
	vec4 currentPos = H;
		
	// Use the world position and transform by the previous view-projection matrix.
	vec4 previousPos = PreviousViewProjection * worldPos;

	// Convert to nonhomegeneous points [-1, 1] by dividing by w.
	previousPos.xy /= previousPos.w;

	// Use this frame's position and last frame's to compute the pixel velocity
	vec2 velocity = (currentPos.xy - previousPos.xy) * BlurLength;
	velocity.y = -velocity.y;
	
	velocity /= vec2(numSamples);	

	// Get the initial color at this pixel
	vec4 color = texture2D(Texture, texCoord);
	texCoord += velocity;
	
	// Accumulate in the color.
	for (int i = 1; i < numSamples; ++i, texCoord += velocity)
	{
		color += texture2D(Texture, texCoord);
	}

	// Average all of the samples to get the final blur color
	gl_FragColor = color / vec4(numSamples);
}
