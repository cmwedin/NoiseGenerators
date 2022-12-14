using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    /// <summary>
    /// A generator object used to create a value noise texture
    /// </summary>
    public class ValueNoiseGenerator : AbstractLatticeNoiseGenerator
    {
        /// <summary>
        /// Constructs a ValueNoiseGenerator
        /// </summary>
        /// <param name="_texWidth">The width of the generated texture</param>
        /// <param name="_texHeight">The height of the generated texture</param>
        /// <param name="_seed">The seed for the pseudo-random number generation</param>
        /// <param name="_latticeCellSize">The size in pixels of a single cell in the lattice </param>
        /// <param name="_allowPartialCells">If the texture should be allowed to cut off lattice cells along the edge, defaults to false</param>
        public ValueNoiseGenerator(
            uint _texWidth,
            uint _texHeight,
            uint _seed,
            Vector2Int _latticeCellSize,
            bool _allowPartialCells = false
        ) : base(_texWidth, _texHeight, _seed, _latticeCellSize, _allowPartialCells) {
        }

        /// <summary>
        /// The memory size of an entry in the lattice buffer (four floats)
        /// </summary>
        protected override int LatticeBufferStride => 4*sizeof(float);

        protected override string ComputeShaderPath => "Compute/ValueNoise";

        protected override void InnerGenerateTexture()
        {
            NoiseGenShader.Dispatch(generateLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            if(RequireSeamlessTiling) {
                NoiseGenShader.Dispatch(wrapLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            }
            NoiseGenShader.Dispatch(GenerateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
        }
        /// <summary>
        /// Generates a value noise texture
        /// </summary>
        /// <param name="_texWidth">the width of the texture</param>
        /// <param name="_texHeight">the height of the texture</param>
        /// <param name="_seed">the seed for the pseudo random number generator</param>
        /// <param name="_latticeCellSize">the size in pixels of a single cell in the lattice</param>
        /// <param name="_allowPartialCells">if the texture should be allowed to cut off lattice cells</param>
        /// <param name="_requireSeamlessTiling">if the texture should tile seamlessly</param>
        /// <returns>The generated texture</returns>
        public static RenderTexture GenerateTexture(
            uint _texWidth,
            uint _texHeight,
            uint _seed,
            Vector2Int _latticeCellSize,
            bool _allowPartialCells = false,
            bool _requireSeamlessTiling = true
        ) {
            if(_allowPartialCells && _requireSeamlessTiling) {
                Debug.LogWarning("Cannot both allow partial cells and require seamless tiling, setting seamless tiling to false");
                _requireSeamlessTiling = false;
            } 
            ValueNoiseGenerator generator = new ValueNoiseGenerator(_texWidth, _texHeight, _seed, _latticeCellSize, _allowPartialCells);
            generator.RequireSeamlessTiling = _requireSeamlessTiling;
            generator.GenerateTexture();
            RenderTexture output = generator.NoiseTexture.Copy();
            generator.Dispose(); //?should be redundant
            return output;
        }
    }
}