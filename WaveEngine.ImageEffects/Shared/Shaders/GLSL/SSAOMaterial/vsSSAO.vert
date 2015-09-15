uniform mat4  WorldViewProj;
uniform vec2  FilterRadius;

attribute vec3 Position0;
attribute vec2 TextureCoordinate0;

// Poisson 
const vec2 P00 = vec2( -0.94201624,  -0.39906216 );
const vec2 P01 = vec2(  0.94558609,  -0.76890725 );
const vec2 P02 = vec2( -0.094184101, -0.92938870 );
const vec2 P03 = vec2(  0.34495938,   0.29387760 );
const vec2 P04 = vec2( -0.91588581,   0.45771432 );
const vec2 P05 = vec2( -0.81544232,  -0.87912464 );
const vec2 P06 = vec2( -0.38277543,   0.27676845 );
const vec2 P07 = vec2(  0.97484398,   0.75648379 );
const vec2 P08 = vec2(  0.44323325,  -0.97511554 );
const vec2 P09 = vec2(  0.53742981,  -0.47373420 );
const vec2 P10 = vec2( -0.26496911,  -0.41893023 );
const vec2 P11 = vec2(  0.79197514,   0.19090188 );
const vec2 P12 = vec2( -0.24188840,   0.99706507 );
const vec2 P13 = vec2( -0.81409955,   0.91437590 );
const vec2 P14 = vec2(  0.19984126,   0.78641367 );
const vec2 P15 = vec2(  0.14383161,  -0.14100790 );

varying vec2 outTexCoord;
varying vec2 outPoisson[16];

void main()
{
  outTexCoord = TextureCoordinate0;	  
  
  outPoisson[0]   = outTexCoord + (P00 * FilterRadius);
  outPoisson[1]   = outTexCoord + (P01 * FilterRadius);
  outPoisson[2]   = outTexCoord + (P02 * FilterRadius);
  outPoisson[3]   = outTexCoord + (P03 * FilterRadius);
  outPoisson[4]   = outTexCoord + (P04 * FilterRadius);
  outPoisson[5]   = outTexCoord + (P05 * FilterRadius);
  outPoisson[6]   = outTexCoord + (P06 * FilterRadius);
  outPoisson[7]   = outTexCoord + (P07 * FilterRadius);
  outPoisson[8]   = outTexCoord + (P08 * FilterRadius);
  outPoisson[9]   = outTexCoord + (P09 * FilterRadius);
  outPoisson[10]  = outTexCoord + (P10 * FilterRadius);
  outPoisson[11]  = outTexCoord + (P11 * FilterRadius);
  outPoisson[12]  = outTexCoord + (P12 * FilterRadius);
  outPoisson[13]  = outTexCoord + (P13 * FilterRadius);
  outPoisson[14]  = outTexCoord + (P14 * FilterRadius);
  outPoisson[15]  = outTexCoord + (P15 * FilterRadius);  
  
  gl_Position = WorldViewProj * vec4(Position0, 1.0);	
}