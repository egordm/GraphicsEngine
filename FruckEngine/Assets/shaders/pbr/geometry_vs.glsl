#version 330 core
layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inNormal;
layout (location = 2) in vec2 inUV;
layout (location = 3) in vec3 inTangent;
layout (location = 4) in vec3 inBitangent;

out VertexData {
    vec3 fragPos;
    vec2 UV;
    vec3 normal;
    vec3 tangent;
    vec3 bitangent;
} o;

uniform mat4 mModel;
uniform mat4 mView;
uniform mat4 mProjection;
uniform float uOffset;

void main() {
    vec4 worldPos = mModel * vec4(inPos + inNormal * uOffset, 1.0);
    o.fragPos = worldPos.xyz;
    o.UV = inUV;

    // Normal calculation
    mat3 normalMatrix = transpose(inverse(mat3(mModel)));
    vec3 T = normalize(normalMatrix * inTangent);
    vec3 B = normalize(normalMatrix * inBitangent);
    vec3 N = normalize(normalMatrix * inNormal);
    o.tangent = T;
    o.bitangent = B;
    o.normal = N;
    
    gl_Position = mProjection * mView * worldPos;
}
