//-----------------------------------------------------------------------------
// RadialBlur.fx
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform int Nsamples;
uniform float BlurWidth;
uniform vec2 Center;
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

void main()
{
	vec2 uv = outTexCoord - Center;
	float precompute = BlurWidth * (1.0 / (float(Nsamples) - 1.0));

	vec3 color = vec3(0.0, 0.0, 0.0);

	for (int i = 0; i < Nsamples; i++)
	{
		float scale = 1.0 + (float(i) * precompute);
		color += texture2D(Texture, uv * scale + Center).xyz;
	}

	color /= float(Nsamples);

	gl_FragColor = vec4(color, 1.0);
}
