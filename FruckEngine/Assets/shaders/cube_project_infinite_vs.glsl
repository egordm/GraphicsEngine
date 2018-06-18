#version 330 core
layout (location = 0) in vec3 inPos;

out VertexData {
    vec3 UV;
} o;

uniform mat4 mView;
uniform mat4 mProjection;

void main() {
    o.UV = inPos;
    vec4 pos = mProjection * mView * vec4(inPos, 0);
    gl_Position = pos.xyww;
}
