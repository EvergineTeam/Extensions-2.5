#ifdef GL_ES
precision mediump float;
#endif

uniform float LinesFactor;
uniform float Attenuation;
uniform sampler2D Texture;

varying vec2 outTexCoord;

void main()
{
	vec3 color = texture2D(Texture, outTexCoord).xyz;

	float scanline = sin(outTexCoord.y * LinesFactor) * Attenuation;
	color -= scanline;

	gl_FragColor = vec4(color.xyz, 1.0);
}
