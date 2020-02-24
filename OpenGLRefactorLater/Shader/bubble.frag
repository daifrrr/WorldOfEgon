#version 430

in vec2 texCoords;
in float timer;

layout (binding = 0) uniform sampler2D basic_texture;

out vec4 frag_colour;

void main()
{
    vec4 temp_color = texture(basic_texture, vec2(texCoords.x, texCoords.y));
    if(temp_color.a < 0.01) discard;
    frag_colour = temp_color;
}