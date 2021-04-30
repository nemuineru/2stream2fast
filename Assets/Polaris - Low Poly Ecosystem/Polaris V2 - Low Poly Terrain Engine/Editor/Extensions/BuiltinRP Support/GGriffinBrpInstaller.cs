using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pinwheel.Griffin.BuiltinRP
{
    public static class GGriffinBrpInstaller
    {
        public static void Install()
        {
            GGriffinBrpResources resources = GGriffinBrpResources.Instance;
            if (resources == null)
            {
                Debug.Log("Unable to load Griffin BuiltinRP Resources.");
            }

            List<GWizardMaterialTemplate> terrainMaterialTemplates = new List<GWizardMaterialTemplate>();
            #region PBR materials
            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.PBR,
                TexturingModel = GTexturingModel.Splat,
                SplatsModel = GSplatsModel.Splats4,
                Material = resources.TerrainPbr4SplatsMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.PBR,
                TexturingModel = GTexturingModel.Splat,
                SplatsModel = GSplatsModel.Splats4Normals4,
                Material = resources.TerrainPbr4Splats4NormalsMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.PBR,
                TexturingModel = GTexturingModel.Splat,
                SplatsModel = GSplatsModel.Splats8,
                Material = resources.TerrainPbr8SplatsMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.PBR,
                TexturingModel = GTexturingModel.GradientLookup,
                Material = resources.TerrainPbrGradientLookupMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.PBR,
                TexturingModel = GTexturingModel.VertexColor,
                Material = resources.TerrainPbrVertexColorMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.PBR,
                TexturingModel = GTexturingModel.ColorMap,
                Material = resources.TerrainPbrColorMapMaterial
            });
            #endregion
            #region Lambert materials
            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.Lambert,
                TexturingModel = GTexturingModel.Splat,
                SplatsModel = GSplatsModel.Splats4,
                Material = resources.TerrainLambert4SplatsMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.Lambert,
                TexturingModel = GTexturingModel.Splat,
                SplatsModel = GSplatsModel.Splats4Normals4,
                Material = resources.TerrainLambert4Splats4NormalsMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.Lambert,
                TexturingModel = GTexturingModel.Splat,
                SplatsModel = GSplatsModel.Splats8,
                Material = resources.TerrainLambert8SplatsMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.Lambert,
                TexturingModel = GTexturingModel.GradientLookup,
                Material = resources.TerrainLambertGradientLookupMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.Lambert,
                TexturingModel = GTexturingModel.VertexColor,
                Material = resources.TerrainLambertVertexColorMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.Lambert,
                TexturingModel = GTexturingModel.ColorMap,
                Material = resources.TerrainLambertColorMapMaterial
            });
            #endregion
            #region Blinn-Phong materials
            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.BlinnPhong,
                TexturingModel = GTexturingModel.Splat,
                SplatsModel = GSplatsModel.Splats4,
                Material = resources.TerrainBlinnPhong4SplatsMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.BlinnPhong,
                TexturingModel = GTexturingModel.Splat,
                SplatsModel = GSplatsModel.Splats4Normals4,
                Material = resources.TerrainBlinnPhong4Splats4NormalsMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.BlinnPhong,
                TexturingModel = GTexturingModel.Splat,
                SplatsModel = GSplatsModel.Splats8,
                Material = resources.TerrainBlinnPhong8SplatsMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.BlinnPhong,
                TexturingModel = GTexturingModel.GradientLookup,
                Material = resources.TerrainBlinnPhongGradientLookupMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.BlinnPhong,
                TexturingModel = GTexturingModel.VertexColor,
                Material = resources.TerrainBlinnPhongVertexColorMaterial
            });

            terrainMaterialTemplates.Add(new GWizardMaterialTemplate()
            {
                Pipeline = GRenderPipelineType.Builtin,
                LightingModel = GLightingModel.BlinnPhong,
                TexturingModel = GTexturingModel.ColorMap,
                Material = resources.TerrainBlinnPhongColorMapMaterial
            });
            #endregion

            GCreateTerrainWizardSettings wizardSetting = GGriffinSettings.Instance.WizardSettings;
            wizardSetting.BuiltinRPMaterials = terrainMaterialTemplates;
            GGriffinSettings.Instance.WizardSettings = wizardSetting;

            GTerrainDataDefault terrainDataDefault = GGriffinSettings.Instance.TerrainDataDefault;
            GFoliageDefault foliageDefault = terrainDataDefault.Foliage;
            foliageDefault.GrassMaterial = resources.GrassMaterial;
            foliageDefault.GrassInteractiveMaterial = resources.GrassInteractiveMaterial;
            foliageDefault.TreeBillboardMaterial = resources.TreeBillboardMaterial;
            foliageDefault.GrassPreviewMaterial = resources.GrassPreviewMaterial;
            terrainDataDefault.Foliage = foliageDefault;
            GGriffinSettings.Instance.TerrainDataDefault = terrainDataDefault;

            EditorUtility.SetDirty(GGriffinSettings.Instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Completed", "Successfully installed Polaris V2 Built-in Render Pipeline support.", "OK");
        }
    }
}
