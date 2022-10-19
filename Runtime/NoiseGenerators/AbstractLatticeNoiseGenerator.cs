using UnityEngine;
using System;

namespace SadSapphicGames.NoiseGenerators
{
    /// <summary>
    /// Base abstract class used for noise generators that specifically use lattice based algorithms
    /// </summary>
    public abstract class AbstractLatticeNoiseGenerator : AbstractNoiseGenerator
    {
        protected AbstractLatticeNoiseGenerator(
            uint _texWidth,
            uint _texHeight,
            uint _seed,
            Vector2Int _latticeCellSize,
            bool _allowPartialCells = false
        ) : base(_texWidth, _texHeight, _seed) {
            if(_allowPartialCells == true) {
                RequireSeamlessTiling = false;
            }
            AllowPartialCells = _allowPartialCells;
            LatticeCellSize = _latticeCellSize;
            latticeBuffer = new ComputeBuffer(latticeHeight * latticeWidth, LatticeBufferStride);
        }
//? Compute shader fields
    //? Kernel and thread group info
        /// <summary>
        /// The kernel for the method in the compute shader to generate the lattice used for creating the noise texture
        /// </summary>
        protected int generateLatticeKernel { get => NoiseGenShader.FindKernel("GenerateLattice"); }
        /// <summary>
        /// The kernel for the method in the compute shader to wrap the edges of the lattice around to each other to ensure seamless tiling
        /// </summary>
        protected int wrapLatticeKernel { get => NoiseGenShader.FindKernel("WrapLattice"); }
        /// <summary>
        /// The number of thread groups to use when generating the lattice
        /// </summary>
        protected Vector3Int latticeThreadGroupCount
        {
            get => new Vector3Int(
                Mathf.CeilToInt(latticeWidth / (float)threadGroupSize.x),
                Mathf.CeilToInt(latticeHeight / (float)threadGroupSize.y),
                1
            );
        }

        //? Lattice Parameters
        /// <summary>
        /// The compute buffer the lattice is stored in
        /// </summary>
        private ComputeBuffer latticeBuffer;
        /// <summary>
        /// The memory size of a single entry in the lattice buffer
        /// </summary>
        protected abstract int LatticeBufferStride { get; }
        /// <summary>
        /// The number of points wide the lattice is
        /// </summary>
        private int latticeWidth { get => Mathf.CeilToInt((float)TexWidth / (float)LatticeCellSize.x)+1; }
        /// <summary>
        /// The number of points tall the lattice is
        /// </summary>
        private int latticeHeight { get => Mathf.CeilToInt((float)TexHeight / (float)LatticeCellSize.y)+1; }
        /// <summary>
        /// If the texture should be allowed to cut off the lattice cells along the edges, cannot be set to true if RequireSeamlessTiling is
        /// </summary>
        public bool AllowPartialCells { 
            get => allowPartialCells; 
            set {
                if(RequireSeamlessTiling == true && value == true) {
                    Debug.LogWarning("Cannot both allow partial cells and require seamless tiling, set RequireSeamlessTiling to false first");
                    allowPartialCells = false;
                } else {
                    allowPartialCells = value;
                }
                if(RegenerateTextureOnParamChange) {
                    GenerateTexture();
                }
            } 
        }
        public override bool RequireSeamlessTiling { 
            get => base.RequireSeamlessTiling;
            set {
                if(AllowPartialCells == true && value == true) {
                    Debug.LogWarning("Cannot both allow partial cells and require seamless tiling, set AllowPartialCells to false first");
                    base.RequireSeamlessTiling = false;
                } else {
                    base.RequireSeamlessTiling = value;
                }
                if(RegenerateTextureOnParamChange) {
                    GenerateTexture();
                }
            }
        }
        private bool allowPartialCells;
        /// <summary>
        ///  The size in pixels of a single lattice cells, unless AllowPartialCells is set to true must be a factor of the texture size (the value will be adjust to the nearest factor automatically)
        /// </summary>
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
        private Vector2Int latticeCellSize;


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