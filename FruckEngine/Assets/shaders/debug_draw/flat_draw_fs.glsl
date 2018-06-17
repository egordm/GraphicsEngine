#version 330 core
out vec4 outColor;

in VertexData {
    vec3 fragPos;
    vec2 UV;
    vec3 normal;
} i;

void main() {
    outColor = vec4(i.fragPos, 1);
}
