#version 430

layout (location = 0) in vec3 aPositions;
layout (location = 1) in vec3 aNormals;
layout (location = 2) in vec2 aTexCoords;

layout (location = 10) uniform mat4 uModel;
layout (location = 11) uniform mat4 uView;
layout (location = 12) uniform mat4 uProjection;

layout (location = 50) uniform double uTime;

out vec3 normals;
out vec2 texCoords;

void main() {
    texCoords = aTexCoords;
    normals = aNormals;
    gl_Position = uProjection * uView * uModel * vec4(aPositions, 1.0);
}