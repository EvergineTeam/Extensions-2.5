//-----------------------------------------------------------------------------
// Tiling.fx
//
// Copyright © 2018 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------

#ifdef GL_ES
precision mediump float;
#endif

// Parameters
uniform vec3 EdgeColor;
uniform float NumTiles;
uniform float Threshhold;
uniform sampler2D Texture;

// Input
varying vec2 outTexCoord;

vec2 fmod(vec2 a, vec2 b)
{
  vec2 c = fract(abs(a / b)) * abs(b);
  return abs(c);
}

void main()
{
    float size = 1.0 / NumTiles;

	vec2 Pbase = outTexCoord - fmod(outTexCoord, vec2(size));
	vec2 PCenter = Pbase + vec2(size / 2.0);
	vec2 st = (outTexCoord - Pbase) / size;
	vec4 c1 = vec4(0);
	vec4 c2 = vec4(0);
	vec4 invOff = vec4((1.0 - EdgeColor), 1.0);

	if (st.x > st.y)
	{
		c1 = invOff; 
	}

	float threshholdB = 1.0 - Threshhold;

	if (st.x > threshholdB) 
	{ 
		c2 = c1; 
	}

	if (st.y > threshholdB) 
	{ 
		c2 = c1; 
	}

	vec4 cBottom = c2;
	c1 = vec4(0);
	c2 = vec4(0);
	
	if (st.x > st.y)
	{ 
		c1 = invOff; 
	}

	if (st.x < Threshhold) 
	{ 
		c2 = c1;
	}
	
	if (st.y < Threshhold) 
	{ 
		c2 = c1; 
	}

	vec4 cTop = c2;
	vec4 tileColor = texture2D(Texture, PCenter);
	gl_FragColor = tileColor + cTop - cBottom;
}
