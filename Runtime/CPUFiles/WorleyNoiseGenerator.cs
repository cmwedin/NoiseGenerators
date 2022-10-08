using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class WorleyNoiseGenerator : AbstractNoiseGenerator
    {
        protected override int generateTextureKernel => noiseGenShader.FindKernel("CSMain");
        private int GeneratePointsKernel => noiseGenShader.FindKernel("GeneratePoints");
        protected int NormalizeTextureKernel => noiseGenShader.FindKernel("NormalizeTexture");

        enum TextureChannel {R,G,B,A}
        [SerializeField] private TextureChannel activeChannel;
        private Vector4 channelMask {
            get => new Vector4(
                    activeChannel == TextureChannel.R ? 1 : 0, 
                    activeChannel == TextureChannel.G ? 1 : 0, 
                    activeChannel == TextureChannel.B ? 1 : 0, 
                    activeChannel == TextureChannel.A ? 1 : 0
                );
        }
        [SerializeField] private bool requireTiling;
        [SerializeField] private bool invertTexture;

        [SerializeField, Tooltip("The number of cells along the x and y axis respectively")] 
        private Vector2Int cellCounts;
        private Vector3Int PointThreadGroupCount { get => new Vector3Int(
            Mathf.CeilToInt((float)cellCounts.x/threadGroupSize.x),
            Mathf.CeilToInt((float)cellCounts.y/threadGroupSize.y),
            1
        );}
        private ComputeBuffer pointsBuffer;
        private ComputeBuffer minMaxBuffer;

        protected override void CleanUpOldTextures()
        {
            base.CleanUpOldTextures();
            pointsBuffer?.Release();
            minMaxBuffer?.Release();
        }

        protected override void SetShaderParameters()
        {
            if (cellCounts.x <= 0 || cellCounts.y <= 0) { throw new System.ArgumentException("the number of cells along each axis must be greater than 0"); }
            base.SetShaderParameters();
            noiseGenShader.SetInt("_CellXCount", cellCounts.x);
            noiseGenShader.SetInt("_CellYCount", cellCounts.y);
            noiseGenShader.SetInt("_PointCellWidth", Mathf.FloorToInt(texWidth/cellCounts.x));
            noiseGenShader.SetInt("_PointCellHeight", Mathf.FloorToInt(texHeight/cellCounts.y));
            noiseGenShader.SetVector("_ChannelMask", channelMask);
            noiseGenShader.SetBool("_Tiling", requireTiling);
            noiseGenShader.SetBool("_Invert", invertTexture);
            noiseGenShader.SetBuffer(generateTextureKernel, "_PointsBuffer", pointsBuffer);
            noiseGenShader.SetBuffer(GeneratePointsKernel, "_PointsBuffer", pointsBuffer);
            noiseGenShader.SetBuffer(generateTextureKernel, "_MinMaxBuffer", minMaxBuffer);
            noiseGenShader.SetBuffer(NormalizeTextureKernel, "_MinMaxBuffer", minMaxBuffer);
            noiseGenShader.SetTexture(NormalizeTextureKernel, "_NoiseTexture", noiseTexture);
        }

        public override void GenerateTexture()
        {
            if (cellCounts.x <= 0 || cellCounts.y <= 0) {
                Debug.LogWarning("the number of cells along each axis must be greater than 0");
                return;
            }
            CleanUpOldTextures();
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            noiseTexture.enableRandomWrite = true;
            noiseTexture.wrapMode = TextureWrapMode.Repeat;
            noiseTexture.Create();
            pointsBuffer = new ComputeBuffer(cellCounts.x*cellCounts.y, 2 * sizeof(float));
            minMaxBuffer = new ComputeBuffer(2, sizeof(uint));
            minMaxBuffer.SetData(new int[] { int.MaxValue, 0 });
            try {
                SetShaderParameters();
            } catch (System.ArgumentException ex) {
                Debug.LogWarning(ex);
                CleanUpOldTextures();
                return;
            }catch (System.Exception){
                throw;
            }
            noiseGenShader.Dispatch(GeneratePointsKernel, PointThreadGroupCount.x, PointThreadGroupCount.y, PointThreadGroupCount.z);
            noiseGenShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            noiseGenShader.Dispatch(NormalizeTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            DisplayTexture();
            pointsBuffer.Release();
            minMaxBuffer.Release();
        }

        protected override void DisplayTexture()
        {
            base.DisplayTexture();
        }
    }
}