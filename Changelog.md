# Changelog
## 1.0.2 - in development
- Added support for multiple textures as input to a fractal noise generator as well as input textures which are not RenderTextures
  - A new input texture will be used for each octave of noise if possible.
    - if fewer input textures than the number of octaves are provided, the final input texture will be reused for all the subsequent octaves. 
  - Methods have been added to set a list of textures as the input for a fractal noise generator, as well as methods to add individual textures as additional inputs
  - An additional constructor for FractalNoiseGenerators has been added using a list of textures as an input.
  - An additional constructor for FractalNoiseGenerators has been added that only sets initial texture width and height, without an input initialized
  - Controls have been added to prevent input textures from having a different size than each other or the final noise texture
    - If texture size is changed after construction the current input will be discarded
  - Controls have been added to prevent a fractal noise generator from running with inputs set unless its constructor used an additional noise generator object object for generating textures
  - Due to the increasing number of ways to use the fractal noise generator object (with a separate generator to create its input, with a single input texture, with multiple input textures) support for using fractal noise generators without pre-generating an input texture will be sunset in 1.1.0
    - If you use fractal noise generators that don't use pre-generated textures, you will need to migrate to ones that do before 1.1.0 (this should be after one more minor update, 1.0.3)
    - You can do this by replacing the additional noise generator object you where using in the fractal noise generator's constructor with a list of textures, or an individual texture, generated separately.
    - Alternatively you could replace the generator object with the size of the final noise texture and set the inputs post-construction
- Added the ability to use Texture2D assets as input to a fractal noise generator component
  - Unlike with the underlying generator objects, fractal noise components will maintain support for using a separate component to generate the input texture
    - this is due to the greater control its inspector gives over how it is used and the more predicable lifecycle bounds
  - Currently all texture assets must be the same size
- added various features to the inspector for fractal generator components
  - added the texture preview feature introduced to other generator components in 1.0.1
  - added a slider to set the desired number of input textures to use, up to the number of octaves
  - added the ability to switch between using a separate generator component and Texture2D assets
    - added a field to add texture assets as input if that mode is selected 
      
## 1.0.1 - 11/5/22
### Noise Generator Components
- Added `TextureGenerated` bool to indicate if the component currently has a generated texture
- Modified `NoiseTexture` property to return null if `TextureGenerated` is false
- fixed a bug that caused a component to be marked as not disposed even when its generator object had not been created
  - occurred if an error prevented the previous invocation of `Dispose` from executing fully
- Added a preview of the texture created by the generator to its inspector
- Removed noiseTexture field as it is redundant with the NoiseTexture property and the previous change removes the need to have a serializable version of the texture to be displayed by the editor 