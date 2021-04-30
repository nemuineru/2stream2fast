using System.Collections.Generic;
using UnityEngine;
using Type = System.Type;

namespace Pinwheel.Griffin
{
    public class GGeometry : ScriptableObject
    {
        public const string HEIGHT_MAP_NAME = "Height Map";

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
        private float width;
        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = Mathf.Max(1, value);
            }
        }

        [SerializeField]
        private float height;
        public float Height
        {
            get
            {
                return height;
            }
            set
            {
                height = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float length;
        public float Length
        {
            get
            {
                return length;
            }
            set
            {
                length = Mathf.Max(1, value);
            }
        }

        [SerializeField]
        private int heightMapResolution;
        public int HeightMapResolution
        {
            get
            {
                return heightMapResolution;
            }
            set
            {
                int oldValue = heightMapResolution;
                heightMapResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(value), GCommon.TEXTURE_SIZE_MIN, GCommon.TEXTURE_SIZE_MAX);
                if (oldValue != heightMapResolution)
                {
                    ResampleHeightMap();
                }
            }
        }

        [SerializeField]
        private Texture2D heightMap;
        public Texture2D HeightMap
        {
            get
            {
                if (heightMap == null)
                {
                    heightMap = GCommon.CreateTexture(HeightMapResolution, Color.clear, HeightMapFormat);
                    heightMap.filterMode = FilterMode.Bilinear;
                    heightMap.wrapMode = TextureWrapMode.Clamp;
                    heightMap.name = HEIGHT_MAP_NAME;
                    heightmapVersion = GVersionInfo.Number;
                }
                GCommon.TryAddObjectToAsset(heightMap, TerrainData);
                if (heightMap.format != HeightMapFormat)
                {
                    ReFormatHeightMap();
                }
                return heightMap;
            }
        }

        [SerializeField]
        private float heightmapVersion;
        private const float HEIGHT_MAP_VERSION_ENCODE_RG = 246;

        public static TextureFormat HeightMapFormat
        {
            get
            {
                return TextureFormat.RGBA32;
            }
        }

        public static RenderTextureFormat HeightMapRTFormat
        {
            get
            {
                return RenderTextureFormat.ARGB32;
            }
        }

        private Texture2D subDivisionMap;
        internal Texture2D Internal_SubDivisionMap
        {
            get
            {
                if (subDivisionMap == null)
                {
                    Internal_CreateNewSubDivisionMap();
                }
                return subDivisionMap;
            }
        }

        [SerializeField]
        private int meshBaseResolution;
        public int MeshBaseResolution
        {
            get
            {
                return meshBaseResolution;
            }
            set
            {
                meshBaseResolution = Mathf.Clamp(value, 0, 10);
            }
        }

        [SerializeField]
        private int meshResolution;
        public int MeshResolution
        {
            get
            {
                return meshResolution;
            }
            set
            {
                meshResolution = Mathf.Clamp(value, 0, 15);
            }
        }

        [SerializeField]
        private int chunkGridSize;
        public int ChunkGridSize
        {
            get
            {
                return chunkGridSize;
            }
            set
            {
                chunkGridSize = Mathf.Max(1, value);
            }
        }

        [SerializeField]
        private int lodCount;
        public int LODCount
        {
            get
            {
                return lodCount;
            }
            set
            {
                lodCount = Mathf.Clamp(value, 1, GCommon.MAX_LOD_COUNT);
            }
        }

        [SerializeField]
        private int displacementSeed;
        public int DisplacementSeed
        {
            get
            {
                return displacementSeed;
            }
            set
            {
                displacementSeed = value;
            }
        }

        [SerializeField]
        private float displacementStrength;
        public float DisplacementStrength
        {
            get
            {
                return displacementStrength;
            }
            set
            {
                displacementStrength = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private GPolygonDistributionMode polygonDistribution;
        public GPolygonDistributionMode PolygonDistribution
        {
            get
            {
                return polygonDistribution;
            }
            set
            {
                polygonDistribution = value;
            }
        }

        [SerializeField]
        private string polygonProcessorName;
        public string PolygonProcessorName
        {
            get
            {
                return polygonProcessorName;
            }
            set
            {
                polygonProcessorName = value;
            }
        }

        private static string PolygonProcessorInterfaceName
        {
            get
            {
                return typeof(IGPolygonProcessor).FullName;
            }
        }

        internal IGPolygonProcessor PolygonProcessor
        {
            get
            {
                if (string.IsNullOrEmpty(PolygonProcessorName))
                    return null;

                Type type = Type.GetType(PolygonProcessorName);
                if (type != null)
                {
                    return System.Activator.CreateInstance(Type.GetType(PolygonProcessorName)) as IGPolygonProcessor;
                }
                else
                {
                    PolygonProcessorName = null;
                    return null;
                }
            }
        }

        private static List<Type> polygonProcessorTypeCollection;
        public static List<Type> PolygonProcessorTypeCollection
        {
            get
            {
                if (polygonProcessorTypeCollection == null)
                {
                    polygonProcessorTypeCollection = new List<Type>();
                }
                return polygonProcessorTypeCollection;
            }
            set
            {
                polygonProcessorTypeCollection = value;
            }
        }

        private List<Rect> dirtyRegion;
        private List<Rect> DirtyRegion
        {
            get
            {
                if (dirtyRegion == null)
                {
                    dirtyRegion = new List<Rect>();
                }
                return dirtyRegion;
            }
            set
            {
                dirtyRegion = value;
            }
        }

        static GGeometry()
        {
            InitPolygonProcessorCollection();
        }

        private static void InitPolygonProcessorCollection()
        {
            List<Type> loadedTypes = GCommon.GetAllLoadedTypes();
            for (int i = 0; i < loadedTypes.Count; ++i)
            {
                Type t = loadedTypes[i];
                if (t.GetInterface(PolygonProcessorInterfaceName) != null)
                {
                    PolygonProcessorTypeCollection.Add(t);
                }
            }
        }

        private void Reset()
        {
            name = "Geometry";
            Width = GGriffinSettings.Instance.TerrainDataDefault.Geometry.Width;
            Height = GGriffinSettings.Instance.TerrainDataDefault.Geometry.Height;
            Length = GGriffinSettings.Instance.TerrainDataDefault.Geometry.Length;
            HeightMapResolution = GGriffinSettings.Instance.TerrainDataDefault.Geometry.HeightMapResolution;
            MeshBaseResolution = GGriffinSettings.Instance.TerrainDataDefault.Geometry.MeshBaseResolution;
            MeshResolution = GGriffinSettings.Instance.TerrainDataDefault.Geometry.MeshResolution;
            ChunkGridSize = GGriffinSettings.Instance.TerrainDataDefault.Geometry.ChunkGridSize;
            LODCount = GGriffinSettings.Instance.TerrainDataDefault.Geometry.LODCount;
            DisplacementSeed = GGriffinSettings.Instance.TerrainDataDefault.Geometry.DisplacementSeed;
            DisplacementStrength = GGriffinSettings.Instance.TerrainDataDefault.Geometry.DisplacementStrength;
            PolygonDistribution = GGriffinSettings.Instance.TerrainDataDefault.Geometry.PolygonDistribution;
            PolygonProcessorName = GGriffinSettings.Instance.TerrainDataDefault.Geometry.PolygonProcessorName;
        }

        public void ResetFull()
        {
            Reset();
            GCommon.FillTexture(HeightMap, Color.clear);
            SetRegionDirty(GCommon.UnitRect);
            TerrainData.SetDirty(GTerrainData.DirtyFlags.Geometry);
        }

        private void ResampleHeightMap()
        {
            if (heightMap == null)
                return;
            Texture2D tmp = new Texture2D(HeightMapResolution, HeightMapResolution, HeightMapFormat, false);
            RenderTexture rt = new RenderTexture(HeightMapResolution, HeightMapResolution, 32, HeightMapRTFormat);
            GCommon.CopyToRT(heightMap, rt);
            GCommon.CopyFromRT(tmp, rt);
            rt.Release();
            Object.DestroyImmediate(rt);

            tmp.name = heightMap.name;
            tmp.filterMode = heightMap.filterMode;
            tmp.wrapMode = heightMap.wrapMode;
            Object.DestroyImmediate(heightMap, true);
            heightMap = tmp;
            GCommon.TryAddObjectToAsset(heightMap, TerrainData);

            Internal_CreateNewSubDivisionMap();
            SetRegionDirty(GCommon.UnitRect);
        }

        private void ReFormatHeightMap()
        {
            if (heightMap == null)
                return;
            if (heightmapVersion < HEIGHT_MAP_VERSION_ENCODE_RG)
            {
                Texture2D tmp = new Texture2D(HeightMapResolution, HeightMapResolution, HeightMapFormat, false);
                RenderTexture rt = new RenderTexture(HeightMapResolution, HeightMapResolution, 32, HeightMapRTFormat);
                Material mat = GInternalMaterials.HeightmapConverterEncodeRGMaterial;
                mat.SetTexture("_MainTex", heightMap);
                GCommon.DrawQuad(rt, GCommon.FullRectUvPoints, mat, 0);
                GCommon.CopyFromRT(tmp, rt);
                rt.Release();
                Object.DestroyImmediate(rt);

                tmp.name = heightMap.name;
                tmp.filterMode = heightMap.filterMode;
                tmp.wrapMode = heightMap.wrapMode;
                Object.DestroyImmediate(heightMap, true);
                heightMap = tmp;
                GCommon.TryAddObjectToAsset(heightMap, TerrainData);

                heightmapVersion = HEIGHT_MAP_VERSION_ENCODE_RG;
                Debug.Log("Polaris V2 auto upgrade: Converted Height Map from RGBAFloat to RGBA32.");
            }
        }

        internal void Internal_CreateNewSubDivisionMap()
        {
            if (subDivisionMap != null)
            {
                if (subDivisionMap.width != GCommon.SUB_DIV_MAP_RESOLUTION ||
                    subDivisionMap.height != GCommon.SUB_DIV_MAP_RESOLUTION)
                    Object.DestroyImmediate(subDivisionMap);
            }

            if (subDivisionMap == null)
            {
                subDivisionMap = new Texture2D(GCommon.SUB_DIV_MAP_RESOLUTION, GCommon.SUB_DIV_MAP_RESOLUTION, TextureFormat.ARGB32, false);
            }

            int resolution = GCommon.SUB_DIV_MAP_RESOLUTION;
            RenderTexture rt = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGB32);
            Material mat = GInternalMaterials.SubDivisionMapMaterial;
            Graphics.Blit(HeightMap, rt, mat);
            GCommon.CopyFromRT(subDivisionMap, rt);
            rt.Release();
            Object.DestroyImmediate(rt);
        }

        internal void Internal_CreateNewSubDivisionMap(Texture altHeightMap)
        {
            if (subDivisionMap != null)
            {
                if (subDivisionMap.width != GCommon.SUB_DIV_MAP_RESOLUTION ||
                    subDivisionMap.height != GCommon.SUB_DIV_MAP_RESOLUTION)
                    Object.DestroyImmediate(subDivisionMap);
            }

            if (subDivisionMap == null)
            {
                subDivisionMap = new Texture2D(GCommon.SUB_DIV_MAP_RESOLUTION, GCommon.SUB_DIV_MAP_RESOLUTION, TextureFormat.ARGB32, false);
            }

            int resolution = GCommon.SUB_DIV_MAP_RESOLUTION;
            RenderTexture rt = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGB32);
            Material mat = GInternalMaterials.SubDivisionMapMaterial;
            Graphics.Blit(altHeightMap, rt, mat);
            GCommon.CopyFromRT(subDivisionMap, rt);
            rt.Release();
            Object.DestroyImmediate(rt);
        }

        public void CleanUp()
        {
            int count = 0;
            List<Vector3Int> keys = TerrainData.GeometryData.GetKeys();
            for (int i = 0; i < keys.Count; ++i)
            {
                bool delete = false;
                try
                {
                    int indexX = keys[i].x;
                    int indexY = keys[i].y;
                    int lod = keys[i].z;
                    if (indexX >= ChunkGridSize || indexY >= ChunkGridSize)
                    {
                        delete = true;
                    }
                    else if (lod >= LODCount)
                    {
                        delete = true;
                    }
                    else
                    {
                        delete = false;
                    }
                }
                catch
                {
                    delete = false;
                }

                if (delete)
                {
                    count += 1;
                    TerrainData.GeometryData.DeleteMesh(keys[i]);
                }
            }

            if (count > 0)
            {
                Debug.Log(string.Format("Deleted {0} object{1} from generated data!", count, count > 1 ? "s" : ""));
            }
        }

        public void SetRegionDirty(Rect uvRect)
        {
            DirtyRegion.Add(uvRect);
        }

        public void SetRegionDirty(IEnumerable<Rect> uvRects)
        {
            DirtyRegion.AddRange(uvRects);
        }

        public Rect[] GetDirtyRegions()
        {
            return DirtyRegion.ToArray();
        }

        public void ClearDirtyRegions()
        {
            DirtyRegion.Clear();
        }

        public void CopyTo(GGeometry des)
        {
            des.Width = Width;
            des.Height = Height;
            des.Length = Length;
            des.HeightMapResolution = HeightMapResolution;
            des.MeshBaseResolution = MeshBaseResolution;
            des.MeshResolution = MeshResolution;
            des.ChunkGridSize = ChunkGridSize;
            des.LODCount = LODCount;
            des.DisplacementSeed = DisplacementSeed;
            des.DisplacementStrength = DisplacementStrength;
            des.PolygonProcessorName = PolygonProcessorName;
        }

        public Vector4 GetDecodedHeightMapSample(Vector2 uv)
        {
            Vector4 c = HeightMap.GetPixelBilinear(uv.x, uv.y);
            Vector2 encodedHeight = new Vector2(c.x, c.y);
            float decodedHeight = GCommon.DecodeTerrainHeight(encodedHeight);
            c.x = decodedHeight;
            c.y = decodedHeight;
            return c;
        }
    }
}
