using UnityEngine;
using UnityEngine.Serialization;
#if __MICROSPLAT_POLARIS__
using JBooth.MicroSplat;
#endif
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Pinwheel.Griffin
{
    public class GShading : ScriptableObject
    {
        public const string ALBEDO_MAP_NAME = "Albedo Map";
        public const string METALLIC_MAP_NAME = "Metallic Map";
        public const string COLOR_BY_HEIGHT_MAP_NAME = "Color By Height Map";
        public const string COLOR_BY_NORMAL_MAP_NAME = "Color By Normal Map";
        public const string COLOR_BLEND_MAP_NAME = "Color Blend Map";
        public const string SPLAT_CONTROL_MAP_NAME = "Splat Control Map";

        [SerializeField]
        private GTerrainData terrainData;
        public GTerrainData TerrainData
        {
            get
            {
                return terrainData;
            }
            internal set
            {
                terrainData = value;
            }
        }

        [SerializeField]
        private GShadingSystem shadingSystem;
        public GShadingSystem ShadingSystem
        {
            get
            {
#if !__MICROSPLAT_POLARIS__
                shadingSystem = GShadingSystem.Polaris;
#endif
                return shadingSystem;
            }
            set
            {
                shadingSystem = value;
            }
        }

        public Material MaterialToRender
        {
            get
            {
                return CustomMaterial;
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
                int oldValue = albedoMapResolution;
                albedoMapResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(value), GCommon.TEXTURE_SIZE_MIN, GCommon.TEXTURE_SIZE_MAX);
                if (oldValue != albedoMapResolution)
                {
                    ResizeAlbedoMap();
                }
            }
        }

        [SerializeField]
        private Texture2D albedoMap;
        public Texture2D AlbedoMap
        {
            get
            {
                if (albedoMap == null)
                {
                    albedoMap = GCommon.CreateTexture(AlbedoMapResolution, Color.clear);
                    albedoMap.filterMode = FilterMode.Bilinear;
                    albedoMap.wrapMode = TextureWrapMode.Clamp;
                    albedoMap.name = ALBEDO_MAP_NAME;
                }
                GCommon.TryAddObjectToAsset(albedoMap, TerrainData);
                return albedoMap;
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
                int oldValue = metallicMapResolution;
                metallicMapResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(value), GCommon.TEXTURE_SIZE_MIN, GCommon.TEXTURE_SIZE_MAX);
                if (oldValue != metallicMapResolution)
                {
                    ResizeMetallicMap();
                }
            }
        }

        [SerializeField]
        [FormerlySerializedAs("internal_MetallicMap")]
        private Texture2D metallicMap;
        public Texture2D MetallicMap
        {
            get
            {
                if (metallicMap == null)
                {
                    metallicMap = GCommon.CreateTexture(MetallicMapResolution, Color.clear);
                    metallicMap.filterMode = FilterMode.Bilinear;
                    metallicMap.wrapMode = TextureWrapMode.Clamp;
                    metallicMap.name = METALLIC_MAP_NAME; ;
                }
                GCommon.TryAddObjectToAsset(metallicMap, TerrainData);
                return metallicMap;
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
        private Texture2D colorByHeightMap;
        public Texture2D ColorByHeightMap
        {
            get
            {
                if (colorByHeightMap == null)
                {
                    UpdateLookupTextures();
                }
                return colorByHeightMap;
            }
        }

        [SerializeField]
        private Texture2D colorByNormalMap;
        public Texture2D ColorByNormalMap
        {
            get
            {
                if (colorByNormalMap == null)
                {
                    UpdateLookupTextures();
                }
                return colorByNormalMap;
            }
        }

        [SerializeField]
        private Texture2D colorBlendMap;
        public Texture2D ColorBlendMap
        {
            get
            {
                if (colorBlendMap == null)
                {
                    UpdateLookupTextures();
                }
                return colorBlendMap;
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
        private int splatControlResolution;
        public int SplatControlResolution
        {
            get
            {
                return splatControlResolution;
            }
            set
            {
                int oldValue = splatControlResolution;
                splatControlResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(value), GCommon.TEXTURE_SIZE_MIN, GCommon.TEXTURE_SIZE_MAX);
                if (oldValue != splatControlResolution)
                {
                    ResizeSplatControlMaps();
                }
            }
        }

        [SerializeField]
        private Texture2D[] splatControls;
        public Texture2D[] SplatControls
        {
            get
            {
                if (splatControls == null)
                {
                    splatControls = new Texture2D[0];
                }
                if (splatControls.Length == SplatControlMapCount)
                {
                    return splatControls;
                }
                else
                {
                    Texture2D[] controls = new Texture2D[SplatControlMapCount];
                    for (int i = 0; i < Mathf.Min(SplatControlMapCount, splatControls.Length); ++i)
                    {
                        controls[i] = splatControls[i];
                    }
                    if (splatControls.Length > SplatControlMapCount)
                    {
                        for (int i = SplatControlMapCount; i < splatControls.Length; ++i)
                        {
                            Object.DestroyImmediate(splatControls[i], true);
                        }
                    }

                    splatControls = controls;
                    return splatControls;
                }
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

        public int SplatControlMapCount
        {
            get
            {
                if (Splats == null)
                {
                    return 0;
                }
                else
                {
                    return (Splats.Prototypes.Count + 3) / 4;
                }
            }
        }

#if __MICROSPLAT_POLARIS__
        [SerializeField]
        private TextureArrayConfig msTextureArrayConfig;
        public TextureArrayConfig MicroSplatTextureArrayConfig
        {
            get
            {
                return msTextureArrayConfig;
            }
            set
            {
                msTextureArrayConfig = value;
            }
        }
#endif

        private void Reset()
        {
            name = "Shading";
            ShadingSystem = GGriffinSettings.Instance.TerrainDataDefault.Shading.ShadingSystem;
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
            Splats = GGriffinSettings.Instance.TerrainDataDefault.Shading.Splats;
        }

        public void ResetFull()
        {
            Reset();
            GCommon.FillTexture(AlbedoMap, Color.clear);
            GCommon.FillTexture(MetallicMap, Color.clear);
            UpdateLookupTextures();
            TerrainData.SetDirty(GTerrainData.DirtyFlags.Shading);
            UpdateMaterials();
        }

        private void ResizeAlbedoMap()
        {
            if (albedoMap == null)
                return;
            Texture2D tmp = GCommon.CreateTexture(AlbedoMapResolution, Color.clear);
            RenderTexture rt = new RenderTexture(AlbedoMapResolution, AlbedoMapResolution, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            GCommon.CopyToRT(albedoMap, rt);
            GCommon.CopyFromRT(tmp, rt);
            rt.Release();
            Object.DestroyImmediate(rt);

            tmp.name = albedoMap.name;
            tmp.filterMode = albedoMap.filterMode;
            tmp.wrapMode = albedoMap.wrapMode;
            Object.DestroyImmediate(albedoMap, true);
            albedoMap = tmp;
            GCommon.TryAddObjectToAsset(albedoMap, TerrainData);
            UpdateMaterials();
        }

        public void UpdateMaterials()
        {
            if (MaterialToRender != null)
            {
                if (MaterialToRender.HasProperty(AlbedoMapPropertyName))
                {
                    MaterialToRender.SetTexture(AlbedoMapPropertyName, AlbedoMap);
                }
                if (MaterialToRender.HasProperty(MetallicMapPropertyName))
                {
                    MaterialToRender.SetTexture(MetallicMapPropertyName, MetallicMap);
                }
                if (MaterialToRender.HasProperty(ColorByHeightPropertyName))
                {
                    MaterialToRender.SetTexture(ColorByHeightPropertyName, ColorByHeightMap);
                }
                if (MaterialToRender.HasProperty(ColorByNormalPropertyName))
                {
                    MaterialToRender.SetTexture(ColorByNormalPropertyName, ColorByNormalMap);
                }
                if (MaterialToRender.HasProperty(ColorBlendPropertyName))
                {
                    MaterialToRender.SetTexture(ColorBlendPropertyName, ColorBlendMap);
                }
                if (MaterialToRender.HasProperty(DimensionPropertyName))
                {
                    Vector4 dim = new Vector4(
                        TerrainData.Geometry.Width,
                        TerrainData.Geometry.Height,
                        TerrainData.Geometry.Length,
                        0);
                    MaterialToRender.SetVector(DimensionPropertyName, dim);
                }

                for (int i = 0; i < SplatControlMapCount; ++i)
                {
                    if (MaterialToRender.HasProperty(SplatControlMapPropertyName + i))
                    {
                        MaterialToRender.SetTexture(SplatControlMapPropertyName + i, GetSplatControl(i));
                    }
                }

                if (Splats != null)
                {
                    for (int i = 0; i < Splats.Prototypes.Count; ++i)
                    {
                        GSplatPrototype p = Splats.Prototypes[i];
                        if (MaterialToRender.HasProperty(SplatMapPropertyName + i))
                        {
                            MaterialToRender.SetTexture(SplatMapPropertyName + i, p.Texture);
                            Vector2 terrainSize = new Vector2(TerrainData.Geometry.Width, TerrainData.Geometry.Length);
                            Vector2 textureScale = new Vector2(
                                p.TileSize.x != 0 ? terrainSize.x / p.TileSize.x : 0,
                                p.TileSize.y != 0 ? terrainSize.y / p.TileSize.y : 0);
                            Vector2 textureOffset = new Vector2(
                                p.TileOffset.x != 0 ? terrainSize.x / p.TileOffset.x : 0,
                                p.TileOffset.y != 0 ? terrainSize.y / p.TileOffset.y : 0);
                            MaterialToRender.SetTextureScale(SplatMapPropertyName + i, textureScale);
                            MaterialToRender.SetTextureOffset(SplatMapPropertyName + i, textureOffset);
                        }
                        if (MaterialToRender.HasProperty(SplatNormalPropertyName + i))
                        {
                            MaterialToRender.SetTexture(SplatNormalPropertyName + i, p.NormalMap);
                        }
                        if (MaterialToRender.HasProperty(SplatMetallicPropertyName + i))
                        {
                            MaterialToRender.SetFloat(SplatMetallicPropertyName + i, p.Metallic);
                        }
                        if (MaterialToRender.HasProperty(SplatSmoothnessPropertyName + i))
                        {
                            MaterialToRender.SetFloat(SplatSmoothnessPropertyName + i, p.Smoothness);
                        }
                    }
                }
            }
        }

        private void ResizeMetallicMap()
        {
            if (metallicMap == null)
                return;
            Texture2D tmp = GCommon.CreateTexture(MetallicMapResolution, Color.black);
            RenderTexture rt = new RenderTexture(MetallicMapResolution, MetallicMapResolution, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            GCommon.CopyToRT(metallicMap, rt);
            GCommon.CopyFromRT(tmp, rt);
            rt.Release();
            Object.DestroyImmediate(rt);

            tmp.name = metallicMap.name;
            tmp.filterMode = metallicMap.filterMode;
            tmp.wrapMode = metallicMap.wrapMode;
            Object.DestroyImmediate(metallicMap, true);
            metallicMap = tmp;
            GCommon.TryAddObjectToAsset(metallicMap, TerrainData);
            UpdateMaterials();
        }

        public void UpdateLookupTextures()
        {
            if (colorByHeightMap != null)
                Object.DestroyImmediate(colorByHeightMap, true);
            if (colorByNormalMap != null)
                Object.DestroyImmediate(colorByNormalMap, true);
            if (colorBlendMap != null)
                Object.DestroyImmediate(colorBlendMap, true);
            int width = 256;
            int height = 8;
            colorByHeightMap = new Texture2D(width, height, TextureFormat.ARGB32, false, false);
            colorByHeightMap.filterMode = FilterMode.Bilinear;
            colorByHeightMap.wrapMode = TextureWrapMode.Clamp;
            colorByHeightMap.name = COLOR_BY_HEIGHT_MAP_NAME;

            colorByNormalMap = new Texture2D(width, height, TextureFormat.ARGB32, false, false);
            colorByNormalMap.filterMode = FilterMode.Bilinear;
            colorByNormalMap.wrapMode = TextureWrapMode.Clamp;
            colorByNormalMap.name = COLOR_BY_NORMAL_MAP_NAME;

            colorBlendMap = new Texture2D(width, height, TextureFormat.ARGB32, false, false);
            colorBlendMap.filterMode = FilterMode.Bilinear;
            colorBlendMap.wrapMode = TextureWrapMode.Clamp;
            colorBlendMap.name = COLOR_BLEND_MAP_NAME;

            Color[] cbhColors = new Color[width * height];
            Color[] cbnColors = new Color[width * height];
            Color[] cbColors = new Color[width * height];

            for (int x = 0; x < width; ++x)
            {
                float f = Mathf.InverseLerp(0, width - 1, x);
                Color cbh = ColorByHeight.Evaluate(f);
                Color cbn = ColorByNormal.Evaluate(f);
                Color cb = Color.white * ColorBlendCurve.Evaluate(f);
                for (int y = 0; y < height; ++y)
                {
                    int index = GUtilities.To1DIndex(x, y, width);
                    cbhColors[index] = cbh;
                    cbnColors[index] = cbn;
                    cbColors[index] = cb;
                }
            }

            colorByHeightMap.SetPixels(cbhColors);
            colorByHeightMap.Apply();
            GCommon.TryAddObjectToAsset(colorByHeightMap, TerrainData);

            colorByNormalMap.SetPixels(cbnColors);
            colorByNormalMap.Apply();
            GCommon.TryAddObjectToAsset(colorByNormalMap, TerrainData);

            colorBlendMap.SetPixels(cbColors);
            colorBlendMap.Apply();
            GCommon.TryAddObjectToAsset(colorBlendMap, TerrainData);
        }

        public Texture2D GetSplatControl(int index)
        {
            if (index < 0 || index >= SplatControlMapCount)
                throw new System.ArgumentException("Index must be >=0 and <=SplatControlMapCount");
            else
            {
                Texture2D t = SplatControls[index];
                if (t == null)
                {
                    Color fillColor = (index == 0 && SplatControlMapCount == 1) ? new Color(1, 0, 0, 0) : new Color(0, 0, 0, 0);
                    t = GCommon.CreateTexture(SplatControlResolution, fillColor);
                    t.filterMode = FilterMode.Bilinear;
                    t.wrapMode = TextureWrapMode.Clamp;
                    t.name = SPLAT_CONTROL_MAP_NAME + " " + index;
                    SplatControls[index] = t;
                }
                GCommon.TryAddObjectToAsset(t, TerrainData);
                return t;
            }
        }

        private void ResizeSplatControlMaps()
        {
            for (int i = 0; i < SplatControlMapCount; ++i)
            {
                Texture2D t = GetSplatControl(i);
                if (t == null)
                    return;
                Texture2D tmp = GCommon.CreateTexture(SplatControlResolution, Color.clear);
                RenderTexture rt = new RenderTexture(SplatControlResolution, SplatControlResolution, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                GCommon.CopyToRT(t, rt);
                GCommon.CopyFromRT(tmp, rt);
                rt.Release();
                Object.DestroyImmediate(rt);

                tmp.name = t.name;
                tmp.filterMode = t.filterMode;
                tmp.wrapMode = t.wrapMode;
                Object.DestroyImmediate(t, true);
                SplatControls[i] = tmp;
                GCommon.TryAddObjectToAsset(tmp, TerrainData);
            }

            UpdateMaterials();
        }

        public void ConvertSplatsToAlbedo()
        {
            if (Splats == null)
                return;
            RenderTexture albedoRt = new RenderTexture(AlbedoMapResolution, AlbedoMapResolution, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            Material mat = GInternalMaterials.SplatsToAlbedoMaterial;

            for (int i = 0; i < SplatControlMapCount; ++i)
            {
                Texture2D controlMap = GetSplatControl(i);
                mat.SetTexture("_Control0", controlMap);
                for (int channel = 0; channel < 4; ++channel)
                {
                    int prototypeIndex = i * 4 + channel;
                    if (prototypeIndex < Splats.Prototypes.Count)
                    {
                        GSplatPrototype p = Splats.Prototypes[prototypeIndex];
                        mat.SetTexture("_Splat" + channel, p.Texture);
                        Vector2 terrainSize = new Vector2(TerrainData.Geometry.Width, TerrainData.Geometry.Length);
                        Vector2 textureScale = new Vector2(
                            p.TileSize.x != 0 ? terrainSize.x / p.TileSize.x : 0,
                            p.TileSize.y != 0 ? terrainSize.y / p.TileSize.y : 0);
                        Vector2 textureOffset = new Vector2(
                            p.TileOffset.x != 0 ? terrainSize.x / p.TileOffset.x : 0,
                            p.TileOffset.y != 0 ? terrainSize.y / p.TileOffset.y : 0);
                        mat.SetTextureScale("_Splat" + channel, textureScale);
                        mat.SetTextureOffset("_Splat" + channel, textureOffset);
                    }
                    else
                    {
                        mat.SetTexture("_Splat" + channel, null);
                        mat.SetTextureScale("_Splat" + channel, Vector2.zero);
                        mat.SetTextureOffset("_Splat" + channel, Vector2.zero);
                    }
                }

                GCommon.DrawQuad(albedoRt, GCommon.FullRectUvPoints, mat, 0);
            }

            GCommon.CopyFromRT(AlbedoMap, albedoRt);
            albedoRt.Release();
            GUtilities.DestroyObject(albedoRt);
        }

        public void CopyTo(GShading des)
        {
            des.AlbedoMapResolution = AlbedoMapResolution;
            des.MetallicMapResolution = MetallicMapResolution;
            des.AlbedoMapPropertyName = AlbedoMapPropertyName;
            des.MetallicMapPropertyName = MetallicMapPropertyName;
            des.ColorByHeight = GUtilities.Clone(ColorByHeight);
            des.ColorByNormal = GUtilities.Clone(ColorByNormal);
            des.ColorBlendCurve = GUtilities.Clone(ColorBlendCurve);
            des.ColorByHeightPropertyName = ColorByHeightPropertyName;
            des.ColorByNormalPropertyName = ColorByNormalPropertyName;
            des.ColorBlendPropertyName = ColorBlendPropertyName;
            des.DimensionPropertyName = DimensionPropertyName;
            des.Splats = Splats;
            des.SplatControlResolution = SplatControlResolution;
            des.SplatControlMapPropertyName = SplatControlMapPropertyName;
            des.SplatMapPropertyName = SplatMapPropertyName;
            des.SplatNormalPropertyName = SplatNormalPropertyName;
            des.SplatMetallicPropertyName = SplatMetallicPropertyName;
            des.SplatSmoothnessPropertyName = SplatSmoothnessPropertyName;
        }

        public bool IsMaterialUseNormalMap()
        {
            if (CustomMaterial == null)
                return false;
            return CustomMaterial.HasProperty(SplatNormalPropertyName + "0");
        }
    }
}
