using UnityEngine;

namespace Pinwheel.Griffin
{
    [System.Serializable]
    public struct GWizardMaterialTemplate
    {
        [SerializeField]
        private GRenderPipelineType pipeline;
        public GRenderPipelineType Pipeline
        {
            get
            {
                return pipeline;
            }
            set
            {
                pipeline = value;
            }
        }

        [SerializeField]
        private GLightingModel lightingModel;
        public GLightingModel LightingModel
        {
            get
            {
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
        private Material material;
        public Material Material
        {
            get
            {
                return material;
            }
            set
            {
                material = value;
            }
        }
    }
}
