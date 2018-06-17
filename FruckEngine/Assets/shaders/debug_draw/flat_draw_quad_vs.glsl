#version 330 core
layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inNormal;
layout (location = 2) in vec2 inUV;

out VertexData {
    vec3 fragPos;
    vec2 UV;
    vec3 normal;
} o;

void main() {
    o.fragPos = inPos;
    o.UV = inUV;
    o.normal = inNormal;
    gl_Position = vec4(inPos, 1.0);
}
