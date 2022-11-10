# Changelog
## 1.0.2 - in development
- Added support for multiple textures as input to a fractal noise generator
  - Methods have been added to set a list of textures as the input for a fractal noise generator, as well as methods to add individual textures as additional inputs
  - A new input texture will be used for each octave of noise.
    - if fewer input textures than the number of octaves are provided, the final input texture will be reused for all the subsequent octaves. 
  - Due to the increasing number of ways to use the fractal noise generator object (with a separate generator to create its input, with a single input texture, with multiple input textures) support for using fractal noise generators without pre-generating an input texture will be sunset in 1.1.0
    - If you use fractal noise generators that don't use pre-generated textures, you will need to migrate to ones that do before 1.1.0 (this should be after one more minor update, 1.0.3)
## 1.0.1 - 11/5/22
### Noise Generator Components
- Added `TextureGenerated` bool to indicate if the component currently has a generated texture
- Modified `NoiseTexture` property to return null if `TextureGenerated` is false
- fixed a bug that caused a component to be marked as not disposed even when its generator object had not been created
  - occurred if an error prevented the previous invocation of `Dispose` from executing fully
- Added a preview of the texture created by the generator to its inspector
- Removed noiseTexture field as it is redundant with the NoiseTexture property and the previous change removes the need to have a serializable version of the texture to be displayed by the editor 