#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    outputColor = vec4(1,1,1,1);//vec4(normal.xyz, 1 );
}