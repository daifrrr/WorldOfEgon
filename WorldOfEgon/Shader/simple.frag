#version 430

in vec3 normals;
in vec2 texCoords;
layout (binding = 0) uniform sampler2D basic_texture;

out vec4 frag_colour;

void main() {
    frag_colour = texture(basic_texture, vec2(texCoords.x, texCoords.y));
}