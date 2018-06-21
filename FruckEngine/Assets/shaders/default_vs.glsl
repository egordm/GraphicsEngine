#version 330 core
layout (location = 0) in vec3 inPos;

uniform mat4 mModel;
uniform mat4 mView;
uniform mat4 mProjection;

// Not so default though
void main() {
     gl_Position = mProjection * mView * mModel * vec4(inPos, 1.0);
}
