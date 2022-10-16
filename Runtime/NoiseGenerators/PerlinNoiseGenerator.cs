using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class PerlinNoiseGenerator : AbstractLatticeNoiseGenerator
    {
        public PerlinNoiseGenerator(
            uint _texWidth,
            uint _texHeight,
            uint _seed,
            Vector2Int _latticeCellSize,
            bool _allowPartialCells = false
        ) : base(_texWidth, _texHeight, _seed, _latticeCellSize, _allowPartialCells) {
        }

        protected override int LatticeBufferStride => 8 * sizeof(float);

        protected override string ComputeShaderPath => "Compute/PerlinNoise";

        public override void GenerateTexture()
        {
            SetShaderParameters();
            NoiseGenShader.Dispatch(generateLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            if(RequireSeamlessTiling) {
                NoiseGenShader.Dispatch(wrapLatticeKernel, latticeThreadGroupCount.x, latticeThreadGroupCount.y, latticeThreadGroupCount.z);
            }
            NoiseGenShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
        }
    }
}