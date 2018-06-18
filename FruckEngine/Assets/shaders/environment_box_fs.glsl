#version 330 core
layout (location = 0) out vec4 outColor;
//layout (location = 1) out vec4 outBrightColor;

in VertexData {
    vec3 UV;
} i;

uniform samplerCube uImage;

void main() {
    outColor = texture(uImage, i.UV);

//    // TODO: this is nasty dont duplicate code pls
//    // Brightness calculation for bloom effect based on thresholding
//    float brightness = dot(outColor.rgb, vec3(0.2126, 0.7152, 0.0722)); // TODO: dont hardcode the threshold. Or do?
//    if(brightness > 1.0) outBrightColor = vec4(outColor.rgb, 1.0);
//    else outBrightColor = vec4(0.0, 0.0, 0.0, 1.0);
}
