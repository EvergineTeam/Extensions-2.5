// Constants
uniform mat4 WorldViewProj;

// Inputs
attribute vec4 Position0;
attribute vec2 TextureCoordinate0;

// Outputs
varying vec2 outTexCoord;

void main(void)
{
    outTexCoord = TextureCoordinate0;
    gl_Position = WorldViewProj * Position0;
}
