# Noise Generators
This package is a collection of compute shaders that can generate various types of noise textures. This README is currently a placeholder and will be update as development continues

# Psuedo-Random Number generation approach

This package uses a "permuted congruential generator" (more commonly known as PCG) algorithm  as it lies on the pareto-fronter of random quality vs preformance (see [Hash Functions for GPU Rendering](https://jcgt.org/published/0009/03/02/)). The particular PCG procedure this package uses is: 

    uint pcg_hash(uint seed) {
        seed = seed * 747796405u + 2891336453u;
        seed = ((seed >> ((seed >> 28u) + 4u)) ^ seed) * 277803737u;
        return (seed >> 22u) ^ seed;
    }

we also include the following 3 -> 3 and 4 -> 4 hash methods, respectively: 

    uint3 pcg3d_hash(uint3 seed) {
        seed = seed * 1664525u + 1013904223u;
        seed.x += seed.y*seed.z; seed.y += seed.z*seed.x; seed.z += seed.x*seed.y;
        seed ^= seed >> 16u;
        seed.x += seed.y*seed.z; seed.y += seed.z*seed.x; seed.z += seed.x*seed.y;
        return seed;
    }
    uint4 pcg4d_hash(uint4 seed) {
        seed = seed * 1664525u + 1013904223u;
        seed.x += seed.y*seed.w; seed.y += seed.z*seed.x; seed.z += seed.x*seed.y; seed.w += seed.y*seed.z;
        seed ^= seed >> 16u;
        seed.x += seed.y*seed.w; seed.y += seed.z*seed.x; seed.z += seed.x*seed.y; seed.w += seed.y*seed.z;
        return seed;
    }

a 2d hash can be generated by discarding the z component of the 3d hash.


# Random noise texture
The is the simplest approach to generating a random noise texture in that it simply uses random values for the color of each pixel without any additional computation. This packages methodology for preforming this is to take the x and y position of each pixel, the hash of a seed, and the hash of that hash, and combine them into the argument of the pcg4d_hash method. The result of this hash is then normalized against the maximum value of uint and used as the color of the pixel in the output texture. This results in a 4d random noise texture. Each of the rgba channels can be used for   

# Value noies texture
This is a lattice based algorithm that generates noise textures with smoother transitions in local areas (also called a coherent noise texture). This algorithm functions by first generating a small random noise texture. This texture defines the colors of the lattice points in the value noise texture. For each pixel in the value noise texture then interpolates between the colors of each of the 4 lattice points surrounding it. The size of each lattice cell can be modified to smooth the random transitions out over differently sized regions. With a lattice size of 1 this method is equivalent to a random noise texture.    
# Perlin noise texture
[perlin noise](https://en.wikipedia.org/wiki/Perlin_noise)