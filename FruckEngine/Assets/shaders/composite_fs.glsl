#version 330 core
out vec4 outColor;

in VertexData {
    vec2 UV;
} i;

uniform sampler2D uShaded;

void main() {
    vec3 color = texture(uShaded, i.UV).rgb;
    
    vec3 ret = color;

    outColor = vec4(ret, 1.0);
}
