#version 330 core
out vec4 outColor;

in VertexData {
    vec2 UV;
} i;

uniform sampler2D uColor;
uniform sampler2D uBrightness;
uniform vec2 uLightScreenPos;
uniform float uDensity;
uniform float uBlurWidth;

const int NUM_SAMPLES = 200;

void main() {

    vec2 ray = i.UV - uLightScreenPos;
    
    vec3 color = texture2D(uColor, i.UV).rgb;
    
    for(int i = 0; i < NUM_SAMPLES; i++) {
        //sample the texture on the pixel-to-center ray getting closer to the center every iteration
        float scale = 1.0 + uBlurWidth * (float(i) / float(NUM_SAMPLES - 1));
        //summing all the samples togheter
        color += ((texture2D(uBrightness, (ray * scale) + uLightScreenPos).xyz) / float(NUM_SAMPLES)) * uDensity;
    }
    
    outColor = vec4(color, 1);
}
