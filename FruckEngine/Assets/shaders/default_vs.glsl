#version 330 core
layout (location = 0) in vec3 inPos;

uniform mat4 mModel;
uniform mat4 mView;
uniform mat4 mProjection;
uniform bool uInfiniteFar = false;


// Not so default though
void main() {
    float ima = uInfiniteFar ? 0 : 1;
     gl_Position = mProjection * mView * mModel * vec4(inPos, ima);
}
