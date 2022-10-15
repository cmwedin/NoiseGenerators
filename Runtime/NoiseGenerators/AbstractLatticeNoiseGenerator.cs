using UnityEngine;
using System;

namespace SadSapphicGames.NoiseGenerators
{
    public abstract class AbstractLatticeNoiseGenerator : AbstractNoiseGenerator
    {
        protected AbstractLatticeNoiseGenerator(
            uint _texWidth,
            uint _texHeight,
            uint _seed,
            Vector2Int _latticeCellSize,
            bool _allowPartialCells = false
        ) : base(_texWidth, _texHeight, _seed) {
            AllowPartialCells = _allowPartialCells;
            LatticeCellSize = _latticeCellSize;
            latticeBuffer = new ComputeBuffer(latticeHeight * latticeWidth, LatticeBufferStride);
        }
//? Compute shader fields
    //? Kernel and thread group info
        int generateLatticeKernel { get => NoiseGenShader.FindKernel("GenerateLattice"); }
        int wrapLatticeKernel { get => NoiseGenShader.FindKernel("WrapLattice"); }
        private Vector3Int latticeThreadGroupCount
        {
            get => new Vector3Int(
                Mathf.CeilToInt(latticeWidth / (float)threadGroupSize.x),
                Mathf.CeilToInt(latticeHeight / (float)threadGroupSize.y),
                1
            );
        }

        //? Lattice Parameters
        private ComputeBuffer latticeBuffer;
        protected abstract int LatticeBufferStride { get; }
        private int latticeWidth { get => Mathf.CeilToInt((float)TexWidth / (float)LatticeCellSize.x)+1; }
        private int latticeHeight { get => Mathf.CeilToInt((float)TexHeight / (float)LatticeCellSize.y)+1; }
        public bool AllowPartialCells { get; set; }
        private Vector2Int latticeCellSize;
        public Vector2Int LatticeCellSize {
            get => latticeCellSize;
            set {
                value = new Vector2Int(Math.Abs(value.x),Math.Abs(value.y));
                if (value.x == 0) { value.x++; }
                if (value.y == 0) { value.y++; }

                if (!AllowPartialCells) {
                    if (TexWidth % value.x != 0)
                    {
                        Debug.LogWarning("Horizontal lattice size must be set to a factor of texture width, adjusting value to closest factor");
                        value = new Vector2Int(
                            HelperMethods.FindClosestFactor((int)TexWidth, value.x),
                            value.y
                        );
                    }
                    if (TexHeight % value.y != 0)
                    {
                        Debug.LogWarning("Vertical lattice size must be set to a factor of texture height, adjusting value to closest factor");
                        value = new Vector2Int(
                            value.x,
                            HelperMethods.FindClosestFactor((int)TexHeight, value.y)
                        );
                    }
                }
                latticeCellSize = value;
                if(RegenerateTextureOnParamChange) {
                    GenerateTexture();
                }
            }
        }

//? Overrides 
    //? Compute Shader Methods
        protected override void SetShaderParameters() {
            base.SetShaderParameters();
            NoiseGenShader.SetInt("_LatticeSizeX", (int)LatticeCellSize.x);
            NoiseGenShader.SetInt("_LatticeSizeY", (int)LatticeCellSize.y);
            NoiseGenShader.SetInt("_LatticeTexWidth", latticeWidth);
            NoiseGenShader.SetInt("_LatticeTexHeight", latticeHeight);
            NoiseGenShader.SetBuffer(generateLatticeKernel, "_LatticeBuffer", latticeBuffer);
            NoiseGenShader.SetBuffer(generateTextureKernel, "_LatticeBuffer", latticeBuffer);
            NoiseGenShader.SetBuffer(wrapLatticeKernel, "_LatticeBuffer", latticeBuffer);
        }

        //? IDisposable
        private bool disposedValue = false;
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                latticeBuffer?.Release();
                latticeBuffer = null;
            }
            base.Dispose(disposing);
        }
        ~AbstractLatticeNoiseGenerator() {
            Dispose(disposing: false);
        }

    }
}