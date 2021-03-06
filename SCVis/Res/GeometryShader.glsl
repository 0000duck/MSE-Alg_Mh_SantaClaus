﻿#version 400 core
#define R 0.005
layout (points) in;
layout (triangle_strip, max_vertices = 5) out;

void build_house(vec4 position)
{    
    gl_Position = position + vec4(-R, 0.0, 0.0, 0.0);
    EmitVertex();
	gl_Position = position + vec4(0.0, -R, 0.0, 0.0);
	EmitVertex();
	gl_Position = position + vec4(R, 0.0, 0.0, 0.0);
	EmitVertex();
	gl_Position = position + vec4(0.0, R, 0.0, 0.0);
	EmitVertex();
	gl_Position = position + vec4(-R, 0.0, 0.0, 0.0);
	EmitVertex();
    EndPrimitive();
}

void main() {    
    build_house(gl_in[0].gl_Position);
} 