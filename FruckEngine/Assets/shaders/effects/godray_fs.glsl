#version 330 core
out vec4 outColor;

in VertexData {
    vec2 UV;
} i;

uniform sampler2D uBrightness;
uniform vec2 uLightScreenPos;
uniform bool uVisible;

const float decay=0.96815;
const float exposure=0.2;
const float density=0.926;
const float weight=0.58767;

const int NUM_SAMPLES = 100;

const float blurWidth = -0.85;


void main() {

    vec2 ray = i.UV - uLightScreenPos;
    
    vec3 color = vec3(0.0);
    
    if(uVisible) {
        for(int i = 0; i < NUM_SAMPLES; i++) {
            //sample the texture on the pixel-to-center ray getting closer to the center every iteration
            float scale = 1.0 + blurWidth * (float(i) / float(NUM_SAMPLES - 1));
            //summing all the samples togheter
            color += ((texture2D(uBrightness, (ray * scale) + uLightScreenPos).xyz) / float(NUM_SAMPLES)) * 0.3;
        }
    }
    
    outColor = vec4(color, 1);
}
