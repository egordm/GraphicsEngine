#version 330 core
out vec4 outColor;

in VertexData {
    vec2 UV;
} i;

uniform sampler2D uShaded;
uniform sampler2D uBloom;

uniform bool uApplyBloom;
uniform float uExposure;

void main() {
    vec3 color = texture(uShaded, i.UV).rgb;
    vec3 bloom = texture(uBloom, i.UV).rgb;
    
    // Apply bloom
    if(uApplyBloom) color += bloom;
    
    vec3 ret = vec3(1.0) - exp(-color * uExposure);

    outColor = vec4(ret, 1.0);
}
