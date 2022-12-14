
uint pcg_hash(uint seed) {
    seed = seed * 747796405u + 2891336453u;
    seed = ((seed >> ((seed >> 28u) + 4u)) ^ seed) * 277803737u;
    return (seed >> 22u) ^ seed;
}

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
