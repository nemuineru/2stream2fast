using UnityEngine;
using System.Collections.Generic;
using Pinwheel.Griffin.TextureTool;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if __MICROSPLAT_POLARIS__
using JBooth.MicroSplat;
#endif

namespace Pinwheel.Griffin
{
    [System.Serializable]
    [ExecuteInEditMode]
    public class GStylizedTerrain : MonoBehaviour
    {
        public delegate void HeightMapProcessCallback(Texture2D heightmap);
        public event HeightMapProcessCallback PreProcessHeightMap;
        public event HeightMapProcessCallback PostProcessHeightMap;

        private static HashSet<GStylizedTerrain> activeTerrainSet;
        private static HashSet<GStylizedTerrain> ActiveTerrainSet
        {
            get
            {
                if (activeTerrainSet == null)
                    activeTerrainSet = new HashSet<GStylizedTerrain>();
                return activeTerrainSet;
            }
        }

        public static IEnumerable<GStylizedTerrain> ActiveTerrains
        {
            get
            {
                return ActiveTerrainSet;
            }
        }

        [SerializeField]
        private GTerrainData terrainData;
        public GTerrainData TerrainData
        {
            get
            {
                return terrainData;
            }
            set
            {
                GTerrainData oldData = terrainData;
                GTerrainData newData = value;
                terrainData = newData;

                if (oldData == null && newData != null)
                {
                    newData.Dirty += OnTerrainDataDirty;
                    newData.Geometry.SetRegionDirty(GCommon.UnitRect);
                    newData.Foliage.SetTreeRegionDirty(GCommon.UnitRect);
                    newData.Foliage.SetGrassRegionDirty(GCommon.UnitRect);
                    OnTerrainDataDirty(GTerrainData.DirtyFlags.All);
                }
                else if (oldData != null && newData != null && oldData != newData)
                {
                    oldData.Dirty -= OnTerrainDataDirty;
                    newData.Dirty += OnTerrainDataDirty;
                    newData.Geometry.SetRegionDirty(GCommon.UnitRect);
                    newData.Foliage.SetTreeRegionDirty(GCommon.UnitRect);
                    newData.Foliage.SetGrassRegionDirty(GCommon.UnitRect);
                    OnTerrainDataDirty(GTerrainData.DirtyFlags.All);
                }
                else if (oldData != null && newData == null)
                {
                    oldData.Dirty -= OnTerrainDataDirty;
                    GUtilities.DestroyGameobject(Internal_ChunkRoot.gameObject);
                }
            }
        }

        [SerializeField]
        private GStylizedTerrain topNeighbor;
        public GStylizedTerrain TopNeighbor
        {
            get
            {
                return topNeighbor;
            }
            set
            {
                topNeighbor = value;
            }
        }

        [SerializeField]
        private GStylizedTerrain bottomNeighbor;
        public GStylizedTerrain BottomNeighbor
        {
            get
            {
                return bottomNeighbor;
            }
            set
            {
                bottomNeighbor = value;
            }
        }

        [SerializeField]
        private GStylizedTerrain leftNeighbor;
        public GStylizedTerrain LeftNeighbor
        {
            get
            {
                return leftNeighbor;
            }
            set
            {
                leftNeighbor = value;
            }
        }

        [SerializeField]
        private GStylizedTerrain rightNeighbor;
        public GStylizedTerrain RightNeighbor
        {
            get
            {
                return rightNeighbor;
            }
            set
            {
                rightNeighbor = value;
            }
        }

