using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Griffin
{
    [System.Serializable]
    public class GGrassPatch
    {
        [SerializeField]
        private GFoliage foliage;
        public GFoliage Foliage
        {
            get
            {
                return foliage;
            }
            private set
            {
                foliage = value;
            }
        }

        [SerializeField]
        private Vector2 index;
        public Vector2 Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }

        [SerializeField]
        private List<GGrassInstance> instances;
        public List<GGrassInstance> Instances
        {
            get
            {
                if (instances == null)
                    instances = new List<GGrassInstance>();
                return instances;
            }
            set
            {
                instances = value;
            }
        }

        private bool requireFullUpdate;
        internal bool RequireFullUpdate
        {
            get
            {
                return requireFullUpdate;
            }
            set
            {
                requireFullUpdate = value;
            }
        }

        internal GGrassPatch(GFoliage owner, int indexX, int indexY)
        {
            foliage = owner;
            Index = new Vector2(indexX, indexY);
        }

        public Rect GetUvRange()
        {
            return GCommon.GetUvRange(foliage.PatchGridSize, (int)Index.x, (int)Index.y);
        }

        public void UpdateMeshes()
        {
            if (Foliage.Grasses == null)
                return;
            List<GGrassPrototype> prototypes = Foliage.Grasses.Prototypes;
            for (int i = 0; i < prototypes.Count; ++i)
            {
                UpdateMesh(i);
            }

            RequireFullUpdate = false;
        }

        public void UpdateMesh(int prototypeIndex)
        {
            if (Foliage.Grasses == null)
                return;
            if (prototypeIndex < 0 || prototypeIndex >= Foliage.Grasses.Prototypes.Count)
                return;

            StripInstances(prototypeIndex);
            //string key = GetPatchMeshName(Index, prototypeIndex);
            Vector3Int key = new Vector3Int((int)Index.x, (int)Index.y, prototypeIndex);
            Mesh m = Foliage.TerrainData.FoliageData.GetMesh(key);
            if (m != null && Foliage.TerrainData.Foliage.GrassInstanceCount == 0)
            {
                Foliage.TerrainData.FoliageData.DeleteMesh(key);
                return;
            }
            else if (m == null)
            {
                m = new Mesh();
                m.MarkDynamic();
                m.name = GetPatchMeshName(Index, prototypeIndex);
                Foliage.TerrainData.FoliageData.SetMesh(key, m);
            }

            GGrassPrototype prototype = Foliage.Grasses.Prototypes[prototypeIndex];
            Mesh baseMesh = prototype.GetBaseMesh();
            Vector3 terrainSize = new Vector3(
                Foliage.TerrainData.Geometry.Width,
                Foliage.TerrainData.Geometry.Height,
                Foliage.TerrainData.Geometry.Length);

            GCombineInfo meshTemplate = new GCombineInfo(baseMesh);

            List<Matrix4x4> transforms = new List<Matrix4x4>();
            Rect uvRange = GetUvRange();
            for (int i = 0; i < Instances.Count; ++i)
            {
                GGrassInstance grass = Instances[i];
                if (grass.PrototypeIndex != prototypeIndex)
                    continue;
                if (!uvRange.Contains(new Vector2(grass.Position.x, grass.Position.z)))
                    continue;

                Matrix4x4 t = Matrix4x4.TRS(
                    new Vector3(grass.Position.x * terrainSize.x, grass.Position.y * terrainSize.y + prototype.PivotOffset, grass.Position.z * terrainSize.z),
                    grass.Rotation,
                    new Vector3(prototype.Size.x * grass.Scale.x, prototype.Size.y * grass.Scale.y, prototype.Size.z * grass.Scale.z));
                transforms.Add(t);
            }

            int vertexCount = meshTemplate.Vertices.Length * transforms.Count;
            if (vertexCount > 65000)
            {
                string warning = string.Format("Failed to batch grass meshes at patch {0}, prototypeIndex {1} due to vertices limit (batch: {2}, limit: 65000). Consider removing some instances or increase Patch Grid Size and try again!", Index.ToString("0"), prototypeIndex, vertexCount);
                Debug.LogWarning(warning);
            }

            GGrassPrototype p = Foliage.Grasses.Prototypes[prototypeIndex];
            bool isDetailObject = p.Shape == GGrassShape.DetailObject;

            GJobSystem.RunOnBackground(() =>
            {
                GCombineInfo result = GCombiner.Combine(meshTemplate, transforms);
                System.Action task = () =>
                {
                    if (result.Vertices.Length == 0)
                    {
                        Foliage.TerrainData.FoliageData.DeleteMesh(key);
                    }
                    else
                    {
                        m.Clear();
                        m.vertices = result.Vertices;
                        m.uv = result.UVs;
                        m.triangles = result.Triangles;
                        m.RecalculateBounds();
                        if (isDetailObject)
                        {
                            m.colors32 = result.Colors;
                            m.RecalculateNormals();
                            m.RecalculateTangents();
                        }
                        else
                        {
                            //Reduce mesh storage space
                            m.colors32 = null;
                            m.normals = null;
                            m.tangents = null;
                            m.uv2 = null;
                        }

                        GCommon.SetDirty(m);
                        //System.GC.Collect(); ==>This line will cause the editor to hang soooooo long!
                    }
                };

                if (result.Vertices.Length < 1000)
                {
                    GJobSystem.RunOnMainThread(task);
                }
                else
                {
                    GJobSystem.ScheduleOnMainThread(task, 0);
                }
            });
        }

        public Mesh GetMesh(int prototypeIndex)
        {
            //string key = GetPatchMeshName(Index, prototypeIndex);
            Vector3Int key = new Vector3Int((int)Index.x, (int)Index.y, prototypeIndex);
            return Foliage.TerrainData.FoliageData.GetMesh(key);
        }

        public static string GetPatchMeshName(Vector2 index, int prototypeIndex)
        {
            return string.Format("~GrassPatch_{0}_{1}_{2}", (int)index.x, (int)index.y, prototypeIndex);
        }

        private void StripInstances()
        {
            if (Foliage.Grasses == null)
                return;
            List<GGrassPrototype> prototypes = Foliage.Grasses.Prototypes;
            for (int i = 0; i < prototypes.Count; ++i)
            {
                StripInstances(i);
            }
        }

        private void StripInstances(int prototypeIndex)
        {
            if (Foliage.Grasses == null)
                return;
            int count = 0;
            for (int i = 0; i < Instances.Count; ++i)
            {
                if (Instances[i].PrototypeIndex == prototypeIndex)
                {
                    count += 1;
                }
            }

            if (count == 0)
                return;

            Mesh baseMesh = Foliage.Grasses.Prototypes[prototypeIndex].GetBaseMesh();
            if (baseMesh == null)
                return;

            int baseVertexCount = baseMesh.vertexCount;
            int vertexCount = baseVertexCount * count;
            if (vertexCount < 65000)
                return;

            int toRemoveCount = 1 + (vertexCount - 65000) / baseVertexCount;
            List<int> indices = new List<int>();
            for (int i = 0; i < Instances.Count; ++i)
            {
                if (Instances[i].PrototypeIndex == prototypeIndex)
                {
                    indices.Add(i);
                }
            }

            GUtilities.ShuffleList(indices);
            for (int i = 0; i < toRemoveCount; ++i)
            {
                GGrassInstance instance = Instances[indices[i]];
                instance.PrototypeIndex = -1;
                Instances[indices[i]] = instance;
            }

            int removedCount = Instances.RemoveAll(g => g.PrototypeIndex < 0);
            Debug.Log(string.Format(
                "Stripped {0} instance{1} at patch {2}, prototype {3}.",
                removedCount,
                removedCount > 1 ? "s" : "",
                Index.ToString("0"),
                prototypeIndex));
        }
    }
}
