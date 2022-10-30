# Noise Generators
This package is a collection of compute shaders that can generate various types of noise textures.  It supports generating Perlin noise, Worley noise (aka Voroni or cellular noise), value noise, and pure random noise. The generators are implemented in HLSL compute shaders allowing the textures to quickly be created (the speed varies based on the type of texture but is generally under 1ms). Full documentation of the included classes is available [here](https://cmwedin.github.io/NoiseGeneratorsDocumentation/namespaces.html)

## Installation
### Through GitHub (Recommended)
#### Stable Installation
To install this package in your Unity project, select the "window/Package Manager" entry in the Unity Inspector toolbar. Then, select the "+" icon in the upper left corner of the opened window, and select "Add package from git url." Paste the following:

    https://github.com/cmwedin/NoiseGenerators.git

Once you see this package show up in the Package Manager window, it has been successfully installed. 

You can automatically update the package by clicking the "update," button in the bottom right corner of the package manager window. This will retrieve any changes on main branch on this repository since installation or your last update.

#### Bleeding-Edge Installation
You can install the experimental version of this package instead by adding #Development-Branch to the end of the Github url above. This will make your installation use the most up to date version possible, even before changes are merged onto the main branch. 

### Through Itch.io
When installing this package through Itch.io you will need to update the package manually. There are two methods to download the package through Itch, you can either install it with the rest of your packages, leaving your asset folder less cluttered, but preventing modifications to the packages files; or you can install it as an asset, placing all of the scripts within your asset folder and allowing you to modify them. Modifications to this packages scripts are recommended only for user very confident in what they are doing.   

#### Installing as a Package
Download this package's archive from its <a href="https://sadsapphic.itch.io/noise-generators" target="_blank">Itch.io page</a>. Once downloaded, extract the "NoiseGenerators" folder contained within to your desired installation location. Note that deleting this folder will break your package installation, even after adding the package to a Unity project.

After you have downloaded and unzipped the package, open the Unity project you wish to add this package too. Open the package manager window, and select the "+" icon in the upper left corner. Then, select the "Add package from disk" option. Navigate to your installation location and select the "package.json" file. Once you see this package show up in the Package Manager window, it has been successfully installed.

#### Installing as an Asset
Download the .unitypackage file from this packages <a href="https://sadsapphic.itch.io/noise-generators" target="_blank">Itch.io page</a>. Once downloaded, open the Unity project you wish to add this package to. Select "Assets" from the Unity Editor's toolbar, an  from the "Import Package" menu select "Custom Package". In the window that pops up navigate to the .unitypackage file you downloaded and select it. The package will be added to your assets folder in the "/Packages/CommandPattern/" directory. 

### Updating the Package
If you installed the package through GitHub, you can automatically update it to its newest version by clicking the "update" button in the bottom right corner of its entry in the package manager window. If not, you will need to update it manually. In addition, there may be steps needed to migrate your code to the newer version of the package. If there are, these will be added to this section when the update has been released.
#### Manual Updating as a package
If you installed the package using the local package installation through the Unity package manager to update it you will need to download the archive for the updated version from the packages Itch.io page. Once you've done this, remove the old package in the package manager, delete the old installation and extract the inner folder from the new archive. If you plan to install the updated package with the same path, you may be able to skip removing the old package, but it is recommended you do so regardless. Once you have completed this, add the updated version of the package through the same installation progress you followed above.
#### Manual Updating as an Asset
If you installed the package as an asset using the .unitypackage file, simply download the updated .unitypackage file, delete you old installation in your assets folder, and follow the installation process above using the new .unitypackage file.  
### Installing the Package Demo
This package includes a demo showcasing the various types of noise textures that are implemented and allowing you to experiment with different parameters for those textures. Not that the texture display can sometime lag behind the generate texture, control click on the noise texture field of the generator component being displayed to force it to update. If you installed the package as an asset the demo will be included by default within the "Demos/NoiseDemo" directory. If installed as a package, this demo can be added by opening the "Samples" dropdown in its entry in the package manager and clicking the "import" button next to the "Noise Demo" entry. Once this demo has been imported, you can open it by navigating to the newly created "Samples/NoiseGenerators/[Current Version]/NoiseDemo" folder and opening the "NoiseDemoScene.Unity" scene.

# Using This Package
There are multiple supported ways to use this package to generate a random noise texture. The first step is to determine what type of noise texture you want to generate. The different types of noise textures supported are discussed [below](#random-noise-texture). Once you have determined the appropriate noise texture for your purpose, you can use one of the following methods to create it

## Using a generator object
This method is recommended for creating a noise texture from a script. You can instantiate the generator object for the type of texture you want to generate using its constructor. You will provide arguments to the constructor to set parameters of the texture such as the random number seed and the texture size. Some types of textures may also require other parameters such as lattice cell size or number of control points. You can also modify other parameters of the texture such as wether it will be required to tile seamlessly through the properties of the generator object. Once you have the parameters set appropriately you can generate the texture using the `GenerateTexture()` method and access it from the `NoiseTexture` property. If you will not need to regenerate the texture you can at this point dispose of noise generator object using its `Dispose()` method, which will release any unmanaged resources the object uses (such as its compute shader buffers). Note that this will not release the unmanaged resources of the texture itself, which you will need to do yourself once done using it through the `RenderTexture.Release()` method. For more details as to why this is see [implementing a disposal method](#implementing-a-disposal-method) 

## Using a monobehaviour component
If would prefer to generate noise texture using monobehaviour components this is supported as well. To do so attach the component appropriate for the type of noise you want to generate to a game object. You can modify the parameters for the generated textures using the editor fields of this component. This parameters can be modified through code as well; however, because properties cannot be serialized for modification in the unity editor, when setting the properties in the editor you are modifying the backing field directly, and when modifying them from code you are using the public properties (this distinction isn't especially relevant as all of these parameters, regardless of where they where modified from, will eventually be passed into the generator object's public property fields, where the values will be validated). Once the texture is generated the component will create the appropriate generator object with the parameters provided, generate its texture, then dispose of it. This texture can be accessed through the `NoiseTexture` property, which will invoke the `GenerateTexture()` method itself if the backing noise texture has not been set yet. Once done with the texture you can invoke the components `Dispose()` method which will release the resources of the noise texture and set it back to null, as well as releasing the resources of the generator object as a precautionary measure, although it should have already been disposed of.
## Using a static method
Rather than instantiating an object to generate a noise texture we also support using a static method to generate the texture instead. The procedure for using this approach is very similar to other methods. Each noise generator class has a static method that will generate the appropriate type of texture an return it to the caller. The parameters for the texture are all included in the arguments of these methods, with some being optional (optional arguments are best left to their default values unless you specifically want to change them). Under the hood these methods are equivalent to using an object, as they simply create the appropriate generator object, use it to generate the texture, then dispose it. These methods are primarily providing as a quality of life method to avoid needing to instantiate a generator object every time you want to create a texture. 

## Creating the texture as an asset using editor tools
The editor tools supporting this method of generating noise texture are planed to be implemented in version 1.1.0

## Displaying the Noise Textures
While experimenting with different types of textures and texture parameters you may find it helpful to display the generated texture in the scene to better see the effect. Using [noise generator components](#using-a-monobehaviour-component) is recommended for this purpose. While you can see the generated texture by control clicking on the "Noise Texture" field in the editor, it can be difficult to distinguish changes after modifying the parameters, regenerating the texture, and re-opening the preview window. To alleviate this, we provide a "NoiseTextureDisplay" component that can show the texture in the scene and update the display whenever its regenerated. To use this, simply add the `NoiseTextureDisplay` component to an empty game object, the component will handle setting up the other components needed to display the texture for you (although for completeness it is recommended you set the mesh to the default quad and the material to an unlit texture), and resize the game object as desired. Once the size and position of the object is set up appropriately, add the generator component you wish to display the texture of to the `Texture Generator` editor field of the `NoiseTextureDisplay` component. Then, whenever the texture of that generator component is recreated, the object displaying it in the scene will be updated accordingly         

# Package Implementation Details
## Pseudo-Random Number generation approach

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


## Random Noise Texture
The is the simplest approach to generating a random noise texture in that it simply uses random values for the color of each pixel without any additional computation. This packages methodology for preforming this is to take the x and y position of each pixel, the hash of a seed, and the hash of that hash, and combine them into the argument of the pcg4d_hash method. The result of this hash is then normalized against the maximum value of uint and used as the color of the pixel in the output texture. This results in a 4d random noise texture. Each of the rgba channels can be used for   


## Value Noises Texture
This is a lattice based algorithm that generates noise textures with smoother transitions in local areas (also called a coherent noise texture). This algorithm functions by first generating a small random colors on each point of a lattice. The size of each cell in the lattice can be modified; however, to ensure tiling the size of a individual lattice cell must be a factor of the size of the total texture (so that an integer number of cells can be fit in the entire texture). Each pixel in the value noise texture then interpolates between the colors of each of the 4 lattice points surrounding it. The values being interpolated between are the same within each cell, the only difference being the "t" value of the interpolation. This results in a blocky final texture which is the primary drawback of this noise generation methods, although this can be alleviated by passing the final texture into a fractal noise generator, which require the value noise generator to be set to tile.     

## Perlin Noise Texture
This is a similar lattice based algorithm that generates coherent noise texture that have a smoother, less blocky appearance than value noise textures. It again first generates a smaller random noise texture that serves as a the base for the random values use on the lattice. The size of each cell in this lattice must be a factor of the total texture size. If it is not, the value will be adjusted automatically to the closest factor. The rgba values of each pixel on this image are use to rotate a vector by a random radian between 0 and 2pi. Then for each pixel on the canvas a color is associated with each point on the lattice by taking the dot product of the offset vector from that point to the pixel and each of the four vectors generated for that lattice point. These colors are then interpolated to create the final color for the pixel. The texture can be tiled seamlessly if using the "tile texture" setting, which is needed if using the texture as a base for fractal noise discussed below. 

## Worley Noise Texture
This type of noise texture, also called Cellular or Voronoi noise, works by scattering points randomly across the texture and setting the values of each pixel based on how far it is from the closest point. The closer the pixel is to a control point the darker it will be, although this can be inverted using the "invert texture" setting. The texture values are then normalized between the maximum and minimum values generated by the algorithm to ensure the full range of 0 to 1 us used.  For the implementation in this project the scattering of the control points isn't purely random but rather the texture is divided into cells with one point placed in each cell. This is to optimize the calculation of the closest point as only the points in adjacent cells need to be checked. Due to poor quality texture when the number of control points is set to a prime number, the number of points cannot be set directly. Instead the number of divisions within which to place a point can be set along each axis. For example, if the x axis is set to be divided into 10 cells, and the y axis into 5, the resulting texture will be generated from 50 control points. The channel the generated noise is stored in can be adjusted by changing the active channel enum. If it is set to all then a separate noise texture will be stored in each channel of the final texture.  
Optionally, the texture generator can be required to generate a texture that will tile seamlessly. When this option is set to true the generator will along the edges wrap the point on the other side of the texture so that it is adjacent to the cell of the pixel the value is being calculated for, and that point will be included in the determination of the closest point. Effectively, surround the texture with a copy of the collection of points along all sides so that the generated texture can be tiled seamlessly.  

## Fractal Noise
Rather than being a method of generating noise in its own right fractal noise is a method to add detail to an existing noise texture by adding onto itself with increasing frequency and decreasing amplitude. In order for the fractal noise generator component to work a separate noise generator is needed to create the texture id will add detail too. For best results it is important that the underlying texture detail is added to be able to tile seamlessly, otherwise there will be visible seems on the result of the fractal noise generator. 
There are a variety of parameters that can be modified to tweak how detail is added. The octaves setting will affect how many times detail will be added onto the texture, note that as with each iteration the amplitude of the added noise is decreased the final texture will typically converge as the number of octaves is increased. 
The frequency and amplitude parameters will modify the initial settings of the generator. At high frequencies the value of a given pixel will be affected by the value of pixels far away from it, and at low frequencies by pixels nearby it. Amplitude affects how much each octave contributes to the final result, as each value added to the final value for a given pixel is multiplied by the amplitude for that octave. Note that by default the affects of amplitude are normalized out of the final texture (this is due to the restriction on the range of values that can be represented using colors, all values outside the 0-1 range are identical on the final texture), meaning that changing the amplitude visually doesn't affect the final result. This can be disabled through the "normalize amplitude" setting, although this is only recommended for very small amplitudes due to the aforementioned restrictions on the range of values that can be represented by a color. 
Lacunarity and gain (also called persistance) affect how quickly the values of frequency and amplitude change with each octave. Once the value on octave will contribute to the final texture is calculated the frequency is multiplied by the lacunarity to determine the frequency for the next octave, and the amplitude by the gain. The generally accepted values of these parameters to generate the best results are 2 and .5 respectively, or if nonstandard values are use that gain be the inverse of lacunarity and vice versa.

# Extension Instructions
While this package tries to provide as many types of noise textures as possible, you may find yourself wanting to build on them further to or even create custom generators of your own. This will require you to have some familiarity with compute shaders, but is supported by the package. The instructions below will explain how to extend the class provided in this package to accomplish that.

## Creating your own noise generator ComputeShader
This first step to implementing your own noise generators is to create its compute shader. While this section will not provide basic tutorials on how to use hlsl, it will provide information for how to implement your compute shader in a manner that is compatible with the expectations of the [AbstractNoiseGenerator](Runtime/NoiseGenerators/AbstractNoiseGenerator.cs) class. 
First, your compute shader must be located in a folder named "Resources," as this is needed for the dispatcher class to be able to reference it. In addition, your compute shader must include at minimum the following properties, named as listed:
- `RWTexture2D<float4> _NoiseTexture;`
- `uint _Seed;`
- `uint _TexWidth;`
- `uint _TexHeight`

The kernel to generate the texture is also recommended to be named `CSMain`. If you don't name it that be sure to override the AbstractNoiseGenerators `GenerateTextureKernel` to find whatever you changed the name of the kernel too. Any additional properties or kernel needed for your purpose can be implemented and named as you wish; however, the AbstractNoiseGenerator base class will be expecting at a minimum the compute shader to comply with the above.

It is also recommended to add the following line at the top of your compute shader

    #include "Packages/com.sadsapphicgames.noisegenerators/Runtime/Resources/Compute/Includes/PRNG.cginc"
as this will allow use to use the pseudo-random number generation methods discussed [above](#pseudo-random-number-generation-approach). You do not need to include this but if you don't you will need to implement your own method to hash the seed of the shader. For convenience the following is a template you can build off of when implementing a compute shader to extend the functionality of this package:

    #pragma kernel CSMain
    #include "Packages/com.sadsapphicgames.noisegenerators/Runtime/Resources/Compute/Includes/PRNG.cginc"
    #define UINTMAXVALUE 4294967295.0 

    RWTexture2D<float4> _NoiseTexture;
    uint _Seed; //? the value used to generate the random hash
    uint _TexWidth; //? the width of the generated texture in pixels
    uint _TexHeight; //? the height of the generated texture in pixels

    [numthreads(8,8,1)]
    void CSMain (uint3 id : SV_DispatchThreadID)
    {
        if(id.x < 0 || id.x >= _TexWidth || id.y < 0 || id.y >= _TexHeight) {return;}

    }
The definition for `UINTMAXVALUE` is not needed but can be useful for converting the hash of a seed into a random float between 0 and 1. In addition you can change the number of threads if you wish; however, if you do be sure to override the `ThreadGroupSize` of the base AbstractNoiseGenerator within you C# class to dispatch this shader, which brings us to what you should be aware of when implementing this dispatcher class. 

## Implementing your compute shaders dispatcher
### Mandatory overrides
When implementing the dispatcher for you compute shader first create a class that inherits from the `AbstractNoiseGenerator` class. There are three mandatory features of this class that you must implement:
- The `ComputeShaderPath` property
- The `InnerGenerateTexture` method
- A constructor of the base class of the from `base(uint _texWidth, uint _texHeight, uint _seed)`

The constructor of your class can differ from the constructor of the base abstract generator, however it must invoke the base constructor of the form listed above. The `ComputeShaderPath` is the simplest to implement as it is simple a string of the path to the compute shader you which to dispatch from the resource folder, excluding the .compute extension (this is to say that if the full path of your compute shader is "Assets/Resources/Compute/MyComputeShader.compute" your implementation of this property should be `protected override string ComputeShaderPath => "Compute/MyComputeShader"`). The InnerGenerateTexture() is where you will dispatch the kernel to generate the texture and any other kernels that need to be run for the generator to function properly. This function is wrapped by the following public function, AbstractNoiseGenerator.GenerateTexture:

    public virtual void GenerateTexture() {
        SetShaderParameters();
        ResetNoiseTexture();
        InnerGenerateTexture();
        OnTextureGeneration?.Invoke();
    }
In other words, you don't need to worry about setting the shader parameters within `InnerGenerateTexture`, just dispatching the relevant kernels. If your compute shader only uses the CSMain (or your shaders equivalent) kernel to generate its texture fully than your implementation of `InnerGenerateTexture` could be as simple as the following:
    
    protected override void InnerGenerateTexture() {
        NoiseGenShader.Dispatch(GenerateTextureKernel,texThreadGroupCount.x,texThreadGroupCount.y,texThreadGroupCount.z);
    }

### Optional overrides
While these overrides are not required to implement the AbstractNoiseGenerator class they can be just as important as the mandatory overrides. As mentioned above, if you change the name of the kernel to generate the noise texture in your compute shader, you will need to override the `GenerateTextureKernel` property to find whatever kernel you changed the name to, as its default implementation will assume the name of this kernel is `CSMain`. Additionally if you changes the number of thread's in a thread group you will need to override the `ThreadGroupSize` property as its default implementation will assume a thread group size of `[numthreads(8,8,1)]`. The package supports overriding the public `GenerateTexture` method; however, it is not recommended that you do this. Instead place the necessary code with overrides for `InnerGenerateTexture` and `SetShaderParameters`.

`SetShaderParameters` is the most important of the overrides that are not explicitly required by implementers of AbstractNoiseGenerators. The base class will handle setting the mandator properties of the compute shader discussed [above](#creating-your-own-noise-generator-computeshader)(thus it is highly recommend you start your override by invoking base.SetShaderParameters), but if you have any properties beyond those mandatory properties you will need to set them here.

### Implementing a disposal method
All AbstractTextureGenerator implement the IDisposable interface. If your generator uses any compute buffers, you will need to implement an override to the `protected virtual void Dispose(bool disposing)` method to dispose of those buffers. The general pattern for implementing this will be something like 

    private bool disposedValue = false;
    protected override void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                myBuffer?.Release(); //? equivalent to myBuffer.Dispose();
                myBuffer = null; 
            }
            base.Dispose(disposing);
            disposedValue = true;
        }
    }

Not that even though RenderTextures have unmanaged resources, disposing a generator object does not dispose of its generated texture. This is for two reasons
- It allows the generating object to be disposed of as soon as the texture is generated will still allowing the generated texture to be used
- The above is essential for supporting static texture generation methods as the object generating the texture returned by those methods must be disposed of within the static methods scope (meaning if textures where disposed of with their generating object the texture would have already been disposed when it was returned by the static method)

Therefore we assume that the consumers of noise texture generated by these objects will use them responsibly and dispose of them when they are done. If this proves not to be the case support for static generation methods will be removed and noise textures will be disposed of with their generating object.    
### Implementing a Static texture generation method
This is not required of classes implementing AbstractNoiseGenerator, but if you wish to use the generator you implemented through a static method you will need to implement one yourself. The general pattern for implementing this method is for it to take all possible texture parameters as an argument (some of these may be optional), and construct an instance of the generator class the method belongs to using those parameters. That instance will then be used to generate a texture, disposed of, and the texture returned to the caller. 

## Implementing a generator component
If you don't plan to use a MonoBehaviour component to generate your noise texture this is entirely optional; however if you do you will need to create another class in addition to your extension of `AbstractNoiseGenerator` which extends `AbstractNoiseGeneratorComponent`. 

The only mandatory requirement to implement this class is to implement the method `CreateNoiseGenerator` method, which will be used to construct your custom noise generator using the parameters set in the editor fields of the component (the base class implements the tex width, tex height, and seed fields, your extension will need to implement any additional parameters added by your custom generator). It is important to not the generator will only be constructed once and reused until the components `Dispose()` method is invoked (unlike with the generator object this will also dispose of the noise texture), which is automatically invoked in the components OnDisable method. 

While it is not explicitly required, similar to overriding `SetShaderParameters` in your custom generator object you will also need to override the `UpdateGeneratorSettings` method to pass any changes properties from the editor fields onto the inner generator object, otherwise these parameters will not be changed after the generator object is constructed.