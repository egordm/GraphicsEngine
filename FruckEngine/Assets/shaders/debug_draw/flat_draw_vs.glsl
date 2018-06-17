#version 330 core
layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inNormal;
layout (location = 2) in vec2 inUV;

out VertexData {
    vec3 fragPos;
    vec2 UV;
    vec3 normal;
} o;

uniform mat4 mModel;
uniform mat4 mView;
uniform mat4 mProjection;

void main() {
    vec4 worldPos = mModel * vec4(inPos, 1.0);
    o.fragPos = worldPos.xyz;
    o.UV = inUV;
    o.normal = inNormal;

    gl_Position = mProjection * mView * worldPos;
}
