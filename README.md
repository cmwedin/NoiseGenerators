# Noise Generators
This package is a collection of compute shaders that can generate various types of noise textures. This README is currently a placeholder and will be update as development continues

# Psuedo-Random Number generation approach

This package uses a "permuted congruential generator" (more commonly known as PCG) algorithm  as it lies on the pareto-fronter of random quality vs preformance (see [Hash Functions for GPU Rendering](https://jcgt.org/published/0009/03/02/)). The particular PCG procedure this package uses is: 
    uint pcg_hash(uint seed) {
        uint state = seed * 747796405u + 2891336453u;
        uint word = ((state >> ((state >> 28u) + 4u)) ^ state) * 277803737u;
        return (word >> 22u) ^ word;
    } 

# Random noise texture

uses random number directly to compute the noise

# Perlin noise texture
[perlin noise](https://en.wikipedia.org/wiki/Perlin_noise)