#version 330 core
out vec4 outBrightColor;

in VertexData {
    vec3 UV;
} i;

uniform samplerCube uImage;

void main() {
    vec4 color = texture(uImage, i.UV);

    // TODO: this is nasty dont duplicate code pls
    // Brightness calculation for bloom effect based on thresholding
    float brightness = dot(color.rgb, vec3(0.2126, 0.7152, 0.0722)); // TODO: dont hardcode the threshold. Or do?
    if(brightness > 1.0) outBrightColor = vec4(color.rgb, 1.0);
    else outBrightColor = vec4(0.0, 0.0, 0.0, 1.0);
}
