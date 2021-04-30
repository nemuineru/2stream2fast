using UnityEngine;
using System.Collections.Generic;
using Action = System.Action;
using System.Threading;
using Rand = System.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.Griffin
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(LODGroup))]
    public class GTerrainChunk : MonoBehaviour
    {
        [SerializeField]
        private GStylizedTerrain terrain;
        public GStylizedTerrain Terrain
        {
            get
            {
                return terrain;
            }
            internal set
            {
                terrain = value;
            }
        }

        [SerializeField]
        private MeshFilter meshFilterComponent;
        public MeshFilter MeshFilterComponent
        {
            get
            {
                if (meshFilterComponent == null)
                {
                    meshFilterComponent = GetComponent<MeshFilter>();
                }
                return meshFilterComponent;
            }
        }

        [SerializeField]
        private MeshRenderer meshRendererComponent;
        public MeshRenderer MeshRendererComponent
        {
            get
            {
                if (meshRendererComponent == null)
                {
                    meshRendererComponent = GetComponent<MeshRenderer>();
                }
                return meshRendererComponent;
            }
        }

        [SerializeField]
        private MeshCollider meshColliderComponent;
        public MeshCollider MeshColliderComponent
        {
            get
            {
                if (meshColliderComponent == null)
                {
                    meshColliderComponent = GetComponent<MeshCollider>();
                }
                return meshColliderComponent;
            }
        }

        [SerializeField]
        private LODGroup lodGroupComponent;
        public LODGroup LodGroupComponent
        {
            get
            {
                if (lodGroupComponent == null)
                {
                    lodGroupComponent = GetComponent<LODGroup>();
                }
                return lodGroupComponent;
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
            internal set
            {
                index = value;
            }
        }

        [SerializeField]
        private GTerrainChunkLOD[] chunkLowerLOD;
        private GTerrainChunkLOD[] ChunkLowerLOD
        {
            get
            {
                if (chunkLowerLOD == null || chunkLowerLOD.Length != Terrain.TerrainData.Geometry.LODCount - 1)
                {
                    chunkLowerLOD = new GTerrainChunkLOD[Terrain.TerrainData.Geometry.LODCount - 1];
                    GUtilities.ClearChildren(transform);
                }
                return chunkLowerLOD;
            }
        }

        private volatile GSubDivisionTree subDivTree;
        private volatile Queue<Action> mainThreadJobs;

#if UNITY_EDITOR
        private Thread stitchSeamThread;
        private int stitchSeamThreadSleepTime = 1000;
        private volatile bool canRunStitchSeamInBackground = false;
#endif

        [SerializeField]
        private GTerrainChunk[] neighborChunks;
        internal GTerrainChunk[] Internal_NeighborChunks
        {
            get
            {
                if (neighborChunks == null || neighborChunks.Length != 4)
                {
                    neighborChunks = new GTerrainChunk[4];
                }
                return neighborChunks;
            }
            set
            {
                neighborChunks = value;
            }
        }

        //triangle edge length in UV space by subdiv levels
        private static double[] triangleEdgeLength;
        private static double[] TriangleEdgeLength
        {
            get
            {
                if (triangleEdgeLength == null)
                {
                    int count = 15;
                    triangleEdgeLength = new double[count];
                    triangleEdgeLength[0] = 1;
                    for (int i = 1; i < count; ++i)
                    {
                        triangleEdgeLength[i] = triangleEdgeLength[i - 1] / Mathf.Sqrt(2);
                    }
                }
                return triangleEdgeLength;
            }
        }

        private List<Vector2> generatedUv;
        private List<Vector2> GeneratedUv
        {
            get
            {
                if (generatedUv == null)
                {
                    generatedUv = new List<Vector2>();
                }
                return generatedUv;
            }
        }

        private List<Vector3> generatedVertices;
        private List<Vector3> GeneratedVertices
        {
            get
            {
                if (generatedVertices == null)
                {
                    generatedVertices = new List<Vector3>();
                }
                return generatedVertices;
            }
        }

        private List<Color32> generatedVertexColors;
        private List<Color32> GeneratedVertexColors
        {
            get
            {
                if (generatedVertexColors == null)
                {
                    generatedVertexColors = new List<Color32>();
                }
                return generatedVertexColors;
            }
        }

        private List<int> generatedTriangles;
        private List<int> GeneratedTriangles
        {
            get
            {
                if (generatedTriangles == null)
                {
                    generatedTriangles = new List<int>();
                }
                return generatedTriangles;
            }
        }

        private int[] indexGrid;

        private System.DateTime lastUpdatedTime;
        public System.DateTime LastUpdatedTime
        {
            get
            {
                return lastUpdatedTime;
            }
            private set
            {
                lastUpdatedTime = value;
            }
        }

        public Matrix4x4 LocalToTerrainMatrix
        {
            get
            {
                return transform.localToWorldMatrix * Terrain.transform.worldToLocalMatrix;
            }
        }

        public Rect GetUvRange()
        {
            if (Terrain == null || Terrain.TerrainData == null)
            {
                return GCommon.UnitRect;
            }
            else
            {
                int gridSize = Terrain.TerrainData.Geometry.ChunkGridSize;
                Vector2 position = index / gridSize;
                Vector2 size = Vector2.one / gridSize;
                return new Rect(position, size);
            }
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.update += Update;
            canRunStitchSeamInBackground = false;
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= Update;
            canRunStitchSeamInBackground = false;
#endif
        }

        private void Update()
        {
            lock (this)
            {
                if (mainThreadJobs != null && mainThreadJobs.Count > 0)
                {
                    Action a = mainThreadJobs.Dequeue();
                    a.Invoke();
                }
#if UNITY_EDITOR
                //ManageSeamStitchingInBackground();
#endif
            }
        }

        private void ManageSeamStitchingInBackground()
        {
#if UNITY_EDITOR
            if (Terrain.TerrainData != null && Terrain.TerrainData.Geometry != null)
            {
                canRunStitchSeamInBackground = true;
            }
            else
            {
                canRunStitchSeamInBackground = false;
            }

            if (canRunStitchSeamInBackground)
            {
                if (stitchSeamThread == null || !stitchSeamThread.IsAlive)
                {
                    if (subDivTree == null)
                        Internal_CreateSubDivTree();
                    stitchSeamThread = new Thread(StitchSeamInBackground);
                    stitchSeamThread.Start();
                }
            }
#endif
        }

        private void StitchSeamInBackground()
        {
#if UNITY_EDITOR
            while (canRunStitchSeamInBackground)
            {
                Thread.Sleep(stitchSeamThreadSleepTime + new System.Random().Next(1000));
                if (subDivTree == null)
                    continue;
                try
                {
                    HashSet<Vector2> vertexPool = new HashSet<Vector2>();
                    subDivTree.ForEachLeaf(n =>
                    {
                        vertexPool.Add(n.V0);
                        vertexPool.Add(n.V1);
                        vertexPool.Add(n.V2);
                    });

                    for (int i = 0; i < Internal_NeighborChunks.Length; ++i)
                    {
                        if (Internal_NeighborChunks[i] == null)
                            continue;
                        GSubDivisionTree tree = Internal_NeighborChunks[i].subDivTree;
                        if (tree == null)
                            continue;
                        tree.ForEachLeaf(n =>
                        {
                            vertexPool.Add(n.V0);
                            vertexPool.Add(n.V1);
                            vertexPool.Add(n.V2);
                        });
                    }

                    bool isNewVertCreated = Internal_StitchGeometrySeamsOnSubDivTree(vertexPool);
                    if (isNewVertCreated)
                    {
                        EnqueueMainThreadJob(() =>
                        {
                            Internal_UpdateMeshLOD0();
                            Internal_UpdateLODsAsync(vertexPool);
                        });
                    }
                }
                catch (System.Exception)
                {
                    //Debug.Log(e.ToString());
                }
            }
#endif
        }

        internal void Internal_CreateSubDivTree()
        {
            Rect uvRange = GetUvRange();
            subDivTree = GSubDivisionTree.Rect(uvRange);

            int firstPassResolution = Terrain.TerrainData.Geometry.PolygonDistribution == GPolygonDistributionMode.Dynamic ?
                Terrain.TerrainData.Geometry.MeshBaseResolution :
                Terrain.TerrainData.Geometry.MeshResolution;
            //Subdiv for base resolution
            for (int i = 0; i < firstPassResolution; ++i)
            {
                subDivTree.ForEachLeaf(n =>
                {
                    n.Split();
                });
            }

            if (Terrain.TerrainData.Geometry.PolygonDistribution != GPolygonDistributionMode.Dynamic)
                return;

            Texture2D subdivMap = null;
            if (GCommon.IsMainThread)
            {
                subdivMap = Terrain.TerrainData.Geometry.Internal_SubDivisionMap;
            }
            GGeometryGenerationContext context = null;
            if (!GCommon.IsMainThread)
            {
                context = Terrain.GenerationContext;
            }

            Stack<GSubDivisionTree.Node> nodes0 = new Stack<GSubDivisionTree.Node>();
            Stack<GSubDivisionTree.Node> nodes1;
            subDivTree.ForEachLeaf(n =>
            {
                nodes0.Push(n);
            });

            //Additional sub division based on the sub div map
            int baseResolution = Terrain.TerrainData.Geometry.MeshBaseResolution;
            int resolution = Terrain.TerrainData.Geometry.MeshResolution;
            int maxLevel = baseResolution + Mathf.Min(Mathf.FloorToInt(1f / GCommon.SUB_DIV_STEP), resolution - baseResolution);

            float r0 = 0;
            float r1 = 0;
            float r2 = 0;
            float rc = 0;
            float rMax = 0;
            float subDivLevel;
            for (int i = baseResolution; i <= resolution; ++i)
            {
                nodes1 = new Stack<GSubDivisionTree.Node>();
                while (nodes0.Count > 0)
                {
                    GSubDivisionTree.Node n = nodes0.Pop();
                    if (n.Level >= maxLevel)
                        continue;
                    if (GCommon.IsMainThread)
                    {
                        r0 = subdivMap.GetPixelBilinear(n.V0.x, n.V0.y).r;
                        r1 = subdivMap.GetPixelBilinear(n.V1.x, n.V1.y).r;
                        r2 = subdivMap.GetPixelBilinear(n.V2.x, n.V2.y).r;
                        rc = subdivMap.GetPixelBilinear(n.VC.x, n.VC.y).r;
                    }
                    else
                    {
                        r0 = context.GetSubdivData(n.V0).r;
                        r1 = context.GetSubdivData(n.V1).r;
                        r2 = context.GetSubdivData(n.V2).r;
                        rc = context.GetSubdivData(n.VC).r;
                    }
                    rMax = Mathf.Max(r0, r1, r2, rc);

                    subDivLevel = baseResolution + (int)(rMax / GCommon.SUB_DIV_STEP);
                    if (subDivLevel <= n.Level)
                        continue;
                    n.Split();
                    nodes1.Push(n.LeftNode);
                    nodes1.Push(n.RightNode);
                }

                nodes0 = nodes1;
            }
        }

        private void UpdateMesh(Mesh m, GSubDivisionTree tree)
        {
            CreatePolygons(tree);
            m.Clear();
            m.SetVertices(GeneratedVertices);
            m.SetTriangles(GeneratedTriangles, 0);
            m.SetUVs(0, GeneratedUv);
            m.SetColors(GeneratedVertexColors);
            m.RecalculateBounds();
            m.RecalculateNormals();
            RecalculateTangentIfNeeded(m);

            GeneratedVertices.Clear();
            GeneratedTriangles.Clear();
            GeneratedVertexColors.Clear();
            GeneratedUv.Clear();

            SetLastUpdatedTimeNow();
        }

        private void RecalculateTangentIfNeeded(Mesh m)
        {
            if (m == null)
                return;
            if (Terrain.TerrainData.Shading.IsMaterialUseNormalMap())
            {
                m.RecalculateTangents();
            }
            else
            {
                m.tangents = null;
            }
        }

        private void CreatePolygons(GSubDivisionTree tree)
        {
            int dispSeed = Terrain.TerrainData.Geometry.DisplacementSeed;
            float dispStrength = Terrain.TerrainData.Geometry.DisplacementStrength;
            Texture2D subdivMap = Terrain.TerrainData.Geometry.Internal_SubDivisionMap;
            Vector2 uv0, uv1, uv2;
            Vector3 v0, v1, v2;
            float r;
            Rand rand;
            double radius;

            int baseResolution = Terrain.TerrainData.Geometry.MeshBaseResolution;
            int resolution = Terrain.TerrainData.Geometry.MeshResolution;
            int level;
            double triangleBaseLength = 1f / Terrain.TerrainData.Geometry.ChunkGridSize;

            IGPolygonProcessor pp = Terrain.TerrainData.Geometry.PolygonProcessor;
            GPolygon polygon = new GPolygon();
            GeneratedUv.Clear();
            GeneratedVertices.Clear();
            GeneratedVertexColors.Clear();
            GeneratedTriangles.Clear();

            Vector3 terrainSize = new Vector3(
                Terrain.TerrainData.Geometry.Width,
                Terrain.TerrainData.Geometry.Height,
                Terrain.TerrainData.Geometry.Length);

            v0 = Vector3.zero;
            v1 = Vector3.zero;
            v2 = Vector3.zero;

            tree.ForEachLeaf((n) =>
            {
                uv0 = n.V0;
                uv1 = n.V1;
                uv2 = n.V2;
                if (dispStrength > 0)
                {
                    if (uv0.x != 0 && uv0.x != 1 && uv0.y != 0 && uv0.y != 1)
                    {
                        r = subdivMap.GetPixelBilinear(uv0.x, uv0.y).r;
                        level = baseResolution + Mathf.Min(Mathf.FloorToInt(r / GCommon.SUB_DIV_STEP), resolution - baseResolution);
                        rand = new Rand(dispSeed + (int)(uv0.x * 1000 + uv0.y * 1000));
                        radius = 0.35 * dispStrength * triangleBaseLength * TriangleEdgeLength[level];
                        uv0.x = Mathf.Clamp01(uv0.x + (float)((rand.NextDouble() - 0.5) * radius));
                        uv0.y = Mathf.Clamp01(uv0.y + (float)((rand.NextDouble() - 0.5) * radius));
                    }

                    if (uv1.x != 0 && uv1.x != 1 && uv1.y != 0 && uv1.y != 1)
                    {
                        r = subdivMap.GetPixelBilinear(uv1.x, uv1.y).r;
                        level = baseResolution + Mathf.Min(Mathf.FloorToInt(r / GCommon.SUB_DIV_STEP), resolution - baseResolution);
                        rand = new Rand(dispSeed + (int)(uv1.x * 1000 + uv1.y * 1000));
                        radius = 0.35 * dispStrength * triangleBaseLength * TriangleEdgeLength[level];
                        uv1.x = Mathf.Clamp01(uv1.x + (float)((rand.NextDouble() - 0.5) * radius));
                        uv1.y = Mathf.Clamp01(uv1.y + (float)((rand.NextDouble() - 0.5) * radius));
                    }

                    if (uv2.x != 0 && uv2.x != 1 && uv2.y != 0 && uv2.y != 1)
                    {
                        r = subdivMap.GetPixelBilinear(uv2.x, uv2.y).r;
                        level = baseResolution + Mathf.Min(Mathf.FloorToInt(r / GCommon.SUB_DIV_STEP), resolution - baseResolution);
                        rand = new Rand(dispSeed + (int)(uv2.x * 1000 + uv2.y * 1000));
                        radius = 0.35 * dispStrength * triangleBaseLength * TriangleEdgeLength[level];
                        uv2.x = Mathf.Clamp01(uv2.x + (float)((rand.NextDouble() - 0.5) * radius));
                        uv2.y = Mathf.Clamp01(uv2.y + (float)((rand.NextDouble() - 0.5) * radius));
                    }
                }

                Vector4 h0 = Terrain.GetInterpolatedHeightMapSample(uv0);
                Vector4 h1 = Terrain.GetInterpolatedHeightMapSample(uv1);
                Vector4 h2 = Terrain.GetInterpolatedHeightMapSample(uv2);
                if (h0.w >= 0.5f || h1.w >= 0.5f || h2.w >= 0.5f) //alpha channel for visibility
                    return;

                if (pp != null)
                {
                    polygon.Clear();
                    v0.Set(uv0.x * terrainSize.x, h0.x * terrainSize.y, uv0.y * terrainSize.z);
                    v1.Set(uv1.x * terrainSize.x, h1.x * terrainSize.y, uv1.y * terrainSize.z);
                    v2.Set(uv2.x * terrainSize.x, h2.x * terrainSize.y, uv2.y * terrainSize.z);
                    polygon.Vertices.Add(v0);
                    polygon.Vertices.Add(v1);
                    polygon.Vertices.Add(v2);
                    polygon.Uvs.Add(uv0);
                    polygon.Uvs.Add(uv1);
                    polygon.Uvs.Add(uv2);
                    polygon.Triangles.Add(0);
                    polygon.Triangles.Add(1);
                    polygon.Triangles.Add(2);
                    pp.Process(this, ref polygon);

                    int currentTrisIndex = GeneratedVertices.Count;
                    for (int i = 0; i < polygon.Triangles.Count; ++i)
                    {
                        GeneratedTriangles.Add(currentTrisIndex + polygon.Triangles[i]);
                    }

                    for (int i = 0; i < polygon.Vertices.Count; ++i)
                    {
                        GeneratedVertices.Add(polygon.Vertices[i]);
                    }

                    for (int i = 0; i < polygon.Uvs.Count; ++i)
                    {
                        GeneratedUv.Add(polygon.Uvs[i]);
                    }

                    if (polygon.VertexColors != null && polygon.VertexColors.Count > 0)
                    {
                        for (int i = 0; i < polygon.VertexColors.Count; ++i)
                        {
                            GeneratedVertexColors.Add(polygon.VertexColors[i]);
                        }
                    }
                }
                else
                {
                    int currentTrisIndex = GeneratedVertices.Count;
                    GeneratedTriangles.Add(currentTrisIndex + 0);
                    GeneratedTriangles.Add(currentTrisIndex + 1);
                    GeneratedTriangles.Add(currentTrisIndex + 2);
                    v0.Set(uv0.x * terrainSize.x, h0.x * terrainSize.y, uv0.y * terrainSize.z);
                    v1.Set(uv1.x * terrainSize.x, h1.x * terrainSize.y, uv1.y * terrainSize.z);
                    v2.Set(uv2.x * terrainSize.x, h2.x * terrainSize.y, uv2.y * terrainSize.z);
                    GeneratedVertices.Add(v0);
                    GeneratedVertices.Add(v1);
                    GeneratedVertices.Add(v2);
                    GeneratedUv.Add(uv0);
                    GeneratedUv.Add(uv1);
                    GeneratedUv.Add(uv2);
                }
            });

            //Convert vertices terrain local space to chunk local space.
            //This way we can place the chunk pivot/origin point at the center of its region instead of the terrain pivot.
            //Chunk position is set by the terrain component.
            //This step is very important for other task such as level streaming and occlusion culling, etc.
            //Special thank to Aleš Stupka for the contribution.
            Matrix4x4 vertexTransformMatrix = Terrain.transform.localToWorldMatrix * transform.worldToLocalMatrix;
            for (int i = 0; i < GeneratedVertices.Count; ++i)
            {
                GeneratedVertices[i] = vertexTransformMatrix.MultiplyPoint(GeneratedVertices[i]);
            }
        }

        internal void Internal_UpdateMeshLOD0()
        {
            Mesh meshLod0 = GetMesh(0);
            UpdateMesh(meshLod0, subDivTree);
            MeshFilterComponent.sharedMesh = meshLod0;
            MeshColliderComponent.sharedMesh = meshLod0;
            GCommon.SetDirty(meshLod0);
        }

        internal void Internal_UpdateRenderer()
        {
            MeshRendererComponent.shadowCastingMode = Terrain.TerrainData.Rendering.CastShadow ?
                UnityEngine.Rendering.ShadowCastingMode.On :
                UnityEngine.Rendering.ShadowCastingMode.Off;
            MeshRendererComponent.receiveShadows = Terrain.TerrainData.Rendering.ReceiveShadow;

            for (int i = 1; i < Terrain.TerrainData.Geometry.LODCount; ++i)
            {
                GTerrainChunkLOD chunkLod = GetChunkLOD(i);
                chunkLod.MeshRendererComponent.shadowCastingMode = MeshRendererComponent.shadowCastingMode;
                chunkLod.MeshRendererComponent.receiveShadows = MeshRendererComponent.receiveShadows;
                chunkLod.MeshRendererComponent.sharedMaterials = MeshRendererComponent.sharedMaterials;
            }
            SetLastUpdatedTimeNow();
        }

        internal void Internal_UpdateMaterial()
        {
            MeshRendererComponent.sharedMaterials = new Material[] { Terrain.TerrainData.Shading.CustomMaterial };

            for (int i = 1; i < Terrain.TerrainData.Geometry.LODCount; ++i)
            {
                GTerrainChunkLOD chunkLod = GetChunkLOD(i);
                chunkLod.MeshRendererComponent.sharedMaterials = MeshRendererComponent.sharedMaterials;
            }
            SetLastUpdatedTimeNow();
        }

        public void SetupLocalToTerrainMatrix(MaterialPropertyBlock props)
        {
            string localToTerrainPropName = "_LocalToTerrainMatrix";
            props.SetMatrix(localToTerrainPropName, LocalToTerrainMatrix);
        }

        public IEnumerable<Vector2> FlattenSubDivTree()
        {
            List<Vector2> verts = new List<Vector2>();

            if (subDivTree == null)
            {
                Internal_CreateSubDivTree();
            }

            subDivTree.ForEachLeaf(n =>
            {
                verts.Add(n.V0);
                verts.Add(n.V1);
                verts.Add(n.V2);
            });

            return verts;
        }

        private bool StitchGeometrySeamsOnSubDivTree(HashSet<Vector2> vertexPool, GSubDivisionTree tree)
        {
#if UNITY_EDITOR
            //canRunStitchSeamInBackground = true;
#endif

            bool isNewVertexCreated = false;
            Stack<GSubDivisionTree.Node> nodes = new Stack<GSubDivisionTree.Node>();

            int baseResolution = Terrain.TerrainData.Geometry.MeshBaseResolution;
            int resolution = Terrain.TerrainData.Geometry.MeshResolution;
            int maxLevel = baseResolution + Mathf.Min(Mathf.FloorToInt(1f / GCommon.SUB_DIV_STEP), resolution - baseResolution);
            //Stitching triangle between tiles            
            tree.ForEachLeaf(n =>
            {
                if (n.Level >= maxLevel)
                    return;
                nodes.Push(n);
            });

            while (nodes.Count > 0)
            {
                GSubDivisionTree.Node n = nodes.Pop();

                bool validV01 = vertexPool.Contains(n.V01);// && GUtilities.IsRectContainPointExclusive(uvRange, n.V01);
                bool validV12 = vertexPool.Contains(n.V12);// && GUtilities.IsRectContainPointExclusive(uvRange, n.V12);
                bool validV20 = vertexPool.Contains(n.V20);// && GUtilities.IsRectContainPointExclusive(uvRange, n.V20);

                if (!validV01 && !validV12 && !validV20)
                    continue;
                n.Split();
                if (!vertexPool.Contains(n.V12))
                {
                    vertexPool.Add(n.V12);
                    isNewVertexCreated = true;
                }
                nodes.Push(n.LeftNode);
                nodes.Push(n.RightNode);
            }

            //Sub division for triangle connectivity
            HashSet<Vector2> verts = new HashSet<Vector2>();
            nodes.Clear();
            tree.ForEachLeaf(n =>
            {
                verts.Add(n.V0);
                verts.Add(n.V1);
                verts.Add(n.V2);
                if (n.Level < maxLevel)
                {
                    nodes.Push(n);
                }
            });

            while (nodes.Count > 0)
            {
                GSubDivisionTree.Node n = nodes.Pop();
                if (!verts.Contains(n.V01) && !verts.Contains(n.V12) && !verts.Contains(n.V20))
                    continue;
                n.Split();
                if (!verts.Contains(n.V12))
                {
                    verts.Add(n.V12);
                    isNewVertexCreated = true;
                }
                nodes.Push(n.LeftNode);
                nodes.Push(n.RightNode);
            }

            return isNewVertexCreated;
        }

        internal bool Internal_StitchGeometrySeamsOnSubDivTree(HashSet<Vector2> vertexPool)
        {
            return StitchGeometrySeamsOnSubDivTree(vertexPool, subDivTree);
        }

        internal void Internal_UpdateLODsAsync(HashSet<Vector2> vertexPool)
        {
            LOD lod0 = new LOD(0, new Renderer[] { MeshRendererComponent });
            LodGroupComponent.SetLODs(new LOD[] { lod0 });
            if (Terrain.TerrainData.Geometry.LODCount == 1)
            {
                GUtilities.ClearChildren(transform);
            }

            for (int i = 1; i < Terrain.TerrainData.Geometry.LODCount; ++i)
            {
                GTerrainChunkLOD chunkLod = GetChunkLOD(i);
                chunkLod.MeshRendererComponent.enabled = false;
            }

            int meshResolution = 0;
            Rect uvRange = GetUvRange();
            HashSet<Vector2> seamVertices = new HashSet<Vector2>();
            subDivTree.ForEachLeaf(n =>
            {
                if (!GUtilities.IsRectContainPointExclusive(uvRange, n.V0))
                    seamVertices.Add(n.V0);
                if (!GUtilities.IsRectContainPointExclusive(uvRange, n.V1))
                    seamVertices.Add(n.V1);
                if (!GUtilities.IsRectContainPointExclusive(uvRange, n.V2))
                    seamVertices.Add(n.V2);
                meshResolution = Mathf.Max(meshResolution, n.Level);
            });

            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    GSubDivisionTree[] lodTrees = new GSubDivisionTree[Terrain.TerrainData.Geometry.LODCount];
                    for (int level = 1; level < Terrain.TerrainData.Geometry.LODCount; ++level)
                    {
                        int i = level;
                        int baseResolution = Terrain.TerrainData.Geometry.PolygonDistribution == GPolygonDistributionMode.Dynamic ?
                            Terrain.TerrainData.Geometry.MeshBaseResolution :
                            0;
                        int targetResolution = Mathf.Max(baseResolution, meshResolution - i);
                        lodTrees[i] = subDivTree.Clone(targetResolution);
                        StitchSeamLOD(lodTrees[i], seamVertices);
                        //StitchGeometrySeamsOnSubDivTree(vertexPool, lodTrees[i]);
                        EnqueueMainThreadJob(() =>
                        {
                            Mesh meshLod = GetMesh(i);
                            UpdateMesh(meshLod, lodTrees[i]);
                            GTerrainChunkLOD chunkLod = GetChunkLOD(i);
                            chunkLod.MeshFilterComponent.sharedMesh = meshLod;
                        });
                    }

                    EnqueueMainThreadJob(() =>
                    {
                        float transitionStep = 1.0f / Terrain.TerrainData.Geometry.LODCount;

                        LOD[] lods = new LOD[Terrain.TerrainData.Geometry.LODCount];
                        lods[0] = new LOD(
                            GGriffinSettings.Instance.LodTransition.Evaluate(transitionStep),
                            new Renderer[] { MeshRendererComponent });

                        for (int level = 1; level < Terrain.TerrainData.Geometry.LODCount; ++level)
                        {
                            int i = level;
                            lods[i] = new LOD(
                                GGriffinSettings.Instance.LodTransition.Evaluate((i + 1) * transitionStep),
                                new Renderer[] { GetChunkLOD(i).MeshRendererComponent });

                            GTerrainChunkLOD chunkLod = GetChunkLOD(i);
                            chunkLod.MeshRendererComponent.enabled = true;
                        }

                        LodGroupComponent.SetLODs(lods);
                        Internal_UpdateRenderer();
                    });
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.ToString());
                }
            });

        }

        private void StitchSeamLOD(GSubDivisionTree tree, HashSet<Vector2> seamVertices)
        {
            Stack<GSubDivisionTree.Node> dirtyNode = new Stack<GSubDivisionTree.Node>();
            tree.ForEachLeaf(n =>
            {
                dirtyNode.Push(n);
            });

            HashSet<Vector2> newVertices = new HashSet<Vector2>();
            while (dirtyNode.Count > 0)
            {
                GSubDivisionTree.Node n = dirtyNode.Pop();
                if (!seamVertices.Contains(n.V01) && !seamVertices.Contains(n.V12) && !seamVertices.Contains(n.V20))
                    continue;

                n.Split();
                newVertices.Add(n.V12);
                dirtyNode.Push(n.LeftNode);
                dirtyNode.Push(n.RightNode);
            }

            dirtyNode.Clear();
            tree.ForEachLeaf(n =>
            {
                dirtyNode.Push(n);
            });
            while (dirtyNode.Count > 0)
            {
                GSubDivisionTree.Node n = dirtyNode.Pop();
                if (!newVertices.Contains(n.V01) && !newVertices.Contains(n.V12) && !newVertices.Contains(n.V20))
                    continue;

                n.Split();
                dirtyNode.Push(n.LeftNode);
                dirtyNode.Push(n.RightNode);
            }
        }

        private void EnqueueMainThreadJob(Action a)
        {
            if (mainThreadJobs == null)
                mainThreadJobs = new Queue<Action>();
            mainThreadJobs.Enqueue(a);
        }

        private GTerrainChunkLOD GetChunkLOD(int level)
        {
            if (level <= 0 || level >= Terrain.TerrainData.Geometry.LODCount)
                throw new System.IndexOutOfRangeException();
            GTerrainChunkLOD c = ChunkLowerLOD[level - 1];
            if (c == null)
            {
                Transform t = GUtilities.GetChildrenWithName(transform, name + "_LOD" + level);
                c = t.gameObject.AddComponent<GTerrainChunkLOD>();
                ChunkLowerLOD[level - 1] = c;
            }
            return c;
        }

        private Mesh GetMesh(int lod)
        {
            //string key = GetChunkMeshName(Index, lod);
            Vector3Int key = new Vector3Int((int)Index.x, (int)Index.y, lod);
            Mesh m = Terrain.TerrainData.GeometryData.GetMesh(key);
            if (m == null)
            {
                m = new Mesh();
                m.name = GetChunkMeshName(Index, lod);
                m.MarkDynamic();
                Terrain.TerrainData.GeometryData.SetMesh(key, m);
            }
            return m;
        }

        public static string GetChunkMeshName(Vector2 index, int lod)
        {
            return string.Format("{0}_{1}_{2}_{3}", GCommon.CHUNK_MESH_NAME_PREFIX, (int)index.x, (int)index.y, lod);
        }

        public void Refresh()
        {
            Mesh lod0 = GetMesh(0);
            MeshFilterComponent.sharedMesh = lod0;
            MeshColliderComponent.sharedMesh = lod0;

            for (int i = 1; i < Terrain.TerrainData.Geometry.LODCount; ++i)
            {
                Mesh lodi = GetMesh(i);
                GTerrainChunkLOD chunkLodi = GetChunkLOD(i);
                chunkLodi.MeshFilterComponent.sharedMesh = lodi;
            }
            SetLastUpdatedTimeNow();
        }

        private void SetLastUpdatedTimeNow()
        {
            LastUpdatedTime = System.DateTime.Now;
        }
    }
}
