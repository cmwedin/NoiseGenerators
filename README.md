# Noise Generators
This package is a collection of compute shaders that can generate various types of noise textures. This README is currently a placeholder and will be update as development continues

# Using This Package
There are multiple supported ways to use this package to generate a random noise texture. The first step is to determine what type of noise texture you want to generate. The different types of noise textures supported are discussed [bellow](#random-noise-texture). Once you have determined the appropriate noise texture for your purpose, you can use one of the following methods to create it

## Using a generator object
This method is recommended for creating a noise texture from a script. You can instantiate the generator object for the type of texture you want to generate using its constructor. You will provide arguments to the constructor to set parameters of the texture such as the random number seed and the texture size. Some types of textures may also require other parameters such as lattice cell size or number of control points. You can also modify other parameters of the texture such as wether it will be required to tile seamlessly through the properties of the generator object. Once you have the parameters set appropriately you can generate the texture using the `GenerateTexture()` method and access it from the `NoiseTexture` property. If you will not need to regenerate the texture you can at this point dispose of noise generator object using its `Dispose()` method, which will release any unmanaged resources the object uses (such as its compute shader buffers). Not that this will not release the unmanaged resources of the texture itself, which you will need to do yourself once done using it through the `RenderTexture.Release()` method. 

## Using a monobehaviour component
If would prefer to generate noise texture using monobehaviour components this is supported as well. To do so attach the component appropriate for the type of noise you want to generate to a game object. You can modify the parameters for the generated textures using the editor fields of this component. This parameters can be modified through code as well; however, because properties cannot be serialized for modification in the unity editor, when setting the properties in the editor you are modifying the backing field directly, and when modifying them from code you are using the public properties (this distinction isn't especially relevant as all of these parameters, regardless of where they where modified from, will eventually be passed into the generator object's public property fields, where the values will be validated). Once the texture is generated the component will create the appropriate generator object with the parameters provided, generate its texture, then dispose of it. This texture can be accessed through the `NoiseTexture` property, which will invoke the `GenerateTexture()` method itself if the backing noise texture has not been set yet. Once done with the texture you can invoke the components `Dispose()` method which will release the resources of the noise texture and set it back to null, as well as releasing the resources of the generator object as a precautionary measure, although it should have already been disposed of.
## Using a static method
Rather than instantiating an object to generate a noise texture we also support using a static method to generate the texture instead. The procedure for using this approach is very similar to other methods. Each noise generator class has a static method that will generate the appropriate type of texture an return it to the caller. The parameters for the texture are all included in the arguments of these methods, with some being optional (optional arguments are best left to their default values unless you specifically want to change them). Under the hood these methods are equivalent to using an object, as they simply create the appropriate generator object, use it to generate the texture, then dispose it. These methods are primarily providing as a quality of life method to avoid needing to instantiate a generator object every time you want to create a texture. 

## Creating the texture as an asset using editor tools
The editor tools supporting this method of generating noise texture are planed to be implemented in version 1.1.0

# Pseudo-Random Number generation approach

This package uses a "permuted congruential generator" (more commonly known as PCG) algorithm  as it lies on the pareto-fronter of random quality vs performance (see [Hash Functions for GPU Rendering](https://jcgt.org/published/0009/03/02/)). The particular PCG procedure this package uses is (as implemented in HLSL): 

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


# Random Noise Texture
The is the simplest approach to generating a random noise texture in that it simply uses random values for the color of each pixel without any additional computation. This packages methodology for preforming this is to take the x and y position of each pixel, the hash of a seed, and the hash of that hash, and combine them into the argument of the pcg4d_hash method. The result of this hash is then normalized against the maximum value of uint and used as the color of the pixel in the output texture. This results in a 4d random noise texture. Each of the rgba channels can be used for   


# Value Noises Texture
This is a lattice based algorithm that generates noise textures with smoother transitions in local areas (also called a coherent noise texture). This algorithm functions by first generating a small random colors on each point of a lattice. The size of each cell in the lattice can be modified; however, to ensure tiling the size of a individual lattice cell must be a factor of the size of the total texture (so that an integer number of cells can be fit in the entire texture). Each pixel in the value noise texture then interpolates between the colors of each of the 4 lattice points surrounding it. The values being interpolated between are the same within each cell, the only difference being the "t" value of the interpolation. This results in a blocky final texture which is the primary drawback of this noise generation methods, although this can be alleviated by passing the final texture into a fractal noise generator, which require the value noise generator to be set to tile.     

# Perlin Noise Texture
This is a similar lattice based algorithm that generates coherent noise texture that have a smoother, less blocky appearance than value noise textures. It again first generates a smaller random noise texture that serves as a the base for the random values use on the lattice. The size of each cell in this lattice must be a factor of the total texture size. If it is not, the value will be adjusted automatically to the closest factor. The rgba values of each pixel on this image are use to rotate a vector by a random radian between 0 and 2pi. Then for each pixel on the canvas a color is associated with each point on the lattice by taking the dot product of the offest vector from that point to the pixel and each of the four vectors generated for that lattice point. These colors are then interpolated to create the final color for the pixel. The texture can be tiled seamlessly if using the "tile texture" setting, which is needed if using the texture as a base for fractal noise discussed below. 

# Worley Noise Texture
This type of noise texture, also called Cellular or Voronoi noise, works by scattering points randomly across the texture and setting the values of each pixel based on how far it is from the closest point. The closer the pixel is to a control point the darker it will be, although this can be inverted using the "invert texture" setting. The texture values are then normalized between the maximum and minimum values generated by the algorithm to ensure the full range of 0 to 1 us used.  For the implementation in this project the scattering of the control points isn't purely random but rather the texture is divided into cells with one point placed in each cell. This is to optimize the calculation of the closest point as only the points in adjacent cells need to be checked. Due to poor quality texture when the number of control points is set to a prime number, the number of points cannot be set directly. Instead the number of divisions within which to place a point can be set along each axis. For example, if the x axis is set to be divided into 10 cells, and the y axis into 5, the resulting texture will be generated from 50 control points. The channel the generated noise is stored in can be adjusted by changing the active channel enum. If it is set to all then a separate noise texture will be stored in each channel of the final texture.  
Optionally, the texture generator can be required to generate a texture that will tile seamlessly. When this option is set to true the generator will along the edges wrap the point on the other side of the texture so that it is adjacent to the cell of the pixel the value is being calculated for, and that point will be included in the determination of the closest point. Effectively, surround the texture with a copy of the collection of points along all sides so that the generated texture can be tiled seamlessly.  

# Fractal Noise
Rather than being a method of generating noise in its own right fractal noise is a method to add detail to an existing noise texture by adding onto itself with increasing frequency and decreasing amplitude. In order for the fractal noise generator component to work a separate noise generator is needed to create the texture id will add detail too. For best results it is important that the underlying texture detail is added to be able to tile seamlessly, otherwise there will be visible seems on the result of the fractal noise generator. 
There are a variety of parameters that can be modified to tweak how detail is added. The octaves setting will affect how many times detail will be added onto the texture, note that as with each iteration the amplitude of the added noise is decreased the final texture will typically converge as the number of octaves is increased. 
The frequency and amplitude parameters will modify the initial settings of the generator. At high frequencies the value of a given pixel will be affected by the value of pixels far away from it, and at low frequencies by pixels nearby it. Amplitude affects how much each octave contributes to the final result, as each value added to the final value for a given pixel is multiplied by the amplitude for that octave. Note that by default the affects of amplitude are normalized out of the final texture (this is due to the restriction on the range of values that can be represented using colors, all values outside the 0-1 range are identical on the final texture), meaning that changing the amplitude visually doesn't affect the final result. This can be disabled through the "normalize amplitude" setting, although this is only recommended for very small amplitudes due to the aforementioned restrictions on the range of values that can be represented by a color. 
Lacunarity and gain (also called persistance) affect how quickly the values of frequency and amplitude change with each octave. Once the value on octave will contribute to the final texture is calculated the frequency is multiplied by the lacunarity to determine the frequency for the next octave, and the amplitude by the gain. The generally accepted values of these parameters to generate the best results are 2 and .5 respectively, or if nonstandard values are use that gain be the inverse of lacunarity and vice versa.     