using UnityEngine;

namespace Pinwheel.Griffin
{
    [System.Serializable]
    public struct GInternalShaderSettings
    {
        [SerializeField]
        private Shader solidColorShader;
        public Shader SolidColorShader
        {
            get
            {
                return solidColorShader;
            }
            set
            {
                solidColorShader = value;
            }
        }

        [SerializeField]
        private Shader copyTextureShader;
        public Shader CopyTextureShader
        {
            get
            {
                return copyTextureShader;
            }
            set
            {
                copyTextureShader = value;
            }
        }

        [SerializeField]
        private Shader subDivisionMapShader;
        public Shader SubDivisionMapShader
        {
            get
            {
                return subDivisionMapShader;
            }
            set
            {
                subDivisionMapShader = value;
            }
        }

        [SerializeField]
        private Shader blurShader;
        public Shader BlurShader
        {
            get
            {
                return blurShader;
            }
            set
            {
                blurShader = value;
            }
        }

        [SerializeField]
        private Shader blurRadiusShader;
        public Shader BlurRadiusShader
        {
            get
            {
                return blurRadiusShader;
            }
            set
            {
                blurRadiusShader = value;
            }
        }

        [SerializeField]
        private Shader elevationPainterShader;
        public Shader ElevationPainterShader
        {
            get
            {
                return elevationPainterShader;
            }
            set
            {
                elevationPainterShader = value;
            }
        }

        [SerializeField]
        private Shader heightSamplingPainterShader;
        public Shader HeightSamplingPainterShader
        {
            get
            {
                return heightSamplingPainterShader;
            }
            set
            {
                heightSamplingPainterShader = value;
            }
        }

        [SerializeField]
        private Shader subdivPainterShader;
        public Shader SubdivPainterShader
        {
            get
            {
                return subdivPainterShader;
            }
            set
            {
                subdivPainterShader = value;
            }
        }

        [SerializeField]
        private Shader painterCursorProjectorShader;
        public Shader PainterCursorProjectorShader
        {
            get
            {
                return painterCursorProjectorShader;
            }
            set
            {
                painterCursorProjectorShader = value;
            }
        }

        [SerializeField]
        private Shader albedoPainterShader;
        public Shader AlbedoPainterShader
        {
            get
            {
                return albedoPainterShader;
            }
            set
            {
                albedoPainterShader = value;
            }
        }

        [SerializeField]
        private Shader metallicPainterShader;
        public Shader MetallicPainterShader
        {
            get
            {
                return metallicPainterShader;
            }
            set
            {
                metallicPainterShader = value;
            }
        }

        [SerializeField]
        private Shader smoothnessPainterShader;
        public Shader SmoothnessPainterShader
        {
            get
            {
                return smoothnessPainterShader;
            }
            set
            {
                smoothnessPainterShader = value;
            }
        }

        [SerializeField]
        private Shader splatPainterShader;
        public Shader SplatPainterShader
        {
            get
            {
                return splatPainterShader;
            }
            set
            {
                splatPainterShader = value;
            }
        }

        [SerializeField]
        private Shader visibilityPainterShader;
        public Shader VisibilityPainterShader
        {
            get
            {
                return visibilityPainterShader;
            }
            set
            {
                visibilityPainterShader = value;
            }
        }

        [SerializeField]
        private Shader rampMakerShader;
        public Shader RampMakerShader
        {
            get
            {
                return rampMakerShader;
            }
            set
            {
                rampMakerShader = value;
            }
        }

        [SerializeField]
        private Shader pathPainterShader;
        public Shader PathPainterShader
        {
            get
            {
                return pathPainterShader;
            }
            set
            {
                pathPainterShader = value;
            }
        }

        [SerializeField]
        private Shader geometryLivePreviewShader;
        public Shader GeometryLivePreviewShader
        {
            get
            {
                return geometryLivePreviewShader;
            }
            set
            {
                geometryLivePreviewShader = value;
            }
        }

        [SerializeField]
        private Shader geometricalHeightMapShader;
        public Shader GeometricalHeightMapShader
        {
            get
            {
                return geometricalHeightMapShader;
            }
            set
            {
                geometricalHeightMapShader = value;
            }
        }

        [SerializeField]
        private Shader foliageRemoverShader;
        public Shader FoliageRemoverShader
        {
            get
            {
                return foliageRemoverShader;
            }
            set
            {
                foliageRemoverShader = value;
            }
        }

        [SerializeField]
        private Shader maskVisualizerShader;
        public Shader MaskVisualizerShader
        {
            get
            {
                return maskVisualizerShader;
            }
            set
            {
                maskVisualizerShader = value;
            }
        }

        [SerializeField]
        private Shader stamperShader;
        public Shader StamperShader
        {
            get
            {
                return stamperShader;
            }
            set
            {
                stamperShader = value;
            }
        }

        [SerializeField]
        private Shader terrainNormalMapShader;
        public Shader TerrainNormalMapShader
        {
            get
            {
                return terrainNormalMapShader;
            }
            set
            {
                terrainNormalMapShader = value;
            }
        }

        [SerializeField]
        private Shader terrainPerPixelNormalMapShader;
        public Shader TerrainPerPixelNormalMapShader
        {
            get
            {
                return terrainPerPixelNormalMapShader;
            }
            set
            {
                terrainPerPixelNormalMapShader = value;
            }
        }

        [SerializeField]
        private Shader textureStamperBrushShader;
        public Shader TextureStamperBrushShader
        {
            get
            {
                return textureStamperBrushShader;
            }
            set
            {
                textureStamperBrushShader = value;
            }
        }

        [SerializeField]
        private Shader grassPreviewShader;
        public Shader GrassPreviewShader
        {
            get
            {
                return grassPreviewShader;
            }
            set
            {
                grassPreviewShader = value;
            }
        }

