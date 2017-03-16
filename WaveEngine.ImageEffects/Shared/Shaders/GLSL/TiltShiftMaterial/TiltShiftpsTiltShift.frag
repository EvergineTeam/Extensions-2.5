//-----------------------------------------------------------------------------
// FastBlur.fx
//
// Copyright © 2017 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform float Power;
uniform float TiltPosition;
uniform sampler2D Texture;
uniform sampler2D Texture1;

// Input
varying vec2 outTexCoord;

void main()
{
	vec3 color = texture2D(Texture, outTexCoord).rgb;
	vec3 blur = texture2D(Texture1, outTexCoord).rgb;
	
	float value = clamp(cos((outTexCoord.y - TiltPosition) * 3.141592), 0.0, 1.0);
	value = pow(value, Power);
	color = mix(blur, color, value);

	gl_FragColor = vec4(color.rgb, 1.0);
}
