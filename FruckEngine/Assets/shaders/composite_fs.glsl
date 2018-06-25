#version 330 core
out vec4 outColor;

in VertexData {
    vec2 UV;
} i;

uniform sampler2D uShaded;
uniform sampler2D uBloom;
uniform sampler2D uGodrays;
uniform sampler2D uColorLUT;
uniform sampler2D uUI;

uniform int uColorLUTSize = 16;
uniform bool uApplyBloom;
uniform bool uApplyColorGrade;
uniform float uExposure;

vec3 LookupColor(vec3 uv, float width);
void main() {
    vec3 color = texture(uShaded, i.UV).rgb;
    vec3 bloom = texture(uBloom, i.UV).rgb;
    vec3 godrays = texture(uGodrays, i.UV).rgb;
    vec4 ui = texture(uUI, vec2(i.UV.x, 1 - i.UV.y));
    
    // Apply bloom
    if(uApplyBloom) color += bloom;
    color += godrays;
    if(ui.a > 0) color = mix(ui.rgb, color,  0.4);
    
    vec3 ret = vec3(1.0) - exp(-color * uExposure);
    
    if(uApplyColorGrade) ret = LookupColor(ret, uColorLUTSize);

    outColor = vec4(ret, 1.0);
}


vec3 LookupColor(vec3 uv, float width) {
    // Shameless Internet copy paste
    float sliceSize = 1.0 / width;
    float slicePixelSize = sliceSize / width;
    float sliceInnerSize = slicePixelSize * (width - 1.0);
    float zSlice0 = min(floor(uv.z * width), width - 1.0);
    float zSlice1 = min(zSlice0 + 1.0, width - 1.0);
    float xOffset = slicePixelSize * 0.5 + uv.x * sliceInnerSize;
    float s0 = xOffset + (zSlice0 * sliceSize);
    float s1 = xOffset + (zSlice1 * sliceSize);
    vec4 slice0Color = texture2D(uColorLUT, vec2(s0, uv.y));
    vec4 slice1Color = texture2D(uColorLUT, vec2(s1, uv.y));
    float zOffset = mod(uv.z * width, 1.0);
    vec3 result = mix(slice0Color, slice1Color, zOffset).rgb;
    return result;
}
