//-----------------------------------------------------------------------------
// FastBlur.fx
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform float Power;
uniform sampler2D Texture;
uniform sampler2D Texture1;

// Input
varying vec2 outTexCoord;

void main()
{
	vec3 color = texture2D(Texture, outTexCoord).rgb;
	vec3 blur = texture2D(Texture1, outTexCoord).rgb;
	
	float value = sin(outTexCoord.y * 3.141592);
	value = pow(value, Power);
	color = mix(blur, color, value);

	gl_FragColor = vec4(color.rgb, 1.0);
}
