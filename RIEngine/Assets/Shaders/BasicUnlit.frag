#version 330 core
in vec4 vertexColor;
in vec2 texCoord;

out vec4 fragColor;

uniform sampler2D texture1;

void main()
{
    fragColor = texture(texture1, texCoord);
}