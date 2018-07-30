#ifdef GL_ES
precision mediump float;
#endif

uniform sampler2D LuminanceTexture;
uniform sampler2D ChromaTexture;

// Inputs
varying vec2 outTexCoord;

void main(void)
{
	mat4 ycbcrToRGBTransform = mat4(
    vec4(+1.0000, +1.0000, +1.0000, +0.0000),
    vec4(+0.0000, -0.3441, +1.7720, +0.0000),
    vec4(+1.4020, -0.7141, +0.0000, +0.0000),
    vec4(-0.7010, +0.5291, -0.8860, +1.0000));

	vec4 ycbcr = vec4(texture2D(LuminanceTexture, outTexCoord).r,
					  texture2D(ChromaTexture, outTexCoord).ra, 1.0);
	gl_FragColor = ycbcrToRGBTransform * ycbcr;
}