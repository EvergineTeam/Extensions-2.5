uniform mat4	WorldViewProj;

attribute vec3 Position0;
attribute vec2 TextureCoordinate0;

varying vec2 outTexCoord;

void main()
{
	gl_Position = WorldViewProj * vec4(Position0, 1.0);
	outTexCoord = TextureCoordinate0;
}
