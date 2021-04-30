using UnityEngine;

namespace Pinwheel.Griffin
{
    public static class GInternalMaterials
    {
        private static Material unlitTextureMaterial;
        public static Material UnlitTextureMaterial
        {
            get
            {
                if (unlitTextureMaterial == null)
                {
                    unlitTextureMaterial = new Material(Shader.Find("Unlit/Texture"));
                }
                return unlitTextureMaterial;
            }
        }

        private static Material unlitTransparentMaterial;
        public static Material UnlitTransparentMaterial
        {
            get
            {
                if (unlitTransparentMaterial == null)
                {
                    unlitTransparentMaterial = new Material(Shader.Find("Unlit/Transparent"));
                }
                return unlitTransparentMaterial;
            }
        }

        private static Material copyTextureMaterial;
        public static Material CopyTextureMaterial
        {
            get
            {
                if (copyTextureMaterial == null)
                {
                    copyTextureMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.CopyTextureShader);
                }
                return copyTextureMaterial;
            }
        }

        private static Material subDivisionMapMaterial;
        public static Material SubDivisionMapMaterial
        {
            get
            {
                if (subDivisionMapMaterial == null)
                {
                    subDivisionMapMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.SubDivisionMapShader);
                    subDivisionMapMaterial.SetFloat("_Epsilon", GCommon.SUB_DIV_EPSILON);
                    subDivisionMapMaterial.SetFloat("_PixelOffset", GCommon.SUB_DIV_PIXEL_OFFSET);
                    subDivisionMapMaterial.SetFloat("_Step", GCommon.SUB_DIV_STEP);
                }
                return subDivisionMapMaterial;
            }
        }

        private static Material blurMaterial;
        public static Material BlurMaterial
        {
            get
            {
                if (blurMaterial == null)
                {
                    blurMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.BlurShader);
                }
                return blurMaterial;
            }
        }

        private static Material blurRadiusMaterial;
        public static Material BlurRadiusMaterial
        {
            get
            {
                if (blurRadiusMaterial == null)
                {
                    blurRadiusMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.BlurRadiusShader);
                }
                return blurRadiusMaterial;
            }
        }

        private static Material elevationPainterMaterial;
        public static Material ElevationPainterMaterial
        {
            get
            {
                if (elevationPainterMaterial == null)
                {
                    elevationPainterMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.ElevationPainterShader);
                }
                return elevationPainterMaterial;
            }
        }

        private static Material heightSamplingPainterMaterial;
        public static Material HeightSamplingPainterMaterial
        {
            get
            {
                if (heightSamplingPainterMaterial == null)
                {
                    heightSamplingPainterMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.HeightSamplingPainterShader);
                }
                return heightSamplingPainterMaterial;
            }
        }

        private static Material subDivPainterMaterial;
        public static Material SubDivPainterMaterial
        {
            get
            {
                if (subDivPainterMaterial == null)
                {
                    subDivPainterMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.SubdivPainterShader);
                }
                return subDivPainterMaterial;
            }
        }

        private static Material painterCursorProjectorMaterial;
        public static Material PainterCursorProjectorMaterial
        {
            get
            {
                if (painterCursorProjectorMaterial == null)
                {
                    painterCursorProjectorMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.PainterCursorProjectorShader);
                }
                return painterCursorProjectorMaterial;
            }
        }

        private static Material albedoPainterMaterial;
        public static Material AlbedoPainterMaterial
        {
            get
            {
                if (albedoPainterMaterial == null)
                {
                    albedoPainterMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.AlbedoPainterShader);
                }
                return albedoPainterMaterial;
            }
        }

        private static Material metallicPainterMaterial;
        public static Material MetallicPainterMaterial
        {
            get
            {
                if (metallicPainterMaterial == null)
                {
                    metallicPainterMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.MetallicPainterShader);
                }
                return metallicPainterMaterial;
            }
        }

        private static Material smoothnessPainterMaterial;
        public static Material SmoothnessPainterMaterial
        {
            get
            {
                if (smoothnessPainterMaterial == null)
                {
                    smoothnessPainterMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.SmoothnessPainterShader);
                }
                return smoothnessPainterMaterial;
            }
        }

        private static Material solidColorMaterial;
        public static Material SolidColorMaterial
        {
            get
            {
                if (solidColorMaterial == null)
                {
                    solidColorMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.SolidColorShader);
                }
                return solidColorMaterial;
            }
        }

        private static Material splatPainterMaterial;
        public static Material SplatPainterMaterial
        {
            get
            {
                if (splatPainterMaterial == null)
                {
                    splatPainterMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.SplatPainterShader);
                }
                return splatPainterMaterial;
            }
        }

        private static Material rampMakerMaterial;
        public static Material RampMakerMaterial
        {
            get
            {
                if (rampMakerMaterial == null)
                {
                    rampMakerMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.RampMakerShader);
                }
                return rampMakerMaterial;
            }
        }

        private static Material pathPainterMaterial;
        public static Material PathPainterMaterial
        {
            get
            {
                if (pathPainterMaterial == null)
                {
                    pathPainterMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.PathPainterShader);
                }
                return pathPainterMaterial;
            }
        }

        private static Material geometryLivePreviewMaterial;
        public static Material GeometryLivePreviewMaterial
        {
            get
            {
                if (geometryLivePreviewMaterial == null)
                {
                    geometryLivePreviewMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.GeometryLivePreviewShader);
                }
                return geometryLivePreviewMaterial;
            }
        }

        private static Material geometricalHeightMapMaterial;
        public static Material GeometricalHeightMapMaterial
        {
            get
            {
                if (geometricalHeightMapMaterial == null)
                {
                    geometricalHeightMapMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.GeometricalHeightMapShader);
                }
                return geometricalHeightMapMaterial;
            }
        }

        private static Material foliageRemoverMaterial;
        public static Material FoliageRemoverMaterial
        {
            get
            {
                if (foliageRemoverMaterial == null)
                {
                    foliageRemoverMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.FoliageRemoverShader);
                }
                return foliageRemoverMaterial;
            }
        }

        private static Material maskVisualizerMaterial;
        public static Material MaskVisualizerMaterial
        {
            get
            {
                if (maskVisualizerMaterial == null)
                {
                    maskVisualizerMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.MaskVisualizerShader);
                }
                return maskVisualizerMaterial;
            }
        }

        private static Material stamperMaterial;
        public static Material StamperMaterial
        {
            get
            {
                if (stamperMaterial == null)
                {
                    stamperMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.StamperShader);
                }
                return stamperMaterial;
            }
        }

        private static Material terrainNormalMapRendererMaterial;
        public static Material TerrainNormalMapRendererMaterial
        {
            get
            {
                if (terrainNormalMapRendererMaterial == null)
                {
                    terrainNormalMapRendererMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.TerrainNormalMapShader);
                }
                return terrainNormalMapRendererMaterial;
            }
        }

        private static Material terrainPerPixelNormalMapRendererMaterial;
        public static Material TerrainPerPixelNormalMapRendererMaterial
        {
            get
            {
                if (terrainPerPixelNormalMapRendererMaterial == null)
                {
                    terrainPerPixelNormalMapRendererMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.TerrainPerPixelNormalMapShader);
                }
                return terrainPerPixelNormalMapRendererMaterial;
            }
        }

        private static Material textureStamperBrushMaterial;
        public static Material TextureStamperBrushMaterial
        {
            get
            {
                if (textureStamperBrushMaterial == null)
                {
                    textureStamperBrushMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.TextureStamperBrushShader);
                }
                return textureStamperBrushMaterial;
            }
        }

        private static Material visibilityPainterMaterial;
        public static Material VisibilityPainterMaterial
        {
            get
            {
                if (visibilityPainterMaterial == null)
                {
                    visibilityPainterMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.VisibilityPainterShader);
                }
                return visibilityPainterMaterial;
            }
        }

        private static Material grassPreviewMaterial;
        public static Material GrassPreviewMaterial
        {
            get
            {
                if (grassPreviewMaterial == null)
                {
                    grassPreviewMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.GrassPreviewShader);
                }
                return grassPreviewMaterial;
            }
        }

        private static Material navHelperDummyGameObjectMaterial;
        public static Material NavHelperDummyGameObjectMaterial
        {
            get
            {
                if (navHelperDummyGameObjectMaterial == null)
                {
                    navHelperDummyGameObjectMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.NavHelperDummyGameObjectShader);
                }
                Color cyan = Color.cyan;
                navHelperDummyGameObjectMaterial.SetColor("_Color", new Color(cyan.r, cyan.g, cyan.b, 0.5f));
                return navHelperDummyGameObjectMaterial;
            }
        }

        private static Material splatsToAlbedoMaterial;
        public static Material SplatsToAlbedoMaterial
        {
            get
            {
                if (splatsToAlbedoMaterial == null)
                {
                    splatsToAlbedoMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.SplatsToAlbedoShader);
                }
                return splatsToAlbedoMaterial;
            }
        }

        private static Material unlitChannelMaskMaterial;
        public static Material UnlitChannelMaskMaterial
        {
            get
            {
                if (unlitChannelMaskMaterial == null)
                {
                    unlitChannelMaskMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.UnlitChannelMaskShader);
                }
                return unlitChannelMaskMaterial;
            }
        }

        private static Material channelToGrayscaleMaterial;
        public static Material ChannelToGrayscaleMaterial
        {
            get
            {
                if (channelToGrayscaleMaterial == null)
                {
                    channelToGrayscaleMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.ChannelToGrayscaleShader);
                }
                return channelToGrayscaleMaterial;
            }
        }

        private static Material heightMapFromMeshMaterial;
        public static Material HeightMapFromMeshMaterial
        {
            get
            {
                if (heightMapFromMeshMaterial == null)
                {
                    heightMapFromMeshMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.HeightMapFromMeshShader);
                }
                return heightMapFromMeshMaterial;
            }
        }

        private static Material interactiveGrassVectorFieldMaterial;
        public static Material InteractiveGrassVectorFieldMaterial
        {
            get
            {
                if (interactiveGrassVectorFieldMaterial == null)
                {
                    interactiveGrassVectorFieldMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.InteractiveGrassVectorFieldShader);
                }
                return interactiveGrassVectorFieldMaterial;
            }
        }

        private static Material subdivLivePreviewMaterial;
        public static Material SubdivLivePreviewMaterial
        {
            get
            {
                if (subdivLivePreviewMaterial == null)
                {
                    subdivLivePreviewMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.SubdivLivePreviewShader);
                }
                return subdivLivePreviewMaterial;
            }
        }

        private static Material visibilityLivePreviewMaterial;
        public static Material VisibilityLivePreviewMaterial
        {
            get
            {
                if (visibilityLivePreviewMaterial == null)
                {
                    visibilityLivePreviewMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.VisibilityLivePreviewShader);
                }
                return visibilityLivePreviewMaterial;
            }
        }

        private static Material terracePainterMaterial;
        public static Material TerracePainterMaterial
        {
            get
            {
                if (terracePainterMaterial == null)
                {
                    terracePainterMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.TerracePainterShader);
                }
                return terracePainterMaterial;
            }
        }

        private static Material remapPainterMaterial;
        public static Material RemapPainterMaterial
        {
            get
            {
                if (remapPainterMaterial == null)
                {
                    remapPainterMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.RemapPainterShader);
                }
                return remapPainterMaterial;
            }
        }

        private static Material noisePainterMaterial;
        public static Material NoisePainterMaterial
        {
            get
            {
                if (noisePainterMaterial == null)
                {
                    noisePainterMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.NoisePainterShader);
                }
                return noisePainterMaterial;
            }
        }

        private static Material heightmapConverterEncodeRGMaterial;
        public static Material HeightmapConverterEncodeRGMaterial
        {
            get
            {
                if (heightmapConverterEncodeRGMaterial == null)
                {
                    heightmapConverterEncodeRGMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.HeightmapConverterEncodeRGShader);
                }
                return heightmapConverterEncodeRGMaterial;
            }
        }

        private static Material heightmapDecodeGrayscaleMaterial;
        public static Material HeightmapDecodeGrayscaleMaterial
        {
            get
            {
                if (heightmapDecodeGrayscaleMaterial == null)
                {
                    heightmapDecodeGrayscaleMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.HeightmapDecodeGrayscaleShader);
                }
                return heightmapDecodeGrayscaleMaterial;
            }
        }

        private static Material drawTex2DArraySliceMaterial;
        public static Material DrawTex2DArraySliceMaterial
        {
            get
            {
                if (drawTex2DArraySliceMaterial == null)
                {
                    drawTex2DArraySliceMaterial = new Material(GGriffinSettings.Instance.InternalShaderSettings.DrawTex2DArraySliceShader);
                }
                return drawTex2DArraySliceMaterial;
            }
        }
    }
}
