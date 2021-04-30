using UnityEngine;

namespace Pinwheel.Griffin.GroupTool
{
    [System.Serializable]
    public struct GGeometryOverride
    {
        [SerializeField]
        private bool overrideWidth;
        public bool OverrideWidth
        {
            get
            {
                return overrideWidth;
            }
            set
            {
                overrideWidth = value;
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
        private bool overrideHeight;
        public bool OverrideHeight
        {
            get
            {
                return overrideHeight;
            }
            set
            {
                overrideHeight = value;
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
        private bool overrideLength;
        public bool OverrideLength
        {
            get
            {
                return overrideLength;
            }
            set
            {
                overrideLength = value;
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
        private bool overrideHeightMapResolution;
        public bool OverrideHeightMapResolution
        {
            get
            {
                return overrideHeightMapResolution;
            }
            set
            {
                overrideHeightMapResolution = value;
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
                heightMapResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(value), GCommon.TEXTURE_SIZE_MIN, GCommon.TEXTURE_SIZE_MAX);
            }
        }

        [SerializeField]
        private bool overrideMeshBaseResolution;
        public bool OverrideMeshBaseResolution
        {
            get
            {
                return overrideMeshBaseResolution;
            }
            set
            {
                overrideMeshBaseResolution = value;
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
        private bool overrideMeshResolution;
        public bool OverrideMeshResolution
        {
            get
            {
                return overrideMeshResolution;
            }
            set
            {
                overrideMeshResolution = value;
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
        private bool overrideChunkGridSize;
        public bool OverrideChunkGridSize
        {
            get
            {
                return overrideChunkGridSize;
            }
            set
            {
                overrideChunkGridSize = value;
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
        private bool overrideLodCount;
        public bool OverrideLodCount
        {
            get
            {
                return overrideLodCount;
            }
            set
            {
                overrideLodCount = value;
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
        private bool overrideDisplacementSeed;
        public bool OverrideDisplacementSeed
        {
            get
            {
                return overrideDisplacementSeed;
            }
            set
            {
                overrideDisplacementSeed = value;
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
        private bool overrideDisplacementStrength;
        public bool OverrideDisplacementStrength
        {
            get
            {
                return overrideDisplacementStrength;
            }
            set
            {
                overrideDisplacementStrength = value;
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
        private bool overridePolygonDistribution;
        public bool OverridePolygonDistribution
        {
            get
            {
                return overridePolygonDistribution;
            }
            set
            {
                overridePolygonDistribution = value;
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
        private bool overrideTriangulatorName;
        public bool OverridePolygonProcessorName
        {
            get
            {
                return overrideTriangulatorName;
            }
            set
            {
                overrideTriangulatorName = value;
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

        public void Reset()
        {
            OverrideWidth = false;
            OverrideHeight = false;
            OverrideLength = false;
            OverrideHeightMapResolution = false;
            OverrideMeshBaseResolution = false;
            OverrideMeshResolution = false;
            OverrideChunkGridSize = false;
            overrideLodCount = false;
            OverrideDisplacementSeed = false;
            OverrideDisplacementStrength = false;
            OverridePolygonDistribution = false;
            OverridePolygonProcessorName = false;

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

        public void Override(GGeometry g)
        {
            if (OverrideWidth)
                g.Width = Width;
            if (OverrideHeight)
                g.Height = Height;
            if (OverrideLength)
                g.Length = Length;
            if (OverrideHeightMapResolution)
                g.HeightMapResolution = HeightMapResolution;
            if (OverrideMeshBaseResolution)
                g.MeshBaseResolution = MeshBaseResolution;
            if (OverrideMeshResolution)
                g.MeshResolution = MeshResolution;
            if (OverrideChunkGridSize)
                g.ChunkGridSize = ChunkGridSize;
            if (OverrideLodCount)
                g.LODCount = LODCount;
            if (OverrideDisplacementSeed)
                g.DisplacementSeed = DisplacementSeed;
            if (OverrideDisplacementStrength)
                g.DisplacementStrength = DisplacementStrength;
            if (OverridePolygonDistribution)
                g.PolygonDistribution = PolygonDistribution;
            if (OverridePolygonProcessorName)
                g.PolygonProcessorName = PolygonProcessorName;
        }
    }
}
