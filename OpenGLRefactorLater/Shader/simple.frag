#version 450

layout(location = 0) in vec4 vs_texCoord;

out vec4 color;

uniform sampler2D texture1;

void main(void)
{
    color = texelFetch(texture1, ivec2(vs_texCoord.x, vs_texCoord.y), 0);
}