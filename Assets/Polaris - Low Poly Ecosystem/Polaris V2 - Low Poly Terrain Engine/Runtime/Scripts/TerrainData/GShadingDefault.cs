using UnityEngine;
    
namespace Pinwheel.Griffin
{
    [System.Serializable]
    public struct GShadingDefault
    {
        [SerializeField]
        private GShadingSystem shadingSystem;
        public GShadingSystem ShadingSystem
        {
            get
            {
                return shadingSystem;
            }
            set
            {
                shadingSystem = value;
            }
        }

        [SerializeField]
        private Material customMaterial;
        public Material CustomMaterial
        {
            get
            {
                return customMaterial;
            }
            set
            {
                customMaterial = value;
            }
        }

        [SerializeField]
        private int albedoMapResolution;
        public int AlbedoMapResolution
        {
            get
            {
                return albedoMapResolution;
            }
            set
            {
                albedoMapResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(value), 32, 4096);
            }
        }

        [SerializeField]
        private int metallicMapResolution;
        public int MetallicMapResolution
        {
            get
            {
                return metallicMapResolution;
            }
            set
            {
                metallicMapResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(value), 32, 4096);
            }
        }

        [SerializeField]
        private string albedoMapPropertyName;
        public string AlbedoMapPropertyName
        {
            get
            {
                return albedoMapPropertyName;
            }
            set
            {
                albedoMapPropertyName = value;
            }
        }

        [SerializeField]
        private string metallicMapPropertyName;
        public string MetallicMapPropertyName
        {
            get
            {
                return metallicMapPropertyName;
            }
            set
            {
                metallicMapPropertyName = value;
            }
        }

        [SerializeField]
        private Gradient colorByHeight;
        public Gradient ColorByHeight
        {
            get
            {
                if (colorByHeight == null)
                {
                    colorByHeight = GUtilities.CreateFullWhiteGradient();
                }
                return colorByHeight;
            }
            set
            {
                colorByHeight = value;
            }
        }

        [SerializeField]
        private Gradient colorByNormal;
        public Gradient ColorByNormal
        {
            get
            {
                if (colorByNormal == null)
                {
                    colorByNormal = GUtilities.CreateFullWhiteGradient();
                }
                return colorByNormal;
            }
            set
            {
                colorByNormal = value;
            }
        }

        [SerializeField]
        private AnimationCurve colorBlendCurve;
        public AnimationCurve ColorBlendCurve
        {
            get
            {
                if (colorBlendCurve == null)
                {
                    colorBlendCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);
                }
                return colorBlendCurve;
            }
            set
            {
                colorBlendCurve = value;
            }
        }

        [SerializeField]
        private string colorByHeightPropertyName;
        public string ColorByHeightPropertyName
        {
            get
            {
                return colorByHeightPropertyName;
            }
            set
            {
                colorByHeightPropertyName = value;
            }
        }

        [SerializeField]
        private string colorByNormalPropertyName;
        public string ColorByNormalPropertyName
        {
            get
            {
                return colorByNormalPropertyName;
            }
            set
            {
                colorByNormalPropertyName = value;
            }
        }

        [SerializeField]
        private string colorBlendPropertyName;
        public string ColorBlendPropertyName
        {
            get
            {
                return colorBlendPropertyName;
            }
            set
            {
                colorBlendPropertyName = value;
            }
        }

        [SerializeField]
        private string dimensionPropertyName;
        public string DimensionPropertyName
        {
            get
            {
                return dimensionPropertyName;
            }
            set
            {
                dimensionPropertyName = value;
            }
        }

        [SerializeField]
        private int splatControlResolution;
        public int SplatControlResolution
        {
            get
            {
                return splatControlResolution;
            }
            set
            {
                splatControlResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(value), 32, 4096);
            }
        }

        [SerializeField]
        private string splatControlMapPropertyName;
        public string SplatControlMapPropertyName
        {
            get
            {
                return splatControlMapPropertyName;
            }
            set
            {
                splatControlMapPropertyName = value;
            }
        }

        [SerializeField]
        private string splatMapPropertyName;
        public string SplatMapPropertyName
        {
            get
            {
                return splatMapPropertyName;
            }
            set
            {
                splatMapPropertyName = value;
            }
        }

        [SerializeField]
        private string splatNormalPropertyName;
        public string SplatNormalPropertyName
        {
            get
            {
                return splatNormalPropertyName;
            }
            set
            {
                splatNormalPropertyName = value;
            }
        }

        [SerializeField]
        private string splatMetallicPropertyName;
        public string SplatMetallicPropertyName
        {
            get
            {
                return splatMetallicPropertyName;
            }
            set
            {
                splatMetallicPropertyName = value;
            }
        }

        [SerializeField]
        private string splatSmoothnessPropertyName;
        public string SplatSmoothnessPropertyName
        {
            get
            {
                return splatSmoothnessPropertyName;
            }
            set
            {
                splatSmoothnessPropertyName = value;
            }
        }

        [SerializeField]
        private GSplatPrototypeGroup splats;
        public GSplatPrototypeGroup Splats
        {
            get
            {
                return splats;
            }
            set
            {
                splats = value;
            }
        }
    }
}
