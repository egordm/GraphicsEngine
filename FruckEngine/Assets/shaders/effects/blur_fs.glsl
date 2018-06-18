#version 330 core
out vec4 outColor;

in VertexData {
    vec2 UV;
} i;

uniform sampler2D uImage;

uniform bool uHorizontal;
uniform float uWeight[5] = float[] (0.2270270270, 0.1945945946, 0.1216216216, 0.0540540541, 0.0162162162);
// TODO: to change or not to change.

void main() {
    vec2 texOffset = 1.0 / textureSize(uImage, 0);
    vec3 ret = texture(uImage, i.UV).rgb * uWeight[0];
    if(uHorizontal) {
        for(int j = 1; j < 5; ++j) {
            ret += texture(uImage, i.UV + vec2(texOffset.x * j, 0.0)).rgb * uWeight[j];
            ret += texture(uImage, i.UV - vec2(texOffset.x * j, 0.0)).rgb * uWeight[j];
        }
    }
    else {
        for(int j = 1; j < 5; ++j) {
            ret += texture(uImage, i.UV + vec2(0.0, texOffset.y * j)).rgb * uWeight[j];
            ret += texture(uImage, i.UV - vec2(0.0, texOffset.y * j)).rgb * uWeight[j];
        }
    }

    outColor = vec4(ret, 1);
}
