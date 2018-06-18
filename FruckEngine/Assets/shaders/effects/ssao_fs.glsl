#version 330 core
out float outColor;

in VertexData {
    vec2 UV;
} i;

uniform sampler2D uPositions;
uniform sampler2D uNormals;
uniform sampler2D uTexNoise;

uniform vec2 uNoiseScale;
uniform int uKernelSize;
uniform float uKernelRadius;
uniform vec3 uKernelSamples[64];
uniform float uStrength;

uniform mat4 mView;
uniform mat4 mProjection;

const float bias = 0.025;

// bbb b but i want hbao+ ;-;. Mb later
void main() {
    vec3 randomVec = normalize(texture(uTexNoise, i.UV * uNoiseScale).xyz);
    vec3 normal = (mView * vec4(texture(uNormals, i.UV).rgb, 0)).xyz;
    vec3 fragPos = (mView * vec4(texture(uPositions, i.UV).xyz, 1.0)).xyz;

    // TBN matrix from tangent to view
    vec3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
    vec3 bitangent = cross(normal, tangent);
    mat3 TBN = mat3(tangent, bitangent, normal);


    // Iterate over kernel and calculate occlusion
    float occlusion = 0.0;
    for(int i = 0; i < uKernelSize; ++i) {
        vec3 kernelSample = TBN * uKernelSamples[i]; // Kernel pos is in tangent space
        kernelSample = fragPos + kernelSample * uKernelRadius;

        // Project to clipspace to get UV on screen quad
        vec4 offset = mProjection * vec4(kernelSample, 1.0);
        offset.xyz /= offset.w; // perspective divide
        offset.xyz = offset.xyz * 0.5 + 0.5;

        // Sample depth
        float sampleDepth = (mView * vec4(texture(uPositions, offset.xy).xyz, 1.0)).z;

        // Range check and accumulate
        float rangeCheck = smoothstep(0.0, 1.0, uKernelRadius / abs(fragPos.z - sampleDepth));
        occlusion += (sampleDepth >= kernelSample.z + bias ? 1.0 : 0.0) * rangeCheck;
    }

    occlusion = 1.0 - (occlusion / float(uKernelSize));
    occlusion = pow(occlusion, uStrength);
    outColor = occlusion;
}
