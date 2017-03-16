//-----------------------------------------------------------------------------
// Sepia.fx
//
// Copyright © 2017 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform vec3 ImageTone;
uniform float Desaturation;
uniform vec3 DarkTone;
uniform float Toning;
uniform vec3 GreyTransfer;
uniform float GlobalAlpha;
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec3 pixelColor = texture2D(Texture, outTexCoord).rgb;

	vec3 scene = pixelColor * ImageTone;

	float grey = dot(GreyTransfer, scene);
	vec3 muted = mix(scene, vec3(grey, grey, grey), Desaturation);
	vec3 sepia = mix(DarkTone, ImageTone, grey);
	pixelColor = mix(muted, sepia, Toning);

	gl_FragColor = vec4(pixelColor, GlobalAlpha);
}