        [SerializeField]
        private Shader navHelperDummyGameObjectShader;
        public Shader NavHelperDummyGameObjectShader
        {
            get
            {
                return navHelperDummyGameObjectShader;
            }
            set
            {
                navHelperDummyGameObjectShader = value;
            }
        }

        [SerializeField]
        private Shader splatsToAlbedoShader;
        public Shader SplatsToAlbedoShader
        {
            get
            {
                return splatsToAlbedoShader;
            }
            set
            {
                splatsToAlbedoShader = value;
            }
        }

        [SerializeField]
        private Shader unlitChannelMaskShader;
        public Shader UnlitChannelMaskShader
        {
            get
            {
                return unlitChannelMaskShader;
            }
            set
            {
                unlitChannelMaskShader = value;
            }
        }

        [SerializeField]
        private Shader channelToGrayscaleShader;
        public Shader ChannelToGrayscaleShader
        {
            get
            {
                return channelToGrayscaleShader;
            }
            set
            {
                channelToGrayscaleShader = value;
            }
        }

        [SerializeField]
        private Shader heightMapFromMeshShader;
        public Shader HeightMapFromMeshShader
        {
            get
            {
                return heightMapFromMeshShader;
            }
            set
            {
                heightMapFromMeshShader = value;
            }
        }

        [SerializeField]
        private Shader curveFilterShader;
        public Shader CurveFilterShader
        {
            get
            {
                return curveFilterShader;
            }
            set
            {
                curveFilterShader = value;
            }
        }

        [SerializeField]
        private Shader invertFilterShader;
        public Shader InvertFilterShader
        {
            get
            {
                return invertFilterShader;
            }
            set
            {
                invertFilterShader = value;
            }
        }

        [SerializeField]
        private Shader stepFilterShader;
        public Shader StepFilterShader
        {
            get
            {
                return stepFilterShader;
            }
            set
            {
                stepFilterShader = value;
            }
        }

        [SerializeField]
        private Shader warpFilterShader;
        public Shader WarpFilterShader
        {
            get
            {
                return warpFilterShader;
            }
            set
            {
                warpFilterShader = value;
            }
        }

        [SerializeField]
        private Shader steepnessMapGeneratorShader;
        public Shader SteepnessMapGeneratorShader
        {
            get
            {
                return steepnessMapGeneratorShader;
            }
            set
            {
                steepnessMapGeneratorShader = value;
            }
        }

        [SerializeField]
        private Shader noiseMapGeneratorShader;
        public Shader NoiseMapGeneratorShader
        {
            get
            {
                return noiseMapGeneratorShader;
            }
            set
            {
                noiseMapGeneratorShader = value;
            }
        }

        [SerializeField]
        private Shader hydraulicErosionFilter;
        public Shader HydraulicErosionFilter
        {
            get
            {
                return hydraulicErosionFilter;
            }
            set
            {
                hydraulicErosionFilter = value;
            }
        }

        [SerializeField]
        private Shader blendMapGeneratorShader;
        public Shader BlendMapGeneratorShader
        {
            get
            {
                return blendMapGeneratorShader;
            }
            set
            {
                blendMapGeneratorShader = value;
            }
        }

        [SerializeField]
        private Shader distributionMapGeneratorShader;
        public Shader DistributionMapGeneratorShader
        {
            get
            {
                return distributionMapGeneratorShader;
            }
            set
            {
                distributionMapGeneratorShader = value;
            }
        }

        [SerializeField]
        private Shader interactiveGrassVectorFieldShader;
        public Shader InteractiveGrassVectorFieldShader
        {
            get
            {
                return interactiveGrassVectorFieldShader;
            }
            set
            {
                interactiveGrassVectorFieldShader = value;
            }
        }

        [SerializeField]
        private Shader subdivLivePreviewShader;
        public Shader SubdivLivePreviewShader
        {
            get
            {
                return subdivLivePreviewShader;
            }
            set
            {
                subdivLivePreviewShader = value;
            }
        }

        [SerializeField]
        private Shader visibilityLivePreviewShader;
        public Shader VisibilityLivePreviewShader
        {
            get
            {
                return visibilityLivePreviewShader;
            }
            set
            {
                visibilityLivePreviewShader = value;
            }
        }

        [SerializeField]
        private Shader terracePainterShader;
        public Shader TerracePainterShader
        {
            get
            {
                return terracePainterShader;
            }
            set
            {
                terracePainterShader = value;
            }
        }

        [SerializeField]
        private Shader remapPainterShader;
        public Shader RemapPainterShader
        {
            get
            {
                return remapPainterShader;
            }
            set
            {
                remapPainterShader = value;
            }
        }

        [SerializeField]
        private Shader noisePainterShader;
        public Shader NoisePainterShader
        {
            get
            {
                return noisePainterShader;
            }
            set
            {
                noisePainterShader = value;
            }
        }

        [SerializeField]
        private Shader heightmapConverterEncodeRGShader;
        public Shader HeightmapConverterEncodeRGShader
        {
            get
            {
                return heightmapConverterEncodeRGShader;
            }
            set
            {
                heightmapConverterEncodeRGShader = value;
            }
        }

        [SerializeField]
        private Shader heightmapDecodeGrayscaleShader;
        public Shader HeightmapDecodeGrayscaleShader
        {
            get
            {
                return heightmapDecodeGrayscaleShader;
            }
            set
            {
                heightmapDecodeGrayscaleShader = value;
            }
        }

        [SerializeField]
        private Shader drawTex2DArraySliceShader;
        public Shader DrawTex2DArraySliceShader
        {
            get
            {
                return drawTex2DArraySliceShader;
            }
            set
            {
                drawTex2DArraySliceShader = value;
            }
        }
    }
}
