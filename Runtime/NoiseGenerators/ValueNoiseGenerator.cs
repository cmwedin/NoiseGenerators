using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class ValueNoiseGenerator : AbstractLatticeNoiseGenerator
    {
        public ValueNoiseGenerator(
            uint _texWidth,
            uint _texHeight,
            uint _seed,
            Vector2Int _latticeCellSize,
            bool _allowPartialCells = false
        ) : base(_texWidth, _texHeight, _seed, _latticeCellSize, _allowPartialCells) {
        }

        protected override int LatticeBufferStride => 4*sizeof(float);

        protected override string ComputeShaderPath => "Compute/ValueNoise";

        public override void GenerateTexture()
        {
            NoiseGenShader.Dispatch(generateLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            if(RequireSeamlessTiling) {
                NoiseGenShader.Dispatch(wrapLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            }
            NoiseGenShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
        }
    }
}