using UnityEngine;

namespace Pinwheel.Griffin.GroupTool
{
    [System.Serializable]
    public struct GShadingOverride
    {
        [SerializeField]
        private bool overrideCustomMaterial;
        public bool OverrideCustomMaterial
        {
            get
            {
                return overrideCustomMaterial;
            }
            set
            {
                overrideCustomMaterial = value;
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
        private bool overrideAlbedoMapResolution;
        public bool OverrideAlbedoMapResolution
        {
            get
            {
                return overrideAlbedoMapResolution;
            }
            set
            {
                overrideAlbedoMapResolution = value;
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
                albedoMapResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(value), GCommon.TEXTURE_SIZE_MIN, GCommon.TEXTURE_SIZE_MAX);
            }
        }

        [SerializeField]
        private bool overrideMetallicMapResolution;
        public bool OverrideMetallicMapResolution
        {
            get
            {
                return overrideMetallicMapResolution;
            }
            set
            {
                overrideMetallicMapResolution = value;
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
                metallicMapResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(value), GCommon.TEXTURE_SIZE_MIN, GCommon.TEXTURE_SIZE_MAX);
            }
        }

        [SerializeField]
        private bool overrideAlbedoMapPropertyName;
        public bool OverrideAlbedoMapPropertyName
        {
            get
            {
                return overrideAlbedoMapPropertyName;
            }
            set
            {
                overrideAlbedoMapPropertyName = value;
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
        private bool overrideMetallicMapPropertyName;
        public bool OverrideMetallicMapPropertyName
        {
            get
            {
                return overrideMetallicMapPropertyName;
            }
            set
            {
                overrideMetallicMapPropertyName = value;
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
        private bool overrideColorByHeight;
        public bool OverrideColorByHeight
        {
            get
            {
                return overrideColorByHeight;
            }
            set
            {
                overrideColorByHeight = value;
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
                    colorByHeight = GUtilities.Clone(GGriffinSettings.Instance.TerrainDataDefault.Shading.ColorByHeight);
                }
                return colorByHeight;
            }
            set
            {
                colorByHeight = value;
            }
        }

        [SerializeField]
        private bool overrideColorByNormal;
        public bool OverrideColorByNormal
        {
            get
            {
                return overrideColorByNormal;
            }
            set
            {
                overrideColorByNormal = value;
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
                    colorByNormal = GUtilities.Clone(GGriffinSettings.Instance.TerrainDataDefault.Shading.ColorByNormal);
                }
                return colorByNormal;
            }
            set
            {
                colorByNormal = value;
            }
        }

        [SerializeField]
        private bool overrideColorBlendCurve;
        public bool OverrideColorBlendCurve
        {
            get
            {
                return overrideColorBlendCurve;
            }
            set
            {
                overrideColorBlendCurve = value;
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
                    colorBlendCurve = GUtilities.Clone(GGriffinSettings.Instance.TerrainDataDefault.Shading.ColorBlendCurve);
                }
                return colorBlendCurve;
            }
            set
            {
                colorBlendCurve = value;
            }
        }

        [SerializeField]
        private bool overrideColorByHeightPropertyName;
        public bool OverrideColorByHeightPropertyName
        {
            get
            {
                return overrideColorByHeightPropertyName;
            }
            set
            {
                overrideColorByHeightPropertyName = value;
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
        private bool overrideColorByNormalPropertyName;
        public bool OverrideColorByNormalPropertyName
        {
            get
            {
                return overrideColorByNormalPropertyName;
            }
            set
            {
                overrideColorByNormalPropertyName = value;
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
        private bool overrideColorBlendPropertyName;
        public bool OverrideColorBlendPropertyName
        {
            get
            {
                return overrideColorBlendPropertyName;
            }
            set
            {
                overrideColorBlendPropertyName = value;
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
        private bool overrideDimensionPropertyName;
        public bool OverrideDimensionPropertyName
        {
            get
            {
                return overrideDimensionPropertyName;
            }
            set
            {
                overrideDimensionPropertyName = value;
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
        private bool overrideSplats;
        public bool OverrideSplats
        {
            get
            {
                return overrideSplats;
            }
            set
            {
                overrideSplats = value;
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

        [SerializeField]
        private bool overrideSplatControlResolution;
        public bool OverrideSplatControlResolution
        {
            get
            {
                return overrideSplatControlResolution;
            }
            set
            {
                overrideSplatControlResolution = value;
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
                splatControlResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(value), GCommon.TEXTURE_SIZE_MIN, GCommon.TEXTURE_SIZE_MAX);
            }
        }

        [SerializeField]
        private bool overrideSplatControlMapPropertyName;
        public bool OverrideSplatControlMapPropertyName
        {
            get
            {
                return overrideSplatControlMapPropertyName;
            }
            set
            {
                overrideSplatControlMapPropertyName = value;
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
        private bool overrideSplatMapPropertyName;
        public bool OverrideSplatMapPropertyName
        {
            get
            {
                return overrideSplatMapPropertyName;
            }
            set
            {
                overrideSplatMapPropertyName = value;
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
        private bool overrideSplatNormalPropertyName;
        public bool OverrideSplatNormalPropertyName
        {
            get
            {
                return overrideSplatNormalPropertyName;
            }
            set
            {
                overrideSplatNormalPropertyName = value;
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
        private bool overrideSplatMetallicPropertyName;
        public bool OverrideSplatMetallicPropertyName
        {
            get
            {
                return overrideSplatMetallicPropertyName;
            }
            set
            {
                overrideSplatMetallicPropertyName = value;
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
        private bool overrideSplatSmoothnessPropertyName;
        public bool OverrideSplatSmoothnessPropertyName
        {
            get
            {
                return overrideSplatSmoothnessPropertyName;
            }
            set
            {
                overrideSplatSmoothnessPropertyName = value;
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

        public void Reset()
        {
            OverrideCustomMaterial = false;
            OverrideCustomMaterial = false;
            OverrideAlbedoMapResolution = false;
            OverrideMetallicMapResolution = false;
            OverrideAlbedoMapPropertyName = false;
            OverrideMetallicMapPropertyName = false;
            OverrideColorByHeight = false;
            OverrideColorByNormal = false;
            OverrideColorBlendCurve = false;
            OverrideColorByHeightPropertyName = false;
            OverrideColorByNormalPropertyName = false;
            OverrideColorBlendPropertyName = false;
            OverrideDimensionPropertyName = false;
            OverrideSplatControlResolution = false;
            OverrideSplatControlMapPropertyName = false;
            OverrideSplatMapPropertyName = false;
            OverrideSplatNormalPropertyName = false;
            OverrideSplatMetallicPropertyName = false;
            OverrideSplatSmoothnessPropertyName = false;

            CustomMaterial = GGriffinSettings.Instance.TerrainDataDefault.Shading.CustomMaterial;
            AlbedoMapResolution = GGriffinSettings.Instance.TerrainDataDefault.Shading.AlbedoMapResolution; ;
            MetallicMapResolution = GGriffinSettings.Instance.TerrainDataDefault.Shading.MetallicMapResolution; ;
            AlbedoMapPropertyName = GGriffinSettings.Instance.TerrainDataDefault.Shading.AlbedoMapPropertyName;
            MetallicMapPropertyName = GGriffinSettings.Instance.TerrainDataDefault.Shading.MetallicMapPropertyName;
            ColorByHeight = GUtilities.Clone(GGriffinSettings.Instance.TerrainDataDefault.Shading.ColorByHeight);
            ColorByNormal = GUtilities.Clone(GGriffinSettings.Instance.TerrainDataDefault.Shading.ColorByNormal);
            ColorBlendCurve = GUtilities.Clone(GGriffinSettings.Instance.TerrainDataDefault.Shading.ColorBlendCurve);
            ColorByHeightPropertyName = GGriffinSettings.Instance.TerrainDataDefault.Shading.ColorByHeightPropertyName;
            ColorByNormalPropertyName = GGriffinSettings.Instance.TerrainDataDefault.Shading.ColorByNormalPropertyName;
            ColorBlendPropertyName = GGriffinSettings.Instance.TerrainDataDefault.Shading.ColorBlendPropertyName;
            DimensionPropertyName = GGriffinSettings.Instance.TerrainDataDefault.Shading.DimensionPropertyName;
            SplatControlResolution = GGriffinSettings.Instance.TerrainDataDefault.Shading.SplatControlResolution;
            SplatControlMapPropertyName = GGriffinSettings.Instance.TerrainDataDefault.Shading.SplatControlMapPropertyName;
            SplatMapPropertyName = GGriffinSettings.Instance.TerrainDataDefault.Shading.SplatMapPropertyName;
            SplatNormalPropertyName = GGriffinSettings.Instance.TerrainDataDefault.Shading.SplatNormalPropertyName;
            SplatMetallicPropertyName = GGriffinSettings.Instance.TerrainDataDefault.Shading.SplatMetallicPropertyName;
            SplatSmoothnessPropertyName = GGriffinSettings.Instance.TerrainDataDefault.Shading.SplatSmoothnessPropertyName;
        }

        public void Override(GShading s)
        {
            if (OverrideCustomMaterial && CustomMaterial != null)
            {
                if (s.CustomMaterial == null)
                {
                    Material mat = Object.Instantiate(CustomMaterial);
                    mat.name = "TerrainMaterial_" + s.TerrainData.Id;
#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.CreateAsset(mat, string.Format("Assets/{0}.mat", mat.name));
#endif
                    s.CustomMaterial = mat;
                }
                else
                {
                    s.CustomMaterial.shader = CustomMaterial.shader;
                }
                s.UpdateMaterials();
            }
            if (OverrideAlbedoMapResolution)
                s.AlbedoMapResolution = AlbedoMapResolution;
            if (OverrideMetallicMapResolution)
                s.MetallicMapResolution = MetallicMapResolution;
            if (OverrideAlbedoMapPropertyName)
                s.AlbedoMapPropertyName = AlbedoMapPropertyName;
            if (OverrideMetallicMapPropertyName)
                s.MetallicMapPropertyName = MetallicMapPropertyName;
            if (OverrideColorByHeight)
                s.ColorByHeight = GUtilities.Clone(ColorByHeight);
            if (OverrideColorByNormal)
                s.ColorByNormal = GUtilities.Clone(ColorByNormal);
            if (OverrideColorBlendCurve)
                s.ColorBlendCurve = GUtilities.Clone(ColorBlendCurve);
            if (OverrideColorByHeightPropertyName)
                s.ColorByHeightPropertyName = ColorByHeightPropertyName;
            if (OverrideColorByNormalPropertyName)
                s.ColorByNormalPropertyName = ColorByNormalPropertyName;
            if (OverrideColorBlendPropertyName)
                s.ColorBlendPropertyName = ColorBlendPropertyName;
            if (OverrideDimensionPropertyName)
                s.DimensionPropertyName = DimensionPropertyName;
            if (OverrideSplats)
                s.Splats = Splats;
            if (OverrideSplatControlResolution)
                s.SplatControlResolution = SplatControlResolution;
            if (OverrideSplatControlMapPropertyName)
                s.SplatControlMapPropertyName = SplatControlMapPropertyName;
            if (OverrideSplatMapPropertyName)
                s.SplatMapPropertyName = SplatMapPropertyName;
            if (OverrideSplatNormalPropertyName)
                s.SplatNormalPropertyName = SplatNormalPropertyName;
            if (OverrideSplatMetallicPropertyName)
                s.SplatMetallicPropertyName = SplatMetallicPropertyName;
            if (OverrideSplatSmoothnessPropertyName)
                s.SplatSmoothnessPropertyName = SplatSmoothnessPropertyName;
            s.UpdateLookupTextures();
        }
    }
}
