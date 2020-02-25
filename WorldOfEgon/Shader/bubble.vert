#version 430

layout (location = 0) in vec3 aPositions;
layout (location = 2) in vec2 aTexCoords;

layout (location = 10) uniform mat4 uModel;
layout (location = 11) uniform mat4 uView;
layout (location = 12) uniform mat4 uProjection;
layout (location = 50) uniform float uTime;
layout (location = 100) uniform float aspectRatio;

out vec2 texCoords;
out float timer;

void main() {
    texCoords = aTexCoords;
    timer += uTime;
    mat4 sModel = uModel;
    vec4 aspectedPositions = vec4(aPositions.x / aspectRatio, aPositions.y, aPositions.z, 1.0);
    if (sModel[3][1] < 1.5)
    {
        sModel[3][1] = -1.5;
        timer = -1.5;
    }
    sModel[3][1] += timer;
    gl_Position = sModel * aspectedPositions;
}