#version 330 core
layout (location = 0) out vec4 outPositionMetallic;
layout (location = 1) out vec4 outNormalRoughness;
layout (location = 2) out vec4 outAlbedoAO;

in VertexData {
    vec3 fragPos;
    vec2 UV;
    vec3 normal;
    vec3 tangent;
    vec3 bitangent;
} i;

struct Material {
    vec3 albedo;
    float metallic;
    float roughness;
    sampler2D albedoTex0;
    sampler2D normalTex0;
    sampler2D metallicTex0;
    sampler2D roughnessTex0;
    sampler2D aoTex0;
};
uniform Material uMaterial;

void main() {
    vec4 color = texture(uMaterial.albedoTex0, i.UV);
    if(color.a < 0.01) discard; // Discard transparent material
    vec3 albedo = color.xyz * uMaterial.albedo;
    
    mat3 TBN = mat3(i.tangent, i.bitangent, i.normal);
    vec3 normal = texture(uMaterial.normalTex0, i.UV).rgb;
    normal = normalize(normal * 2.0 - 1.0);
    normal = normalize(TBN * normal);
    
    float metallic = texture(uMaterial.metallicTex0, i.UV).r * uMaterial.metallic;
    float roughness = texture(uMaterial.roughnessTex0, i.UV).r * uMaterial.roughness;
    float ao = texture(uMaterial.aoTex0, i.UV).r;
    
    outPositionMetallic = vec4(i.fragPos, metallic);
    outNormalRoughness = vec4(normal, roughness);
    outAlbedoAO = vec4(albedo.xyz, ao);
}
