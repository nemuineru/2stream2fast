using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Griffin
{
    public class GFoliage : ScriptableObject
    {
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
        private GTreePrototypeGroup trees;
        public GTreePrototypeGroup Trees
        {
            get
            {
                return trees;
            }
            set
            {
                trees = value;
            }
        }

        [SerializeField]
        private List<GTreeInstance> treeInstances;
        public List<GTreeInstance> TreeInstances
        {
            get
            {
                if (treeInstances == null)
                    treeInstances = new List<GTreeInstance>();
                return treeInstances;
            }
            set
            {
                treeInstances = value;
            }
        }

        [SerializeField]
        private GSnapMode treeSnapMode;
        public GSnapMode TreeSnapMode
        {
            get
            {
                return treeSnapMode;
            }
            set
            {
                treeSnapMode = value;
            }
        }

        [SerializeField]
        private LayerMask treeSnapLayerMask;
        public LayerMask TreeSnapLayerMask
        {
            get
            {
                return treeSnapLayerMask;
            }
            set
            {
                treeSnapLayerMask = value;
            }
        }

        [SerializeField]
        private GGrassPrototypeGroup grasses;
        public GGrassPrototypeGroup Grasses
        {
            get
            {
                return grasses;
            }
            set
            {
                grasses = value;
            }
        }

        [SerializeField]
        private GSnapMode grassSnapMode;
        public GSnapMode GrassSnapMode
        {
            get
            {
                return grassSnapMode;
            }
            set
            {
                grassSnapMode = value;
            }
        }

        [SerializeField]
        private LayerMask grassSnapLayerMask;
        public LayerMask GrassSnapLayerMask
        {
            get
            {
                return grassSnapLayerMask;
            }
            set
            {
                grassSnapLayerMask = value;
            }
        }

        [SerializeField]
        private int patchGridSize;
        public int PatchGridSize
        {
            get
            {
                return patchGridSize;
            }
            set
            {
                patchGridSize = Mathf.Max(1, value);
            }
        }

        [SerializeField]
        private GGrassPatch[] grassPatches;
        public GGrassPatch[] GrassPatches
        {
            get
            {
                if (grassPatches == null)
                {
                    grassPatches = new GGrassPatch[PatchGridSize * PatchGridSize];
                    for (int x = 0; x < PatchGridSize; ++x)
                    {
                        for (int z = 0; z < patchGridSize; ++z)
                        {
                            GGrassPatch patch = new GGrassPatch(this, x, z);
                            grassPatches[GUtilities.To1DIndex(x, z, PatchGridSize)] = patch;
                        }
                    }
                }
                if (grassPatches.Length != PatchGridSize * PatchGridSize)
                {
                    ResampleGrassPatches();
                }
                return grassPatches;
            }
        }

        private List<Rect> treeDirtyRegions;
        private List<Rect> TreeDirtyRegions
        {
            get
            {
                if (treeDirtyRegions == null)
                {
                    treeDirtyRegions = new List<Rect>();
                }
                return treeDirtyRegions;
            }
            set
            {
                treeDirtyRegions = value;
            }
        }

        private List<Rect> grassDirtyRegions;
        private List<Rect> GrassDirtyRegions
        {
            get
            {
                if (grassDirtyRegions == null)
                {
                    grassDirtyRegions = new List<Rect>();
                }
                return grassDirtyRegions;
            }
            set
            {
                grassDirtyRegions = value;
            }
        }

        [SerializeField]
        private float estimatedGrassStorageMB;
        public float EstimatedGrassStorageMB
        {
            get
            {
                return estimatedGrassStorageMB;
            }
            private set
            {
                estimatedGrassStorageMB = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private bool enableInteractiveGrass;
        public bool EnableInteractiveGrass
        {
            get
            {
                return enableInteractiveGrass;
            }
            set
            {
                enableInteractiveGrass = value;
            }
        }

        [SerializeField]
        private int vectorFieldMapResolution;
        public int VectorFieldMapResolution
        {
            get
            {
                return vectorFieldMapResolution;
            }
            set
            {
                vectorFieldMapResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(value), GCommon.TEXTURE_SIZE_MIN, GCommon.TEXTURE_SIZE_MAX);
            }
        }

        [SerializeField]
        private float bendSensitive;
        public float BendSensitive
        {
            get
            {
                return bendSensitive;
            }
            set
            {
                bendSensitive = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float restoreSensitive;
        public float RestoreSensitive
        {
            get
            {
                return restoreSensitive;
            }
            set
            {
                restoreSensitive = Mathf.Clamp01(value);
            }
        }

        public int GrassInstanceCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < GrassPatches.Length; ++i)
                {
                    count += GrassPatches[i].Instances.Count;
                }
                return count;
            }
        }

        private void Reset()
        {
            name = "Foliage";
            Trees = GGriffinSettings.Instance.TerrainDataDefault.Foliage.Trees;
            TreeSnapMode = GGriffinSettings.Instance.TerrainDataDefault.Foliage.TreeSnapMode;
            TreeSnapLayerMask = GGriffinSettings.Instance.TerrainDataDefault.Foliage.TreeSnapLayerMask;
            TreeInstances = null;
            Grasses = GGriffinSettings.Instance.TerrainDataDefault.Foliage.Grasses;
            GrassSnapMode = GGriffinSettings.Instance.TerrainDataDefault.Foliage.GrassSnapMode;
            GrassSnapLayerMask = GGriffinSettings.Instance.TerrainDataDefault.Foliage.GrassSnapLayerMask;
            PatchGridSize = GGriffinSettings.Instance.TerrainDataDefault.Foliage.PatchGridSize;
            EnableInteractiveGrass = GGriffinSettings.Instance.TerrainDataDefault.Foliage.EnableInteractiveGrass;
            VectorFieldMapResolution = GGriffinSettings.Instance.TerrainDataDefault.Foliage.VectorFieldMapResolution;
            BendSensitive = GGriffinSettings.Instance.TerrainDataDefault.Foliage.BendSensitive;
            RestoreSensitive = GGriffinSettings.Instance.TerrainDataDefault.Foliage.RestoreSensitive;
            ClearGrassInstances();
        }

        public void ResetFull()
        {
            Reset();
            for (int i = 0; i < GrassPatches.Length; ++i)
            {
                GrassPatches[i].UpdateMeshes();
            }
            CalculateEstimatedGrassStorage();
        }

        public void Refresh()
        {
            if (Trees != null)
            {
                List<GTreePrototype> prototypes = Trees.Prototypes;
                for (int i = 0; i < prototypes.Count; ++i)
                {
                    prototypes[i].Refresh();
                }
                TreeInstances.RemoveAll(t => t.PrototypeIndex < 0 || t.PrototypeIndex >= Trees.Prototypes.Count);
            }
            if (Grasses != null)
            {
                for (int i = 0; i < GrassPatches.Length; ++i)
                {
                    GrassPatches[i].Instances.RemoveAll(g => g.PrototypeIndex < 0 || g.PrototypeIndex >= Grasses.Prototypes.Count);
                }
            }

            CalculateEstimatedGrassStorage();
        }

        private void ResampleGrassPatches()
        {
            List<GGrassInstance> grassInstances = new List<GGrassInstance>();
            for (int i = 0; i < grassPatches.Length; ++i)
            {
                grassInstances.AddRange(grassPatches[i].Instances);
            }

            grassPatches = new GGrassPatch[PatchGridSize * PatchGridSize];
            for (int x = 0; x < PatchGridSize; ++x)
            {
                for (int z = 0; z < patchGridSize; ++z)
                {
                    int index = GUtilities.To1DIndex(x, z, PatchGridSize);
                    GGrassPatch patch = new GGrassPatch(this, x, z);
                    grassPatches[index] = patch;
                }
            }

            AddGrassInstances(grassInstances);
        }

        public void AddGrassInstances(List<GGrassInstance> instances)
        {
            Rect[] uvRects = new Rect[GrassPatches.Length];
            for (int r = 0; r < uvRects.Length; ++r)
            {
                uvRects[r] = GrassPatches[r].GetUvRange();
            }

            for (int i = 0; i < instances.Count; ++i)
            {
                GGrassInstance grass = instances[i];
                for (int r = 0; r < uvRects.Length; ++r)
                {
                    if (uvRects[r].Contains(new Vector2(grass.Position.x, grass.Position.z)))
                    {
                        grassPatches[r].Instances.Add(grass);
                        break;
                    }
                }
            }
            CalculateEstimatedGrassStorage();
        }

        public List<GGrassInstance> GetGrassInstances()
        {
            List<GGrassInstance> instances = new List<GGrassInstance>();
            for (int i = 0; i < GrassPatches.Length; ++i)
            {
                instances.AddRange(GrassPatches[i].Instances);
            }
            return instances;
        }

        public void ClearGrassInstances()
        {
            for (int i = 0; i < GrassPatches.Length; ++i)
            {
                GrassPatches[i].Instances.Clear();
            }
            CalculateEstimatedGrassStorage();
        }

        public void CleanUp()
        {
            int count = 0;
            List<Vector3Int> keys = TerrainData.FoliageData.GetKeys();
            for (int i = 0; i < keys.Count; ++i)
            {
                bool delete = false;
                try
                {
                    int indexX = keys[i].x;
                    int indexY = keys[i].y;
                    int prototype = keys[i].z;
                    if (indexX >= PatchGridSize || indexY >= PatchGridSize)
                    {
                        delete = true;
                    }
                    else if (Grasses == null || prototype >= Grasses.Prototypes.Count)
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
                    TerrainData.FoliageData.DeleteMesh(keys[i]);
                }
            }

            if (count > 0)
            {
                Debug.Log(string.Format("Deleted {0} object{1} from generated data!", count, count > 1 ? "s" : ""));
            }
        }

        public void SetTreeRegionDirty(Rect uvRect)
        {
            TreeDirtyRegions.Add(uvRect);
        }

        public void SetTreeRegionDirty(IEnumerable<Rect> uvRects)
        {
            TreeDirtyRegions.AddRange(uvRects);
        }

        public Rect[] GetTreeDirtyRegions()
        {
            return TreeDirtyRegions.ToArray();
        }

        public void ClearTreeDirtyRegions()
        {
            TreeDirtyRegions.Clear();
        }

        public void SetGrassRegionDirty(Rect uvRect)
        {
            GrassDirtyRegions.Add(uvRect);
        }

        public void SetGrassRegionDirty(IEnumerable<Rect> uvRects)
        {
            GrassDirtyRegions.AddRange(uvRects);
        }

        public Rect[] GetGrassDirtyRegions()
        {
            return GrassDirtyRegions.ToArray();
        }

        public void ClearGrassDirtyRegions()
        {
            GrassDirtyRegions.Clear();
        }

        private void CalculateEstimatedGrassStorage()
        {
#if UNITY_EDITOR
            if (Grasses == null)
            {
                EstimatedGrassStorageMB = 0;
                return;
            }

            long byteCount = 0;
            List<GGrassPrototype> prototypes = Grasses.Prototypes;
            int[] vertexCountPerPrototypes = new int[prototypes.Count];
            uint[] indexCountPerPrototypes = new uint[prototypes.Count];
            for (int i = 0; i < prototypes.Count; ++i)
            {
                Mesh m = prototypes[i].GetBaseMesh();
                if (m == null)
                {
                    vertexCountPerPrototypes[i] = 0;
                    indexCountPerPrototypes[i] = 0;
                }
                else
                {
                    vertexCountPerPrototypes[i] = m.vertexCount;
                    indexCountPerPrototypes[i] = m.GetIndexCount(0);
                }
            }

            int indexSize = sizeof(ushort);
            int vertexSize = sizeof(float) * 3 + sizeof(float) * 2 + sizeof(byte) * 4; //pos + uv + color32 

            List<GGrassInstance> instances = GetGrassInstances();
            for (int i = 0; i < instances.Count; ++i)
            {
                if (instances[i].PrototypeIndex < 0 || instances[i].PrototypeIndex >= prototypes.Count)
                    continue;
                int protoIndex = instances[i].PrototypeIndex;
                Mesh m = prototypes[protoIndex].GetBaseMesh();
                if (m == null)
                    continue;
                byteCount += 2 * (vertexCountPerPrototypes[protoIndex] * vertexSize + indexCountPerPrototypes[protoIndex] * indexSize);
            }

            EstimatedGrassStorageMB = byteCount / 1000000;
#endif
        }

        public void CopyTo(GFoliage des)
        {
            des.Trees = Trees;
            des.TreeSnapMode = TreeSnapMode;
            des.TreeSnapLayerMask = TreeSnapLayerMask;
            des.Grasses = Grasses;
            des.GrassSnapMode = GrassSnapMode;
            des.GrassSnapLayerMask = GrassSnapLayerMask;
            des.PatchGridSize = PatchGridSize;
        }
    }
}
