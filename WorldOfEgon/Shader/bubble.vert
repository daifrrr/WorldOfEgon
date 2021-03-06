﻿#version 430

layout (location = 0) in vec3 aPositions;
layout (location = 2) in vec2 aTexCoords;

layout (location = 50) uniform float uTime;
layout (location = 100) uniform vec2 uResultion;

layout (location = 10) uniform mat4 uModel;
layout (location = 11) uniform mat4 uView;
layout (location = 12) uniform mat4 uProjection;

out vec2 texCoords;
out float timer;

void main() {
    texCoords = aTexCoords;
    timer += uTime;
    gl_Position = uModel * vec4(aPositions.x, aPositions.yz, 1.0);
}