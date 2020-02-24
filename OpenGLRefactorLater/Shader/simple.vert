﻿#version 450
layout(location=0) in vec4 position;
layout(location=1) in vec4 texCoords;

out vec4 vs_texCoords;

void main(void)
{
    gl_Position = position.x;
    vs_texCoords = texCoords;
}