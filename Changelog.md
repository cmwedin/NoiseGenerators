# Changelog
## 1.0.0 - 11/5/22
### Noise Generator Components
- Added `TextureGenerated` bool to indicate if the component currently has a generated texture
- Modified `NoiseTexture` property to return null if `TextureGenerated` is false
- fixed a bug that caused a component to be marked as not disposed even when its generator object had not been created
  - occurred if an error prevented the previous invocation of `Dispose` from executing fully
- Added a preview of the texture created by the generator to its inspector
- Removed noiseTexture field as it is redundant with the NoiseTexture property and the previous change removes the need to have a serializable version of the texture to be displayed by the editor 