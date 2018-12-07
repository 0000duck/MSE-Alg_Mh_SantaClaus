#version 400 core
layout (location = 0) in vec2 pos;

uniform mat4 T;

void main()
{
    gl_Position = T * vec4(pos.x, pos.y, 0.0, 1.0);
}