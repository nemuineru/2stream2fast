using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Griffin
{
    [System.Serializable]
    public struct GCreateTerrainWizardSettings
    {
        [SerializeField]
        private GLightingModel lightingModel;
        public GLightingModel LightingModel
        {
            get
            {
                if (GCommon.CurrentRenderPipeline == GRenderPipelineType.Lightweight)
                {
                    lightingModel = GLightingModel.PBR;
                }
                return lightingModel;
            }
            set
            {
                lightingModel = value;
            }
        }

        [SerializeField]
        private GTexturingModel texturingModel;
        public GTexturingModel TexturingModel
        {
            get
            {
                return texturingModel;
            }
            set
            {
                texturingModel = value;
            }
        }

        [SerializeField]
        private GSplatsModel splatsModel;
        public GSplatsModel SplatsModel
        {
            get
            {
                return splatsModel;
            }
            set
            {
                splatsModel = value;
            }
        }

        [SerializeField]
        private List<GWizardMaterialTemplate> builtinRPMaterials;
        public List<GWizardMaterialTemplate> BuiltinRPMaterials
        {
            get
            {
                if (builtinRPMaterials == null)
                {
                    builtinRPMaterials = new List<GWizardMaterialTemplate>();
                }
                return builtinRPMaterials;
            }
            set
            {
                builtinRPMaterials = value;
            }
        }

        [SerializeField]
        private List<GWizardMaterialTemplate> lightweightRPMaterials;
        public List<GWizardMaterialTemplate> LightweightRPMaterials
        {
            get
            {
                if (lightweightRPMaterials == null)
                {
                    lightweightRPMaterials = new List<GWizardMaterialTemplate>();
                }
                return lightweightRPMaterials;
            }
            set
            {
                lightweightRPMaterials = value;
            }
        }

        [SerializeField]
        private List<GWizardMaterialTemplate> universalRPMaterials;
        public List<GWizardMaterialTemplate> UniversalRPMaterials
        {
            get
            {
                if (universalRPMaterials == null)
                {
                    universalRPMaterials = new List<GWizardMaterialTemplate>();
                }
                return universalRPMaterials;
            }
            set
            {
                universalRPMaterials = value;
            }
        }

        public Material GetClonedMaterial()
        {
            return GetClonedMaterial(LightingModel, TexturingModel, SplatsModel);
        }

        public Material GetClonedMaterial(GLightingModel light, GTexturingModel texturing, GSplatsModel splats = GSplatsModel.Splats4)
        {
            GWizardMaterialTemplate matTemplate;
            List<GWizardMaterialTemplate> collection =
                GCommon.CurrentRenderPipeline == GRenderPipelineType.Universal ? UniversalRPMaterials :
                GCommon.CurrentRenderPipeline == GRenderPipelineType.Lightweight ? LightweightRPMaterials :
                BuiltinRPMaterials;

            if (texturing != GTexturingModel.Splat)
            {
                matTemplate = collection.Find(m =>
                {
                    return m.LightingModel == light && m.TexturingModel == texturing;
                });
            }
            else
            {
                matTemplate = collection.Find(m =>
                {
                    return m.LightingModel == light && m.TexturingModel == texturing && m.SplatsModel == splats;
                });
            }

            Material mat = null;
            if (matTemplate.Material != null)
            {
                mat = Object.Instantiate(matTemplate.Material);
            }

            return mat;
        }

        public Material GetClonedMaterial(GRenderPipelineType pipeline, GLightingModel light, GTexturingModel texturing, GSplatsModel splats = GSplatsModel.Splats4)
        {
            GWizardMaterialTemplate matTemplate;
            List<GWizardMaterialTemplate> collection =
                pipeline == GRenderPipelineType.Universal ? UniversalRPMaterials :
                BuiltinRPMaterials;

            if (texturing != GTexturingModel.Splat)
            {
                matTemplate = collection.Find(m =>
                {
                    return m.LightingModel == light && m.TexturingModel == texturing;
                });
            }
            else
            {
                matTemplate = collection.Find(m =>
                {
                    return m.LightingModel == light && m.TexturingModel == texturing && m.SplatsModel == splats;
                });
            }

            Material mat = null;
            if (matTemplate.Material != null)
            {
                mat = UnityEngine.Object.Instantiate(matTemplate.Material);
            }

            return mat;
        }

        public bool FindMaterialTemplate(Shader shader, GRenderPipelineType pipeline, out GWizardMaterialTemplate template)
        {
            template = default;
            List<GWizardMaterialTemplate> templateList =
                pipeline == GRenderPipelineType.Builtin ? BuiltinRPMaterials :
                pipeline == GRenderPipelineType.Universal ? UniversalRPMaterials :
                null;
            if (templateList == null)
            {
                return false;
            }

            int index = templateList.FindIndex((t) =>
            {
                return t.Material != null && t.Material.shader == shader;
            });

            if (index >= 0)
            {
                template = templateList[index];
            }

            return index >= 0;
        }
    }
}