        [SerializeField]
        private int groupId;
        public int GroupId
        {
            get
            {
                return groupId;
            }
            set
            {
                groupId = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private bool autoConnect = true;
        public bool AutoConnect
        {
            get
            {
                return autoConnect;
            }
            set
            {
                autoConnect = value;
            }
        }

        [SerializeField]
        public float geometryVersion;
        public const float GEOMETRY_VERSION_CHUNK_POSITION_AT_CHUNK_CENTER = 245;

        public Bounds Bounds
        {
            get
            {
                Bounds b = new Bounds();
                b.size = TerrainData != null ?
                    new Vector3(TerrainData.Geometry.Width, TerrainData.Geometry.Height, TerrainData.Geometry.Length) :
                    Vector3.zero;
                b.center = transform.position + b.size * 0.5f;
                return b;
            }
        }

        private Texture2D heightMap2D;
        private RenderTexture heightMap;
        private RenderTexture normalMap;
        private RenderTexture normalMapInterpolated;
        private RenderTexture normalMapPerPixel;
        private RenderTexture grassVectorFieldMap;
        private RenderTexture grassVectorFieldMapTmp;

        internal GGeometryGenerationContext GenerationContext { get; private set; }

        public bool Internal_IsGeneratingInBackground { get; private set; }

        public Transform Internal_ChunkRoot
        {
            get
            {
                Transform root = transform.Find(GCommon.CHUNK_ROOT_NAME_OBSOLETED);
                if (root == null)
                {
                    root = GUtilities.GetChildrenWithName(transform, GCommon.CHUNK_ROOT_NAME);
                }
                else
                {
                    root.gameObject.name = GCommon.CHUNK_ROOT_NAME;
                }

                root.hideFlags = GGriffinSettings.Instance.ShowGeometryChunksInHierarchy ? HideFlags.None : HideFlags.HideInHierarchy;
#if UNITY_EDITOR
                StaticEditorFlags staticFlags = GameObjectUtility.GetStaticEditorFlags(root.gameObject);
                GameObjectUtility.SetStaticEditorFlags(root.gameObject, staticFlags | StaticEditorFlags.NavigationStatic);
#endif
                return root;
            }
        }

        public static void ConnectAdjacentTiles()
        {
            IEnumerator<GStylizedTerrain> terrains = ActiveTerrains.GetEnumerator();
            while (terrains.MoveNext())
            {
                GStylizedTerrain t = terrains.Current;
                t.ConnectNeighbor();
            }
        }

        public void ConnectNeighbor()
        {
            if (TerrainData == null)
                return;
            if (!AutoConnect)
                return;
            Vector2 sizeXZ = new Vector2(
                TerrainData.Geometry.Width,
                TerrainData.Geometry.Length);
            Vector2 posXZ = new Vector2(
                transform.position.x,
                transform.position.z);

            IEnumerator<GStylizedTerrain> terrains = ActiveTerrains.GetEnumerator();
            while (terrains.MoveNext())
            {
                GStylizedTerrain t = terrains.Current;
                if (t.TerrainData == null)
                    return;
                if (GroupId != t.GroupId)
                    continue;

                Vector2 neighborSizeXZ = new Vector2(
                    t.TerrainData.Geometry.Width,
                    t.TerrainData.Geometry.Length);
                Vector2 neighborPosXZ = new Vector2(
                    t.transform.position.x,
                    t.transform.position.z);
                Vector2 neighborCenter = neighborPosXZ + neighborSizeXZ * 0.5f;

                if (LeftNeighbor == null)
                {
                    Rect r = new Rect();
                    r.size = sizeXZ;
                    r.position = new Vector2(posXZ.x - sizeXZ.x, posXZ.y);
                    if (r.Contains(neighborCenter))
                    {
                        LeftNeighbor = t;
                        t.RightNeighbor = this;
                    }
                }
                if (TopNeighbor == null)
                {
                    Rect r = new Rect();
                    r.size = sizeXZ;
                    r.position = new Vector2(posXZ.x, posXZ.y + sizeXZ.y);
                    if (r.Contains(neighborCenter))
                    {
                        TopNeighbor = t;
                        t.BottomNeighbor = this;
                    }
                }
                if (RightNeighbor == null)
                {
                    Rect r = new Rect();
                    r.size = sizeXZ;
                    r.position = new Vector2(posXZ.x + sizeXZ.x, posXZ.y);
                    if (r.Contains(neighborCenter))
                    {
                        RightNeighbor = t;
                        t.LeftNeighbor = this;
                    }
                }
                if (BottomNeighbor == null)
                {
                    Rect r = new Rect();
                    r.size = sizeXZ;
                    r.position = new Vector2(posXZ.x, posXZ.y - sizeXZ.y);
                    if (r.Contains(neighborCenter))
                    {
                        BottomNeighbor = t;
                        t.TopNeighbor = this;
                    }
                }
            }
        }

        private void OnEnable()
        {
            ActiveTerrainSet.Add(this);
            GStylizedTerrain.ConnectAdjacentTiles();

            GCommon.RegisterBeginRender(OnBeginCameraRendering);
            GCommon.RegisterBeginRenderSRP(OnBeginCameraRenderingSRP);
            GCommon.RegisterEndRender(OnEndCameraRendering);

#if __MICROSPLAT_POLARIS__
            PullMaterialAndSplatMapsFromMicroSplat();
            PushControlTexturesToMicroSplat();
#endif

            if (TerrainData != null)
            {
                TerrainData.Dirty += OnTerrainDataDirty;
                TerrainData.SetDirty(GTerrainData.DirtyFlags.Shading);
            }

            if (TerrainData != null)
            {
                TerrainData.Shading.UpdateMaterials();
            }
        }

        private void OnDisable()
        {
            ReleaseTemporaryTextures();
            ActiveTerrainSet.Remove(this);
            GStylizedTerrain.ConnectAdjacentTiles();

            GCommon.UnregisterBeginRender(OnBeginCameraRendering);
            GCommon.UnregisterBeginRenderSRP(OnBeginCameraRenderingSRP);
            GCommon.UnregisterEndRender(OnEndCameraRendering);

            if (TerrainData != null)
            {
                TerrainData.Dirty -= OnTerrainDataDirty;
            }
        }

        private void OnDestroy()
        {
            ReleaseTemporaryTextures();
        }

        private void Reset()
        {
            geometryVersion = GVersionInfo.Number;
        }

        private void ReleaseTemporaryTextures()
        {
            if (heightMap != null)
            {
                heightMap.Release();
                GUtilities.DestroyObject(heightMap);
            }
            if (heightMap2D != null)
            {
                GUtilities.DestroyObject(heightMap2D);
            }
            if (normalMap != null)
            {
                normalMap.Release();
                GUtilities.DestroyObject(normalMap);
            }
            if (normalMapInterpolated != null)
            {
                normalMapInterpolated.Release();
                GUtilities.DestroyObject(normalMapInterpolated);
            }
            if (normalMapPerPixel != null)
            {
                normalMapPerPixel.Release();
                GUtilities.DestroyObject(normalMapPerPixel);
            }
        }

        private void OnBeginCameraRendering(Camera cam)
        {
            if (GUtilities.IsSceneViewOrGameCamera(cam))
            {
                GFoliageRenderer.Render(this, cam);
            }

#if UNITY_EDITOR
            if (GGriffinSettings.Instance.DebugMode)
            {
                DrawChunkUpdateDebug(cam);
            }
#endif
        }

        private void OnBeginCameraRenderingSRP(ScriptableRenderContext context, Camera cam)
        {
            OnBeginCameraRendering(cam);
        }

        private void OnEndCameraRendering(Camera cam)
        {
            GFoliageRenderer.RenderPreview(this, cam);
        }

        private void OnTerrainDataDirty(GTerrainData.DirtyFlags dirtyFlag)
        {
            if ((dirtyFlag & GTerrainData.DirtyFlags.Geometry) == GTerrainData.DirtyFlags.Geometry)
            {
                OnGeometryDirty();
            }
            else if ((dirtyFlag & GTerrainData.DirtyFlags.GeometryAsync) == GTerrainData.DirtyFlags.GeometryAsync)
            {
                OnGeometryAsyncDirty();
            }

            if ((dirtyFlag & GTerrainData.DirtyFlags.Shading) == GTerrainData.DirtyFlags.Shading)
            {
                OnShadingDirty();
            }

            if ((dirtyFlag & GTerrainData.DirtyFlags.Rendering) == GTerrainData.DirtyFlags.Rendering)
            {
                OnRenderingDirty();
            }

            if ((dirtyFlag & GTerrainData.DirtyFlags.Foliage) == GTerrainData.DirtyFlags.Foliage)
            {
                OnFoliageDirty();
            }
            GUtilities.MarkCurrentSceneDirty();
        }

        private void OnGeometryDirty()
        {
            geometryVersion = GVersionInfo.Number;

            if (PreProcessHeightMap != null)
                PreProcessHeightMap.Invoke(TerrainData.Geometry.HeightMap);
            try
            {
                bool useDynamicPolygon = TerrainData.Geometry.PolygonDistribution == GPolygonDistributionMode.Dynamic;

                InitChunks();
                TerrainData.Geometry.Internal_CreateNewSubDivisionMap();

                GTerrainChunk[] chunks = GetChunks();
                List<GTerrainChunk> chunksToUpdate = new List<GTerrainChunk>();
                Rect[] dirtyRegion = TerrainData.Geometry.GetDirtyRegions();
                for (int i = 0; i < chunks.Length; ++i)
                {
                    Rect uvRect = chunks[i].GetUvRange();
                    for (int j = 0; j < dirtyRegion.Length; ++j)
                    {
                        if (uvRect.Overlaps(dirtyRegion[j]))
                        {
                            chunksToUpdate.Add(chunks[i]);
                            break;
                        }
                    }
                }

                for (int i = 0; i < chunksToUpdate.Count; ++i)
                {
                    chunksToUpdate[i].Internal_CreateSubDivTree();
                }

                HashSet<Vector2> vertexPool = new HashSet<Vector2>();
                if (useDynamicPolygon)
                {
                    HashSet<GTerrainChunk> chunkFlags = new HashSet<GTerrainChunk>();
                    for (int i = 0; i < chunksToUpdate.Count; ++i)
                    {
                        for (int n = 0; n < chunksToUpdate[i].Internal_NeighborChunks.Length; ++n)
                        {
                            GTerrainChunk neighbor = chunksToUpdate[i].Internal_NeighborChunks[n];
                            if (neighbor != null && !chunkFlags.Contains(neighbor))
                            {
                                vertexPool.UnionWith(neighbor.FlattenSubDivTree());
                                chunkFlags.Add(neighbor);
                            }
                        }
                    }
                    GetNeighborEdgeVertices(vertexPool);

                    bool isNewVertexCreated = true;
                    for (int i = 0; i < GGriffinSettings.Instance.TriangulateIteration; ++i)
                    {
                        isNewVertexCreated = false;
                        for (int cIndex = 0; cIndex < chunksToUpdate.Count; ++cIndex)
                        {
                            isNewVertexCreated = isNewVertexCreated || chunksToUpdate[cIndex].Internal_StitchGeometrySeamsOnSubDivTree(vertexPool);
                        }

                        if (!isNewVertexCreated)
                            break;
                    }
                }

                for (int i = 0; i < chunksToUpdate.Count; ++i)
                {
                    chunksToUpdate[i].Internal_UpdateMeshLOD0();
                    chunksToUpdate[i].Internal_UpdateLODsAsync(vertexPool);
                }

                ReleaseTemporaryTextures();
                TerrainData.Geometry.ClearDirtyRegions();
                OnShadingDirty();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
            }

            if (PostProcessHeightMap != null)
            {
                PostProcessHeightMap.Invoke(TerrainData.Geometry.HeightMap);
            }
        }

        private void OnGeometryAsyncDirty()
        {
            geometryVersion = GVersionInfo.Number;

            if (PreProcessHeightMap != null)
                PreProcessHeightMap.Invoke(TerrainData.Geometry.HeightMap);

            try
            {
                bool useDynamicPolygon = TerrainData.Geometry.PolygonDistribution == GPolygonDistributionMode.Dynamic;

                InitChunks();
                if (useDynamicPolygon)
                {
                    TerrainData.Geometry.Internal_CreateNewSubDivisionMap();
                    CreateGenerationContext();
                }

                GTerrainChunk[] chunks = GetChunks();
                List<GTerrainChunk> chunksToUpdate = new List<GTerrainChunk>();
                Rect[] dirtyRegion = TerrainData.Geometry.GetDirtyRegions();
                for (int i = 0; i < chunks.Length; ++i)
                {
                    Rect uvRect = chunks[i].GetUvRange();
                    for (int j = 0; j < dirtyRegion.Length; ++j)
                    {
                        if (uvRect.Overlaps(dirtyRegion[j]))
                        {
                            chunksToUpdate.Add(chunks[i]);
                            break;
                        }
                    }
                }

                HashSet<Vector2> vertexPool = new HashSet<Vector2>();
                if (useDynamicPolygon)
                {
                    GetNeighborEdgeVertices(vertexPool);
                }

                Internal_IsGeneratingInBackground = true;
                GJobSystem.RunOnBackground(() =>
                {
                    for (int i = 0; i < chunksToUpdate.Count; ++i)
                    {
                        chunksToUpdate[i].Internal_CreateSubDivTree();
                    }

                    if (useDynamicPolygon)
                    {
                        HashSet<GTerrainChunk> chunkFlags = new HashSet<GTerrainChunk>();
                        for (int i = 0; i < chunksToUpdate.Count; ++i)
                        {
                            for (int n = 0; n < chunksToUpdate[i].Internal_NeighborChunks.Length; ++n)
                            {
                                GTerrainChunk neighbor = chunksToUpdate[i].Internal_NeighborChunks[n];
                                if (neighbor != null && !chunkFlags.Contains(neighbor))
                                {
                                    vertexPool.UnionWith(neighbor.FlattenSubDivTree());
                                    chunkFlags.Add(neighbor);
                                }
                            }
                        }

                        bool isNewVertexCreated = true;
                        for (int i = 0; i < GGriffinSettings.Instance.TriangulateIteration; ++i)
                        {
                            isNewVertexCreated = false;
                            for (int cIndex = 0; cIndex < chunksToUpdate.Count; ++cIndex)
                            {
                                isNewVertexCreated = isNewVertexCreated || chunksToUpdate[cIndex].Internal_StitchGeometrySeamsOnSubDivTree(vertexPool);
                            }

                            if (!isNewVertexCreated)
                                break;
                        }
                    }

                    GJobSystem.RunOnMainThread(() =>
                    {
                        for (int i = 0; i < chunksToUpdate.Count; ++i)
                        {
                            if (chunksToUpdate[i] == null)
                                continue;
                            chunksToUpdate[i].Internal_UpdateMeshLOD0();
                            chunksToUpdate[i].Internal_UpdateLODsAsync(vertexPool);
                        }
                        ReleaseTemporaryTextures();
                        GenerationContext = null;
                        Internal_IsGeneratingInBackground = false;

                        if (PostProcessHeightMap != null)
                            PostProcessHeightMap.Invoke(TerrainData.Geometry.HeightMap);

                        if (TerrainData.Foliage.TreeInstances.Count > 0)
                        {
                            TerrainData.Foliage.SetTreeRegionDirty(TerrainData.Geometry.GetDirtyRegions());
                            UpdateTreesPosition(true);
                            TerrainData.Foliage.ClearTreeDirtyRegions();
                        }
                        if (TerrainData.Foliage.GrassInstanceCount > 0)
                        {
                            TerrainData.Foliage.SetGrassRegionDirty(TerrainData.Geometry.GetDirtyRegions());
                            UpdateGrassPatches(-1, true);
                            TerrainData.Foliage.ClearGrassDirtyRegions();
                        }

                        TerrainData.Geometry.ClearDirtyRegions();
                        OnShadingDirty();
                        System.GC.Collect();
                    });
                });
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                if (PostProcessHeightMap != null)
                    PostProcessHeightMap.Invoke(TerrainData.Geometry.HeightMap);
            }
        }

        private void CreateGenerationContext()
        {
            GenerationContext = new GGeometryGenerationContext(this, TerrainData);
        }

        private void OnRenderingDirty()
        {
            InitChunks();
            GTerrainChunk[] chunks = GetChunks();
            for (int i = 0; i < chunks.Length; ++i)
            {
                chunks[i].Internal_UpdateRenderer();
            }
        }

        private void OnShadingDirty()
        {
#if __MICROSPLAT_POLARIS__
            if (TerrainData.Shading.ShadingSystem == GShadingSystem.MicroSplat)
            {
                PullMaterialAndSplatMapsFromMicroSplat();
                PushControlTexturesToMicroSplat();
            }
#endif

            if (TerrainData.Shading.ShadingSystem != GShadingSystem.Polaris)
                return;
            InitChunks();
            GTerrainChunk[] chunks = GetChunks();
            for (int i = 0; i < chunks.Length; ++i)
            {
                chunks[i].Internal_UpdateMaterial();
            }
        }

#if __MICROSPLAT_POLARIS__
        public void PushControlTexturesToMicroSplat()
        {
            if (TerrainData == null)
                return;
            MicroSplatPolarisMesh pm = gameObject.GetComponent<MicroSplatPolarisMesh>();
            if (pm == null)
                return;
            Texture2D[] controls = new Texture2D[TerrainData.Shading.SplatControlMapCount];
            for (int i = 0; i < controls.Length; ++i)
            {
                controls[i] = TerrainData.Shading.GetSplatControl(i);
            }
            pm.controlTextures = controls;
            pm.Sync();
        }

        public void PullMaterialAndSplatMapsFromMicroSplat()
        {
            if (TerrainData == null)
                return;
            MicroSplatPolarisMesh pm = gameObject.GetComponent<MicroSplatPolarisMesh>();
            if (pm == null)
                return;
            TerrainData.Shading.CustomMaterial = pm.matInstance;

            TextureArrayConfig config = TerrainData.Shading.MicroSplatTextureArrayConfig;
            if (config == null)
                return;
            GSplatPrototypeGroup protoGroup = TerrainData.Shading.Splats;
            if (protoGroup == null)
                return;

            List<TextureArrayConfig.TextureEntry> entries = config.sourceTextures;
            List<GSplatPrototype> prototypes = new List<GSplatPrototype>();
            for (int i = 0; i < entries.Count; ++i)
            {
                GSplatPrototype p = new GSplatPrototype();
                p.Texture = entries[i].diffuse;
                prototypes.Add(p);
            }

            protoGroup.Prototypes = prototypes;
        }
#endif

        private void OnFoliageDirty()
        {
            TerrainData.Foliage.Refresh();
        }

        private void InitChunks()
        {
            GTerrainChunk[] chunks = GetChunks();
            int gridSize = TerrainData.Geometry.ChunkGridSize;
            if (chunks.Length != gridSize * gridSize)
            {
                //GUtilities.DestroyObject(Internal_ChunkRoot.gameObject);
                DestroyImmediate(Internal_ChunkRoot.gameObject);
                for (int z = 0; z < gridSize; ++z)
                {
                    for (int x = 0; x < gridSize; ++x)
                    {
                        Transform chunkTransform = GUtilities.GetChildrenWithName(Internal_ChunkRoot, string.Format("C({0},{1})", x, z));
                        GTerrainChunk chunk = chunkTransform.gameObject.AddComponent<GTerrainChunk>();
                        chunk.Index = new Vector2(x, z);
                        chunk.Terrain = this;
                        chunk.Internal_UpdateRenderer();
                        chunk.Internal_UpdateMaterial();
                    }
                }
            }

            Vector2 chunkPhysicalSize = new Vector2(TerrainData.Geometry.Width, TerrainData.Geometry.Length) / gridSize;
            chunks = GetChunks();
            for (int i = 0; i < chunks.Length; ++i)
            {
                GTerrainChunk currentChunk = chunks[i];
                if (geometryVersion >= GEOMETRY_VERSION_CHUNK_POSITION_AT_CHUNK_CENTER)
                {
                    currentChunk.transform.localPosition = new Vector3((currentChunk.Index.x + 0.5f) * chunkPhysicalSize.x, 0, (currentChunk.Index.y + 0.5f) * chunkPhysicalSize.y);
                }

                GUtilities.Fill(currentChunk.Internal_NeighborChunks, null);
                for (int j = 0; j < chunks.Length; ++j)
                {
                    GTerrainChunk otherChunk = chunks[j];
                    if (otherChunk.Index == currentChunk.Index + Vector2.left)
                    {
                        currentChunk.Internal_NeighborChunks[0] = otherChunk;
                    }
                    if (otherChunk.Index == currentChunk.Index + Vector2.up)
                    {
                        currentChunk.Internal_NeighborChunks[1] = otherChunk;
                    }
                    if (otherChunk.Index == currentChunk.Index + Vector2.right)
                    {
                        currentChunk.Internal_NeighborChunks[2] = otherChunk;
                    }
                    if (otherChunk.Index == currentChunk.Index + Vector2.down)
                    {
                        currentChunk.Internal_NeighborChunks[3] = otherChunk;
                    }
                }
#if UNITY_EDITOR
                StaticEditorFlags staticFlags = GameObjectUtility.GetStaticEditorFlags(currentChunk.gameObject);
                GameObjectUtility.SetStaticEditorFlags(currentChunk.gameObject, staticFlags | StaticEditorFlags.NavigationStatic);
#endif
            }

            GUtilities.MarkCurrentSceneDirty();
        }

        public GTerrainChunk[] GetChunks()
        {
            return Internal_ChunkRoot.GetComponentsInChildren<GTerrainChunk>();
        }

        private List<GTerrainChunk> GetChunksSortedByDistance(Vector3 origin)
        {
            List<GTerrainChunk> chunks = new List<GTerrainChunk>(GetChunks());
            chunks.Sort((c1, c2) =>
            {
                Vector3 center1 = c1.MeshColliderComponent.bounds.center;
                Vector3 center2 = c2.MeshColliderComponent.bounds.center;
                float d1 = Vector3.Distance(origin, center1);
                float d2 = Vector3.Distance(origin, center2);
                return d1.CompareTo(d2);
            });
            return chunks;
        }

        public bool Raycast(Ray ray, out RaycastHit hit, float distance)
        {
            List<GTerrainChunk> chunks = GetChunksSortedByDistance(ray.origin);

            for (int i = 0; i < chunks.Count; ++i)
            {
                if (chunks[i].MeshColliderComponent.Raycast(ray, out hit, distance))
                {
                    return true;
                }
            }

            hit = new RaycastHit();
            return false;
        }

        public bool Raycast(Vector3 normalizePoint, out RaycastHit hit)
        {
            Ray r = new Ray();
            Vector3 origin = NormalizedToWorldPoint(normalizePoint);
            origin.y = 10000;
            r.origin = origin;
            r.direction = Vector3.down;

            return Raycast(r, out hit, float.MaxValue);
        }

        public static bool Raycast(Ray ray, out RaycastHit hit, float distance, int groupId)
        {
            List<RaycastHit> hitInfo = new List<RaycastHit>();
            IEnumerator<GStylizedTerrain> terrain = ActiveTerrains.GetEnumerator();
            while (terrain.MoveNext())
            {
                if (terrain.Current.GroupId != groupId && groupId >= 0)
                    continue;
                RaycastHit h;
                if (terrain.Current.Raycast(ray, out h, distance))
                {
                    hitInfo.Add(h);
                }
            }
            if (hitInfo.Count == 0)
            {
                hit = new RaycastHit();
                return false;
            }
            else
            {
                hitInfo.Sort((h0, h1) =>
                    Vector3.SqrMagnitude(h0.point - ray.origin)
                    .CompareTo(Vector3.SqrMagnitude(h1.point - ray.origin)));
                hit = hitInfo[0];
                return true;
            }
        }

        private List<Vector2> GetVertexPool(bool left, bool top, bool right, bool bottom)
        {
            InitChunks();
            List<Vector2> vertexPool = new List<Vector2>();
            GTerrainChunk[] chunks = GetChunks();

            int gridSize = TerrainData.Geometry.ChunkGridSize;
            for (int i = 0; i < chunks.Length; ++i)
            {
                GTerrainChunk c = chunks[i];
                bool valid = false;
                if (left && (int)c.Index.x == 0)
                {
                    valid = true;
                }
                else if (top && (int)c.Index.y == gridSize - 1)
                {
                    valid = true;
                }
                else if (right && (int)c.Index.x == gridSize - 1)
                {
                    valid = true;
                }
                else if (bottom && (int)c.Index.y == 0)
                {
                    valid = true;
                }

                if (valid)
                {
                    vertexPool.AddRange(c.FlattenSubDivTree());
                }
            }
            return vertexPool;
        }

        public Vector2 WorldPointToUV(Vector3 point)
        {
            if (TerrainData == null)
                return Vector2.zero;
            Vector3 localPoint = transform.InverseTransformPoint(point);
            Vector3 terrainSize = new Vector3(TerrainData.Geometry.Width, TerrainData.Geometry.Height, TerrainData.Geometry.Length);
            Vector2 uv = new Vector2(
                GUtilities.InverseLerpUnclamped(0, terrainSize.x, localPoint.x),
                GUtilities.InverseLerpUnclamped(0, terrainSize.z, localPoint.z));
            return uv;
        }

        public Vector3 WorldPointToNormalized(Vector3 point)
        {
            if (TerrainData == null)
                return Vector2.zero;
            Vector3 localPoint = transform.InverseTransformPoint(point);
            Vector3 terrainSize = new Vector3(TerrainData.Geometry.Width, TerrainData.Geometry.Height, TerrainData.Geometry.Length);
            Vector3 normalized = new Vector3(
                GUtilities.InverseLerpUnclamped(0, terrainSize.x, localPoint.x),
                GUtilities.InverseLerpUnclamped(0, terrainSize.y, localPoint.y),
                GUtilities.InverseLerpUnclamped(0, terrainSize.z, localPoint.z));
            return normalized;
        }

        public Vector3 NormalizedToWorldPoint(Vector3 normalizedPoint)
        {
            if (TerrainData == null)
            {
                return normalizedPoint;
            }
            else
            {
                Matrix4x4 matrix = Matrix4x4.TRS(
                    transform.position,
                    transform.rotation,
                    new Vector3(
                        transform.lossyScale.x * TerrainData.Geometry.Width,
                        transform.lossyScale.y * TerrainData.Geometry.Height,
                        transform.lossyScale.z * TerrainData.Geometry.Length));
                return matrix.MultiplyPoint(normalizedPoint);
            }
        }

        public Matrix4x4 GetWorldToNormalizedMatrix()
        {
            Matrix4x4 matrix = Matrix4x4.TRS(
                    transform.position,
                    transform.rotation,
                    new Vector3(
                        transform.lossyScale.x * TerrainData.Geometry.Width,
                        transform.lossyScale.y * TerrainData.Geometry.Height,
                        transform.lossyScale.z * TerrainData.Geometry.Length));
            return matrix.inverse;
        }

        public void ForceLOD(int level)
        {
            GTerrainChunk[] chunks = GetChunks();
            for (int i = 0; i < chunks.Length; ++i)
            {
                chunks[i].LodGroupComponent.ForceLOD(level);
            }
        }

        private void GetNeighborEdgeVertices(HashSet<Vector2> vertexPool)
        {
            if (LeftNeighbor != null && LeftNeighbor.isActiveAndEnabled)
            {
                List<Vector2> verts = LeftNeighbor.GetVertexPool(false, false, true, false);
                for (int i = 0; i < verts.Count; ++i)
                {
                    Vector2 v = verts[i];
                    if (v.x == 1)
                    {
                        vertexPool.Add(new Vector2(0, v.y));
                    }
                }
            }
            if (RightNeighbor != null && RightNeighbor.isActiveAndEnabled)
            {
                List<Vector2> verts = RightNeighbor.GetVertexPool(true, false, false, false);
                for (int i = 0; i < verts.Count; ++i)
                {
                    Vector2 v = verts[i];
                    if (v.x == 0)
                    {
                        vertexPool.Add(new Vector2(1, v.y));
                    }
                }
            }
            if (TopNeighbor != null && TopNeighbor.isActiveAndEnabled)
            {
                List<Vector2> verts = TopNeighbor.GetVertexPool(false, false, false, true);
                for (int i = 0; i < verts.Count; ++i)
                {
                    Vector2 v = verts[i];
                    if (v.y == 0)
                    {
                        vertexPool.Add(new Vector2(v.x, 1));
                    }
                }
            }
            if (BottomNeighbor != null && BottomNeighbor.isActiveAndEnabled)
            {
                List<Vector2> verts = BottomNeighbor.GetVertexPool(false, true, false, false);
                for (int i = 0; i < verts.Count; ++i)
                {
                    Vector2 v = verts[i];
                    if (v.y == 1)
                    {
                        vertexPool.Add(new Vector2(v.x, 0));
                    }
                }
            }
        }

        public Vector4 GetHeightMapSample(Vector2 uv)
        {
            if (TerrainData == null)
                return Vector4.zero;
            Vector4 sample = TerrainData.Geometry.GetDecodedHeightMapSample(uv);
            return sample;
        }

        private static Vector4 GetHeightMapSample(GStylizedTerrain t, Vector2 uv)
        {
            if (t == null || !t.isActiveAndEnabled)
                return Vector4.zero;
            else
                return t.GetHeightMapSample(uv);
        }

        public Vector4 GetInterpolatedHeightMapSample(Vector2 uv)
        {
            int count = 1;
            Vector4 sample = Vector4.zero;
            if (uv.x == 0 && uv.y == 0) //bottom left
            {
                sample += GetHeightMapSample(uv);
                sample += GetHeightMapSample(LeftNeighbor, new Vector2(1, 0));
                sample += GetHeightMapSample(BottomNeighbor, new Vector2(0, 1));
                sample +=
                    LeftNeighbor != null ? GetHeightMapSample(LeftNeighbor.BottomNeighbor, new Vector2(1, 1)) :
                    BottomNeighbor != null ? GetHeightMapSample(BottomNeighbor.LeftNeighbor, new Vector2(1, 1)) : Vector4.zero;

                count += LeftNeighbor != null ? 1 : 0;
                count += BottomNeighbor != null ? 1 : 0;
                count += LeftNeighbor != null && LeftNeighbor.BottomNeighbor != null ? 1 :
                    BottomNeighbor != null && BottomNeighbor.LeftNeighbor != null ? 1 : 0;
            }
            else if (uv.x == 0 && uv.y == 1) //top left
            {
                sample += GetHeightMapSample(uv);
                sample += GetHeightMapSample(LeftNeighbor, new Vector2(1, 1));
                sample += GetHeightMapSample(TopNeighbor, new Vector2(0, 0));
                sample +=
                    LeftNeighbor != null ? GetHeightMapSample(LeftNeighbor.TopNeighbor, new Vector2(1, 0)) :
                    TopNeighbor != null ? GetHeightMapSample(TopNeighbor.LeftNeighbor, new Vector2(1, 0)) : Vector4.zero;

                count += LeftNeighbor != null ? 1 : 0;
                count += TopNeighbor != null ? 1 : 0;
                count += LeftNeighbor != null && LeftNeighbor.TopNeighbor != null ? 1 :
                    TopNeighbor != null && TopNeighbor.LeftNeighbor != null ? 1 : 0;
            }
            else if (uv.x == 1 && uv.y == 1) //top right
            {
                sample += GetHeightMapSample(uv);
                sample += GetHeightMapSample(RightNeighbor, new Vector2(0, 1));
                sample += GetHeightMapSample(TopNeighbor, new Vector2(1, 0));
                sample +=
                    RightNeighbor != null ? GetHeightMapSample(RightNeighbor.TopNeighbor, new Vector2(0, 0)) :
                    TopNeighbor != null ? GetHeightMapSample(TopNeighbor.RightNeighbor, new Vector2(0, 0)) : Vector4.zero;

                count += RightNeighbor != null ? 1 : 0;
                count += TopNeighbor != null ? 1 : 0;
                count += RightNeighbor != null && RightNeighbor.TopNeighbor != null ? 1 :
                    TopNeighbor != null && TopNeighbor.RightNeighbor != null ? 1 : 0;
            }
            else if (uv.x == 1 && uv.y == 0) //bottom right
            {
                sample += GetHeightMapSample(uv);
                sample += GetHeightMapSample(RightNeighbor, new Vector2(0, 0));
                sample += GetHeightMapSample(BottomNeighbor, new Vector2(1, 1));
                sample +=
                    RightNeighbor != null ? GetHeightMapSample(RightNeighbor.BottomNeighbor, new Vector2(0, 1)) :
                    BottomNeighbor != null ? GetHeightMapSample(BottomNeighbor.RightNeighbor, new Vector2(0, 1)) : Vector4.zero;

                count += RightNeighbor != null ? 1 : 0;
                count += BottomNeighbor != null ? 1 : 0;
                count += RightNeighbor != null && RightNeighbor.BottomNeighbor != null ? 1 :
                    BottomNeighbor != null && BottomNeighbor.RightNeighbor != null ? 1 : 0;
            }
            else if (uv.x == 0 && uv.y != 0 && uv.y != 1) //left edge
            {
                sample += GetHeightMapSample(uv);
                sample += GetHeightMapSample(LeftNeighbor, new Vector2(1, uv.y));

                count += LeftNeighbor != null ? 1 : 0;
            }
            else if (uv.x == 1 && uv.y != 0 && uv.y != 1) //right edge
            {
                sample += GetHeightMapSample(uv);
                sample += GetHeightMapSample(RightNeighbor, new Vector2(0, uv.y));

                count += RightNeighbor != null ? 1 : 0;
            }
            else if (uv.x != 0 && uv.x != 1 && uv.y == 0) //bottom edge
            {
                sample += GetHeightMapSample(uv);
                sample += GetHeightMapSample(BottomNeighbor, new Vector2(uv.x, 1));

                count += BottomNeighbor != null ? 1 : 0;
            }
            else if (uv.x != 0 && uv.x != 1 && uv.y == 1) //top edge
            {
                sample += GetHeightMapSample(uv);
                sample += GetHeightMapSample(TopNeighbor, new Vector2(uv.x, 0));

                count += TopNeighbor != null ? 1 : 0;
            }
            else
            {
                sample = GetHeightMapSample(uv);
            }
            return sample * 1.0f / count;
        }

        public static void MatchEdges(int groupId)
        {
            IEnumerator<GStylizedTerrain> terrains = ActiveTerrains.GetEnumerator();
            while (terrains.MoveNext())
            {
                GStylizedTerrain t = terrains.Current;
                if (t.TerrainData == null)
                    continue;
                if (groupId < 0 ||
                    (groupId >= 0 && t.GroupId == groupId))
                {
                    if (t.LeftNeighbor != null)
                    {
                        Rect leftRect = new Rect();
                        leftRect.size = new Vector2(0.01f, 1);
                        leftRect.center = new Vector2(0, 0.5f);
                        t.TerrainData.Geometry.SetRegionDirty(leftRect);
                    }

                    if (t.TopNeighbor != null)
                    {
                        Rect topRect = new Rect();
                        topRect.size = new Vector2(1, 0.01f);
                        topRect.center = new Vector2(0.5f, 1);
                        t.TerrainData.Geometry.SetRegionDirty(topRect);
                    }

                    if (t.RightNeighbor != null)
                    {
                        Rect rightRect = new Rect();
                        rightRect.size = new Vector2(0.01f, 1f);
                        rightRect.center = new Vector2(1, 0.5f);
                        t.TerrainData.Geometry.SetRegionDirty(rightRect);
                    }

                    if (t.BottomNeighbor != null)
                    {
                        Rect bottomRect = new Rect();
                        bottomRect.size = new Vector2(1, 0.01f);
                        bottomRect.center = new Vector2(0.5f, 0);
                        t.TerrainData.Geometry.SetRegionDirty(bottomRect);
                    }

                    t.TerrainData.SetDirty(GTerrainData.DirtyFlags.Geometry);
                }
            }
        }

        public void UpdateTreesPosition(bool showProgressBar = false)
        {
            if (TerrainData == null)
                return;
            if (TerrainData.Foliage.Trees == null)
                return;
            Vector3 terrainSize = new Vector3(
                TerrainData.Geometry.Width,
                TerrainData.Geometry.Height,
                TerrainData.Geometry.Length);
            Rect[] dirtyRect = TerrainData.Foliage.GetTreeDirtyRegions();
            int treeCount = TerrainData.Foliage.TreeInstances.Count;
            Vector3 pos = Vector3.zero;
            Vector2 uv = Vector2.zero;
            Ray ray = new Ray();
            ray.direction = Vector3.down;
            Vector3 localRayOrigin = Vector3.zero;
            RaycastHit hit;
            RaycastHit hitTerrain;
            RaycastHit hitWorld;

#if UNITY_EDITOR
            string title = "Working";
            string info = "Updating Tree Position ... 0%";
            int lastPercent = 0;
            if (showProgressBar)
                GCommonGUI.CancelableProgressBar(title, info, 0);
#endif

            for (int i = 0; i < treeCount; ++i)
            {
#if UNITY_EDITOR
                if (showProgressBar)
                {
                    float progress = Mathf.InverseLerp(0, treeCount - 1, i);
                    int percent = Mathf.RoundToInt(progress * 100);
                    if (percent != lastPercent)
                    {
                        info = string.Format("Updating Tree Position ... {0}%", percent);
                        GCommonGUI.CancelableProgressBar(title, info, progress);
                    }
                    lastPercent = percent;
                }
#endif

                GTreeInstance tree = TerrainData.Foliage.TreeInstances[i];
                pos.Set(
                    Mathf.Clamp01(tree.Position.x),
                    Mathf.Clamp01(tree.Position.y),
                    Mathf.Clamp01(tree.Position.z));

                for (int r = 0; r < dirtyRect.Length; ++r)
                {
                    uv.Set(tree.Position.x, tree.Position.z);
                    if (dirtyRect[r].Contains(uv))
                    {
                        localRayOrigin.Set(
                            terrainSize.x * pos.x,
                            10000,
                            terrainSize.z * pos.z);
                        ray.origin = transform.TransformPoint(localRayOrigin);

                        bool isHit = false;

                        if (TerrainData.Foliage.TreeSnapMode == GSnapMode.Terrain)
                        {
                            isHit = Raycast(ray, out hit, float.MaxValue);
                        }
                        else
                        {
                            bool isHitTerrain = Raycast(ray, out hitTerrain, float.MaxValue);
                            float terrainHitPoint = isHitTerrain ? hitTerrain.point.y : -10000;

                            bool isHitWorld = Physics.Raycast(ray, out hitWorld, float.MaxValue, TerrainData.Foliage.TreeSnapLayerMask);
                            float worldHitPoint = isHitWorld ? hitWorld.point.y : -10000;

                            isHit = isHitTerrain || isHitWorld;
                            hit = terrainHitPoint > worldHitPoint ? hitTerrain : hitWorld;
                        }

                        if (isHit)
                        {
                            pos.y = Mathf.InverseLerp(0, terrainSize.y, transform.InverseTransformPoint(hit.point).y);
                        }
                        break;
                    }
                }
                tree.Position = pos;
                TerrainData.Foliage.TreeInstances[i] = tree;
            }

#if UNITY_EDITOR
            if (showProgressBar)
                GCommonGUI.ClearProgressBar();
#endif
        }

        public void UpdateGrassPatches(int prototypeIndex = -1, bool showProgressBar = false)
        {
            if (TerrainData == null)
                return;
            if (TerrainData.Foliage.Grasses == null)
                return;

            Rect[] dirtyRects = TerrainData.Foliage.GetGrassDirtyRegions();
            GGrassPatch[] patches = TerrainData.Foliage.GrassPatches;
            List<GGrassPatch> toUpdatePatches = new List<GGrassPatch>();
            for (int i = 0; i < patches.Length; ++i)
            {
                Rect uvRect = patches[i].GetUvRange();
                for (int r = 0; r < dirtyRects.Length; ++r)
                {
                    if (dirtyRects[r].Overlaps(uvRect))
                    {
                        toUpdatePatches.Add(patches[i]);
                        break;
                    }
                }
            }

#if UNITY_EDITOR
            string title = "Working";
            string info = "Batching Grass ... 0%";
            if (showProgressBar)
                GCommonGUI.CancelableProgressBar(title, info, 0);
#endif

            for (int i = 0; i < toUpdatePatches.Count; ++i)
            {
                GGrassPatch patch = toUpdatePatches[i];
                UpdateGrassPatch(patch, prototypeIndex);

#if UNITY_EDITOR
                if (showProgressBar)
                {
                    float progress = Mathf.InverseLerp(0, toUpdatePatches.Count - 1, i);
                    int percent = Mathf.RoundToInt(progress * 100);
                    info = string.Format("Batching Grass ... {0}%", percent);
                    GCommonGUI.CancelableProgressBar(title, info, progress);
                }
#endif
            }

#if UNITY_EDITOR
            if (showProgressBar)
                GCommonGUI.ClearProgressBar();
#endif
        }

        private void UpdateGrassPatch(GGrassPatch patch, int prototypeIndex)
        {
            if (TerrainData.Foliage.Grasses == null)
                return;

            Vector3 terrainSize = new Vector3(
               TerrainData.Geometry.Width,
               TerrainData.Geometry.Height,
               TerrainData.Geometry.Length);
            int instanceCount = patch.Instances.Count;
            Vector3 pos = Vector3.zero;
            Vector2 uv = Vector2.zero;
            Ray ray = new Ray();
            ray.direction = Vector3.down;
            Vector3 localRayOrigin = Vector3.zero;
            RaycastHit hit;
            RaycastHit hitTerrain;
            RaycastHit hitWorld;
            bool isHit;

            for (int i = 0; i < instanceCount; ++i)
            {
                GGrassInstance grass = patch.Instances[i];
                if (grass.PrototypeIndex < 0 || grass.PrototypeIndex >= TerrainData.Foliage.Grasses.Prototypes.Count)
                    continue;

                pos.Set(
                    Mathf.Clamp01(grass.Position.x),
                    Mathf.Clamp01(grass.Position.y),
                    Mathf.Clamp01(grass.Position.z));
                localRayOrigin.Set(
                    terrainSize.x * pos.x,
                    10000,
                    terrainSize.z * pos.z);
                ray.origin = transform.TransformPoint(localRayOrigin);

                if (TerrainData.Foliage.GrassSnapMode == GSnapMode.Terrain)
                {
                    isHit = Raycast(ray, out hit, float.MaxValue);
                }
                else
                {
                    bool isHitTerrain = Raycast(ray, out hitTerrain, float.MaxValue);
                    float terrainHitPoint = isHitTerrain ? hitTerrain.point.y : -10000;

                    bool isHitWorld = Physics.Raycast(ray, out hitWorld, float.MaxValue, TerrainData.Foliage.GrassSnapLayerMask);
                    float worldHitPoint = isHitWorld ? hitWorld.point.y : -10000;

                    isHit = isHitTerrain || isHitWorld;
                    hit = terrainHitPoint > worldHitPoint ? hitTerrain : hitWorld;
                }

                if (isHit)
                {
                    pos.y = Mathf.InverseLerp(0, terrainSize.y, transform.InverseTransformPoint(hit.point).y);
                }

                grass.Position = pos;
                GGrassPrototype proto = TerrainData.Foliage.Grasses.Prototypes[grass.PrototypeIndex];
                if (proto.AlignToSurface)
                {
                    Quaternion currentRotationY = Quaternion.Euler(0, grass.Rotation.eulerAngles.y, 0);
                    Quaternion rotationXZ = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    grass.Rotation = rotationXZ * currentRotationY;
                }
                else
                {
                    Quaternion currentRotationY = Quaternion.Euler(0, grass.Rotation.eulerAngles.y, 0);
                    grass.Rotation = currentRotationY;
                }
                patch.Instances[i] = grass;
            }

            if (prototypeIndex < 0 || patch.RequireFullUpdate)
                patch.UpdateMeshes();
            else
                patch.UpdateMesh(prototypeIndex);
        }

        public RenderTexture GetHeightMap(int resolution)
        {
            if (heightMap == null)
            {
                heightMap = new RenderTexture(resolution, resolution, 0, GGeometry.HeightMapRTFormat, RenderTextureReadWrite.Linear);
                RenderHeightMap(heightMap);
            }
            if (heightMap.width != resolution ||
                heightMap.height != resolution ||
                heightMap.format != GGeometry.HeightMapRTFormat)
            {
                heightMap.Release();
                GUtilities.DestroyObject(heightMap);
                heightMap = new RenderTexture(resolution, resolution, 0, GGeometry.HeightMapRTFormat, RenderTextureReadWrite.Linear);
                RenderHeightMap(heightMap);
            }
            if (!heightMap.IsCreated())
            {
                RenderHeightMap(heightMap);
            }

            return heightMap;
        }

        private void RenderHeightMap(RenderTexture rt)
        {
            GTerrainChunk[] chunks = GetChunks();
            for (int i = 0; i < chunks.Length; ++i)
            {
                RenderHeightMap(rt, chunks[i]);
            }
        }

        private void RenderHeightMap(RenderTexture rt, GTerrainChunk chunk)
        {
            Mesh m = chunk.MeshFilterComponent.sharedMesh;
            if (m == null)
                return;
            Vector2[] uvs = m.uv;
            Material mat = GInternalMaterials.GeometricalHeightMapMaterial;
            mat.SetPass(0);
            RenderTexture.active = rt;
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Begin(GL.TRIANGLES);

            for (int i = 0; i < uvs.Length; ++i)
            {
                GL.Vertex3(uvs[i].x, uvs[i].y, GetInterpolatedHeightMapSample(uvs[i]).x);
            }

            GL.End();
            GL.PopMatrix();
            RenderTexture.active = null;
        }

        private void CreateHeightMap2D()
        {
            if (heightMap2D != null)
            {
                GUtilities.DestroyObject(heightMap2D);
            }
            heightMap2D = new Texture2D(TerrainData.Geometry.HeightMapResolution, TerrainData.Geometry.HeightMapResolution, GGeometry.HeightMapFormat, false, true);
            GCommon.CopyFromRT(heightMap2D, GetHeightMap(TerrainData.Geometry.HeightMapResolution));
        }

        public RenderTexture GetSharpNormalMap(int resolution)
        {
            if (normalMap == null)
            {
                normalMap = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
                RenderSharpNormalMap(normalMap);
            }
            if (normalMap.width != resolution ||
                normalMap.height != resolution)
            {
                normalMap.Release();
                GUtilities.DestroyObject(normalMap);
                normalMap = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
                RenderSharpNormalMap(normalMap);
            }
            if (!normalMap.IsCreated())
            {
                RenderSharpNormalMap(normalMap);
            }

            return normalMap;
        }

        private void RenderSharpNormalMap(RenderTexture rt)
        {
            GNormalMapGeneratorParams param = new GNormalMapGeneratorParams();
            param.Terrain = this;
            param.Space = GNormalMapSpace.Local;
            param.Mode = GNormalMapMode.Sharp;
            GNormalMapGenerator gen = new GNormalMapGenerator();
            gen.RenderSharpNormalMap(param, rt);
        }

        public RenderTexture GetInterpolatedNormalMap(int resolution)
        {
            if (normalMapInterpolated == null)
            {
                normalMapInterpolated = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
                RenderInterpolatedNormalMap(normalMapInterpolated);
            }
            if (normalMapInterpolated.width != resolution ||
                normalMapInterpolated.height != resolution)
            {
                normalMapInterpolated.Release();
                GUtilities.DestroyObject(normalMapInterpolated);
                normalMapInterpolated = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
                RenderInterpolatedNormalMap(normalMapInterpolated);
            }
            if (!normalMapInterpolated.IsCreated())
            {
                RenderInterpolatedNormalMap(normalMapInterpolated);
            }

            return normalMapInterpolated;
        }

        private void RenderInterpolatedNormalMap(RenderTexture rt)
        {
            GNormalMapGeneratorParams param = new GNormalMapGeneratorParams();
            param.Terrain = this;
            param.Space = GNormalMapSpace.Local;
            param.Mode = GNormalMapMode.Interpolated;
            GNormalMapGenerator gen = new GNormalMapGenerator();
            gen.RenderInterpolatedNormalMap(param, rt);
        }

        public RenderTexture GetPerPixelNormalMap(int resolution)
        {
            if (normalMapPerPixel == null)
            {
                normalMapPerPixel = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
                RenderPerPixelNormalMap(normalMapPerPixel);
            }
            if (normalMapPerPixel.width != resolution ||
                normalMapPerPixel.height != resolution)
            {
                normalMapPerPixel.Release();
                GUtilities.DestroyObject(normalMapPerPixel);
                normalMapPerPixel = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
                RenderPerPixelNormalMap(normalMapPerPixel);
            }
            if (!normalMapPerPixel.IsCreated())
            {
                RenderPerPixelNormalMap(normalMapPerPixel);
            }

            return normalMapPerPixel;
        }

        private void RenderPerPixelNormalMap(RenderTexture rt)
        {
            GNormalMapGeneratorParams param = new GNormalMapGeneratorParams();
            param.Terrain = this;
            param.Space = GNormalMapSpace.Local;
            param.Mode = GNormalMapMode.PerPixel;
            GNormalMapGenerator gen = new GNormalMapGenerator();
            gen.RenderPerPixelNormalMap(param, rt);
        }

        public void Refresh()
        {
            InitChunks();
            GTerrainChunk[] chunks = GetChunks();
            for (int i = 0; i < chunks.Length; ++i)
            {
                chunks[i].Refresh();
            }
        }

        private void DrawChunkUpdateDebug(Camera cam)
        {
            Material mat = GInternalMaterials.MaskVisualizerMaterial;
            MaterialPropertyBlock block = new MaterialPropertyBlock();

            GTerrainChunk[] chunks = GetChunks();
            for (int i = 0; i < chunks.Length; ++i)
            {
                GTerrainChunk c = chunks[i];
                Mesh m = c.MeshFilterComponent.sharedMesh;
                if (m == null)
                    continue;

                System.DateTime now = System.DateTime.Now;
                System.TimeSpan timeSpan = now - c.LastUpdatedTime;
                float duration = 1f;
                float span = (float)timeSpan.TotalSeconds;
                if (span > duration)
                    continue;

                float alpha = 1 - Mathf.InverseLerp(0, duration, span);
                Color color = new Color(1, 1, 1, alpha * 0.5f);
                block.SetColor("_Color", color);
                Graphics.DrawMesh(
                    m,
                    c.transform.localToWorldMatrix,
                    mat,
                    LayerMask.NameToLayer("Default"),
                    cam,
                    0,
                    block);
            }

        }

        public RenderTexture GetGrassVectorFieldRenderTexture()
        {
            if (TerrainData == null)
                return null;

            int resolution = TerrainData.Foliage.VectorFieldMapResolution;
            if (grassVectorFieldMap == null)
            {
                grassVectorFieldMap = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                GCommon.FillTexture(grassVectorFieldMap, Color.gray);
            }
            if (grassVectorFieldMap.width != resolution ||
                grassVectorFieldMap.height != resolution)
            {
                grassVectorFieldMap.Release();
                GUtilities.DestroyObject(grassVectorFieldMap);
                grassVectorFieldMap = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                GCommon.FillTexture(grassVectorFieldMap, Color.gray);
            }
            if (!grassVectorFieldMap.IsCreated())
            {
                GCommon.FillTexture(grassVectorFieldMap, Color.gray);
            }

            if (grassVectorFieldMapTmp == null)
            {
                grassVectorFieldMapTmp = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            }
            if (grassVectorFieldMapTmp.width != resolution ||
                grassVectorFieldMapTmp.height != resolution)
            {
                grassVectorFieldMapTmp.Release();
                GUtilities.DestroyObject(grassVectorFieldMapTmp);
                grassVectorFieldMapTmp = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            }

            //TerrainData.Shading.MaterialToRender.SetTexture("_MainTex", grassVectorFieldMap);

            return grassVectorFieldMap;
        }

        private void LateUpdate()
        {
            if (TerrainData != null && TerrainData.Foliage.EnableInteractiveGrass)
            {
                FadeGrassVectorField();
            }
            if (TerrainData != null && !TerrainData.Foliage.EnableInteractiveGrass)
            {
                if (grassVectorFieldMap != null)
                {
                    grassVectorFieldMap.Release();
                    GUtilities.DestroyObject(grassVectorFieldMap);
                }
                if (grassVectorFieldMapTmp != null)
                {
                    grassVectorFieldMapTmp.Release();
                    GUtilities.DestroyObject(grassVectorFieldMapTmp);
                }
            }
        }

        private void FadeGrassVectorField()
        {
            RenderTexture rt = GetGrassVectorFieldRenderTexture();
            RenderTexture bg = grassVectorFieldMapTmp;
            GCommon.CopyToRT(rt, bg);

            Material mat = GInternalMaterials.InteractiveGrassVectorFieldMaterial;
            mat.SetTexture("_Background", bg);
            mat.SetFloat("_Opacity", TerrainData.Foliage.RestoreSensitive);
            int pass = 1;
            GCommon.DrawQuad(rt, GCommon.FullRectUvPoints, mat, pass);
        }
    }
}

