#version 330 core
layout (location = 0) out vec4 outColor;
layout (location = 1) out vec4 outBrightColor;

in VertexData {
    vec2 UV;
} i;

struct DirectionalLight {
    vec3 direction;
    vec3 color;
    float intensity;
};

struct Light {
    vec3 position;
    vec3 color;
    float intensity;
};

// Geometry & AO
uniform sampler2D uPositionMetallic;
uniform sampler2D uNormalRoughness;
uniform sampler2D uAlbedoAO;
uniform sampler2D uSSAO;

// IBL
uniform samplerCube uIrradianceMap;
uniform samplerCube uPrefilterMap;
uniform sampler2D uBrdfLUT;

uniform vec3 uViewPos;

uniform vec3 uAmbientLight;

const int MAX_N_LIGHTS = 32;
uniform int uPointLightCount;
uniform Light uPointLights[MAX_N_LIGHTS];
uniform DirectionalLight uDirectionalLight;

const float PI = 3.14159265359;
const float RADIAL_LIGHT_DECAY = 0.81;

float DistributionGGX(vec3 N, vec3 H, float roughness);
float GeometrySchlickGGX(float NdotV, float roughness);
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness);
vec3 FresnelSchlick(float cosTheta, vec3 F0);
vec3 FresnelSchlickRoughness(float cosTheta, vec3 F0, float roughness);

vec3 CalcPointLight(vec3 pos, vec3 N, vec3 V, vec3 F0, vec3 albedo, float metallic, float roughness);
vec3 CalcDirectionalLight(vec3 pos, vec3 N, vec3 V, vec3 F0, vec3 albedo, float metallic, float roughness);


void main() {
    vec3 pos = texture(uPositionMetallic, i.UV).rgb;
    float metallic = texture(uPositionMetallic, i.UV).a;
    float roughness = texture(uNormalRoughness, i.UV).a;
    vec3 albedo = texture(uAlbedoAO, i.UV).rgb;
    float ao = texture(uAlbedoAO, i.UV).a * texture(uSSAO, i.UV).r;
    
    albedo = pow(albedo, vec3(2.2));
    
    vec3 N = texture(uNormalRoughness, i.UV).rgb;
    vec3 V = normalize(uViewPos - pos);
    vec3 R = reflect(-V, N);
    
    // reflectance at normal incidence. F0 = Also specular?
    vec3 F0 = vec3(0.04);
    F0 = mix(F0, albedo, metallic);
    
    // Calculate lighting
    vec3 Lo = CalcPointLight(pos, N, V, F0, albedo, metallic, roughness);
    Lo += CalcDirectionalLight(pos, N, V, F0, albedo, metallic, roughness);
   
    vec3 F = FresnelSchlickRoughness(max(dot(N, V), 0.0), F0, roughness);
    
    vec3 kS = F;
    vec3 kD = 1.0 - kS;
    kD *= 1.0 - metallic;
    
    vec3 irradiance = texture(uIrradianceMap, N).rgb * uAmbientLight; 
    vec3 diffuse = irradiance * albedo;
    
    const float MAX_REFLECTION_LOD = 4.0;
    vec3 prefilteredColor = textureLod(uPrefilterMap, R,  roughness * MAX_REFLECTION_LOD).rgb;
    vec2 brdf  = texture(uBrdfLUT, vec2(max(dot(N, V), 0.0), roughness)).rg;
    vec3 specular = prefilteredColor * (F * brdf.x + brdf.y);
    
    vec3 ambient = (kD * diffuse + specular) * ao;
    
    vec3 color = ambient + Lo;
    
    // HDR tonemapping
    //color = color / (color + vec3(1.0));
    // gamma correct
    color = pow(color, vec3(1.0/2.2));
    
    outColor = vec4(color, 1.0);
    
    // Brightness calculation for bloom effect based on thresholding
    float brightness = dot(color, vec3(0.2126, 0.7152, 0.0722)); // TODO: dont hardcode the threshold. Or do?
    if(brightness > 1.0) outBrightColor = vec4(color, 1.0);
    else outBrightColor = vec4(0.0, 0.0, 0.0, 1.0);
}


vec3 CalcDirectionalLight(vec3 pos, vec3 N, vec3 V, vec3 F0, vec3 albedo, float metallic, float roughness) {
    vec3 Lo = vec3(0.0);

    vec3 L = -uDirectionalLight.direction;
    vec3 H = normalize(V + L);
    vec3 radiance = uDirectionalLight.color * uDirectionalLight.intensity;
    
    // Cook-Torrance BRDF
    float NDF = DistributionGGX(N, H, roughness);
    float G   = GeometrySmith(N, V, L, roughness);
    vec3 F    = FresnelSchlick(max(dot(H, V), 0.0), F0);
    
    vec3 nominator    = NDF * G * F;
    float denominator = 4 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.001; // 0.001 to prevent divide by zero.
    vec3 specular = nominator / denominator;
    
     // kS is equal to Fresnel
    vec3 kS = F;
    vec3 kD = vec3(1.0) - kS;
    kD *= 1.0 - metallic;
    
    // scale light by NdotL
    float NdotL = max(dot(N, L), 0.0);
    
    // add to outgoing radiance Lo
    return (kD * albedo / PI + specular) * radiance * NdotL;
}

vec3 CalcPointLight(vec3 pos, vec3 N, vec3 V, vec3 F0, vec3 albedo, float metallic, float roughness) {
    vec3 Lo = vec3(0.0);
    for(int i = 0; i < MAX_N_LIGHTS; ++i) {
        Light lite = uPointLights[i];
        vec3 L = lite.position - pos;
        float distance_sq = dot(L, L);
        if(distance_sq > lite.intensity*lite.intensity * RADIAL_LIGHT_DECAY) continue;
        
        L = L / sqrt(distance_sq);
        vec3 H = normalize(V + L);
        float attenuation = lite.intensity / distance_sq;
        vec3 radiance = lite.color * attenuation;
        
        // Cook-Torrance BRDF
        float NDF = DistributionGGX(N, H, roughness);
        float G   = GeometrySmith(N, V, L, roughness);
        vec3 F    = FresnelSchlick(max(dot(H, V), 0.0), F0);
        
        vec3 nominator    = NDF * G * F;
        float denominator = 4 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.001; // 0.001 to prevent divide by zero.
        vec3 specular = nominator / denominator;
        
         // kS is equal to Fresnel
        vec3 kS = F;
        vec3 kD = vec3(1.0) - kS;
        kD *= 1.0 - metallic;
        
        // scale light by NdotL
        float NdotL = max(dot(N, L), 0.0);
        
        // add to outgoing radiance Lo
        Lo += (kD * albedo / PI + specular) * radiance * NdotL;
    }
    return Lo;
}

float DistributionGGX(vec3 N, vec3 H, float roughness) {
    float a = roughness*roughness;
    float a2 = a*a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;

    float nom   = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return nom / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness) {
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;

    float nom   = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return nom / denom;
}

float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness) {
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}

vec3 FresnelSchlick(float cosTheta, vec3 F0) {
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

vec3 FresnelSchlickRoughness(float cosTheta, vec3 F0, float roughness) {
    return F0 + (max(vec3(1.0 - roughness), F0) - F0) * pow(1.0 - cosTheta, 5.0);
}