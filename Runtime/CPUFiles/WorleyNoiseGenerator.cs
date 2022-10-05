using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SadSapphicGames.NoiseGenerators
{
    public class WorleyNoiseGenerator : AbstractNoiseGenerator
    {
        protected override int generateTextureKernel => noiseGenShader.FindKernel("CSMain");
        private int GeneratePointsKernel => noiseGenShader.FindKernel("GeneratePoints");
        [SerializeField] private int pointCount;
        private Vector2Int cellCounts;
        private Vector3Int PointThreadGroupCount { get => new Vector3Int(
            Mathf.CeilToInt((float)cellCounts.x/threadGroupSize.x),
            Mathf.CeilToInt((float)cellCounts.y/threadGroupSize.y),
            1
        );}
        private ComputeBuffer pointsBuffer;

        protected override void CleanUpOldTextures()
        {
            base.CleanUpOldTextures();
            pointsBuffer?.Release();
        }

        protected override void SetShaderParameters()
        {
            base.SetShaderParameters();
            noiseGenShader.SetInt("_PointCount", pointCount);
            noiseGenShader.SetInt("_CellXCount", cellCounts.x);
            noiseGenShader.SetInt("_CellYCount", cellCounts.y);
            noiseGenShader.SetInt("_PointCellWidth", Mathf.FloorToInt(texWidth/cellCounts.x));
            noiseGenShader.SetInt("_PointCellHeight", Mathf.FloorToInt(texHeight/cellCounts.y));
            noiseGenShader.SetBuffer(generateTextureKernel, "_PointsBuffer", pointsBuffer);
            noiseGenShader.SetBuffer(GeneratePointsKernel, "_PointsBuffer", pointsBuffer);
        }
        public override void GenerateTexture()
        {
            CleanUpOldTextures();
            cellCounts = HelperMethods.PartitionTexture((int)texWidth, (int)texHeight, pointCount);
            noiseTexture = new RenderTexture((int)texWidth, (int)texHeight, 24);
            noiseTexture.enableRandomWrite = true;
            noiseTexture.Create();
            pointsBuffer = new ComputeBuffer(pointCount, 2 * sizeof(float));
            SetShaderParameters();
            noiseGenShader.Dispatch(GeneratePointsKernel, PointThreadGroupCount.x, PointThreadGroupCount.y, PointThreadGroupCount.z);
            noiseGenShader.Dispatch(generateTextureKernel, texThreadGroupCount.x, texThreadGroupCount.y, texThreadGroupCount.z);
            DisplayTexture();
            pointsBuffer.Release();
        }
    }
}