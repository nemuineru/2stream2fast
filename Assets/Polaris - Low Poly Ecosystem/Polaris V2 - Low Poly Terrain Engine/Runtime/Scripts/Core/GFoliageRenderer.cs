using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.Griffin
{
    public static class GFoliageRenderer
    {
        static GFoliageRenderer()
        {
            resourceManager = new GRendererResourceManager();
        }

        private static GRendererResourceManager resourceManager;
        private static GRendererResourceManager ResourceManager
        {
            get
            {
                if (resourceManager == null)
                    resourceManager = new GRendererResourceManager();
                return resourceManager;
            }
        }

        private static Plane[] frustumPlanes = new Plane[6];
        private static Matrix4x4[] batchMatrices = new Matrix4x4[MAX_INSTANCE_PER_BATCH];
        private static Matrix4x4[] batchMatricesShadowCaster = new Matrix4x4[MAX_INSTANCE_PER_BATCH];
        private static MaterialPropertyBlock grassMaterialPropertyBlock = new MaterialPropertyBlock();
        private static List<int> preprocessFlags = new List<int>();
        private static List<Vector3> preprocessWorldPositions = new List<Vector3>();

        private const string COLOR_PROPERTY = "_Color";
        private const string MAINTEX_PROPERTY = "_MainTex";
        private const string BEND_FACTOR_PROPERTY = "_BendFactor";
        private const string IMAGE_TEXCOORDS_PROPERTY = "_ImageTexcoords";
        private const string IMAGE_COUNT_PROPERTY = "_ImageCount";
        private const string NOISETEX_PROPERTY = "_NoiseTex";
        private const string VECTOR_FIELD_PROPERTY = "_VectorField";
        private const string WORLD_TO_NORMALIZED_PROPERTY = "_WorldToNormalized";
        private const string WIND_PROPERTY = "_Wind";

        private const int FLAG_CULLED = 0;
        private const int FLAG_NON_INSTANCED = 1;
        private const int FLAG_NON_INSTANCED_BILLBOARD = 2;
        private const int FLAG_INSTANCED = 3;
        private const int FLAG_INSTANCED_BILLBOARD = 4;
        private const int MAX_INSTANCE_PER_BATCH = 1023;

        public static void Render(GStylizedTerrain t, Camera cam)
        {
            if (t.TerrainData == null)
                return;
            if (!t.TerrainData.Rendering.DrawFoliage || GGriffinSettings.Instance.IsHidingFoliageOnEditing)
                return;
            Plane[] frustum = GeometryUtility.CalculateFrustumPlanes(cam);
            if (!GeometryUtility.TestPlanesAABB(frustum, t.Bounds))
            {
                return;
            }

            if (t.TerrainData.Rendering.EnableInstancing && IsInstancingAvailable())
            {
                RenderTreesInstanced(t, cam);
                RenderGrass(t, cam);
            }
            else
            {
                RenderTreesNonInstanced(t, cam);
                RenderGrass(t, cam);
            }
        }

        public static void RenderPreview(GStylizedTerrain t, Camera cam)
        {
            if (t.TerrainData == null)
                return;
            if (!t.TerrainData.Rendering.DrawFoliage || GGriffinSettings.Instance.IsHidingFoliageOnEditing)
                return;
            RenderPreviewGrass(t, cam);
        }

        private static void RenderTreesNonInstanced(GStylizedTerrain t, Camera cam)
        {
            if (t.TerrainData.Foliage.Trees == null ||
                t.TerrainData.Foliage.Trees.Prototypes.Count == 0)
                return;
            List<GTreePrototype> prototypes = t.TerrainData.Foliage.Trees.Prototypes;
            List<GTreeInstance> instances = t.TerrainData.Foliage.TreeInstances;
            int prototypeCount = prototypes.Count;
            int instanceCount = instances.Count;

            bool[] prototypeValidation = new bool[prototypeCount];
            int[] prototypeSubMeshCount = new int[prototypeCount];
            for (int pIndex = 0; pIndex < prototypeCount; ++pIndex)
            {
                prototypeValidation[pIndex] = prototypes[pIndex].IsValid;
                prototypeSubMeshCount[pIndex] = prototypeValidation[pIndex] ? prototypes[pIndex].SharedMesh.subMeshCount : 0;

                if (prototypeValidation[pIndex] == true &&
                    prototypes[pIndex].Billboard != null &&
                    prototypes[pIndex].Billboard.material != null)
                {
                    try
                    {
                        Material mat = prototypes[pIndex].Billboard.material;
                        mat.SetVectorArray(IMAGE_TEXCOORDS_PROPERTY, prototypes[pIndex].Billboard.GetImageTexCoords());
                        mat.SetInt(IMAGE_COUNT_PROPERTY, prototypes[pIndex].Billboard.imageCount);
                    }
                    catch { }
                }
            }

            Quaternion billboardShadowCasterRotation = Quaternion.identity;
            Light[] directionalLights = Light.GetLights(LightType.Directional, 0);
            Light firstDirLight = null;
            if (directionalLights.Length > 0)
            {
                firstDirLight = directionalLights[0];
                billboardShadowCasterRotation = Quaternion.Euler(0, firstDirLight.transform.root.eulerAngles.y, 0);
            }

            float sqrBillboardStart = t.TerrainData.Rendering.BillboardStart * t.TerrainData.Rendering.BillboardStart;
            float sqrTreeDistance = t.TerrainData.Rendering.TreeDistance * t.TerrainData.Rendering.TreeDistance;
            float sqrDistance = 0;
            Vector3 dimension = new Vector3(
                t.TerrainData.Geometry.Width,
                t.TerrainData.Geometry.Height,
                t.TerrainData.Geometry.Length);
            Vector3 localPos = Vector3.zero;
            Vector3 worldPos = Vector3.zero;
            int prototypeMaxIndex = prototypeCount - 1;
            GTreePrototype p = null;
            int subMeshCount = 0;
            int materialCount = 0;
            int drawCallCount = 0;
            int i = 0;
            int d = 0;
            Vector3 camLocalPos = t.transform.InverseTransformPoint(cam.transform.position);

            for (i = 0; i < instanceCount; ++i)
            {
                GTreeInstance tree = instances[i];
                if (tree.PrototypeIndex < 0 || tree.PrototypeIndex > prototypeMaxIndex)
                    continue;
                if (!prototypeValidation[tree.PrototypeIndex])
                    continue;
                localPos.Set(
                    tree.Position.x * dimension.x,
                    tree.Position.y * dimension.y,
                    tree.Position.z * dimension.z);

                sqrDistance = Vector3.SqrMagnitude(localPos - camLocalPos);
                if (sqrDistance > sqrTreeDistance)
                    continue;

                worldPos = t.transform.TransformPoint(localPos);
                p = prototypes[tree.PrototypeIndex];

                if (sqrDistance < sqrBillboardStart)
                {
                    subMeshCount = prototypeSubMeshCount[tree.PrototypeIndex];
                    materialCount = p.SharedMaterials.Length;
                    drawCallCount = Mathf.Min(subMeshCount, materialCount);
                    for (d = 0; d < drawCallCount; ++d)
                    {
                        Graphics.DrawMesh(
                            p.SharedMesh,
                            Matrix4x4.TRS(worldPos + Vector3.up * p.PivotOffset, p.BaseRotation * tree.Rotation, tree.Scale.Mul(p.BaseScale)),
                            p.SharedMaterials[d],
                            p.Layer,
                            cam, //camera
                            d, //sub mesh index
                            null, //properties block
                            p.ShadowCastingMode,
                            p.ReceiveShadow,
                            null, //probe anchor
                            LightProbeUsage.BlendProbes,
                            null); //proxy volume
                    }
                }
                else
                {
                    if (p.Billboard == null)
                        continue;
                    if (p.Billboard.material == null)
                        continue;
                    Vector3 lookDir = cam.transform.position - worldPos;
                    lookDir.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(-lookDir, Vector3.up);
                    Mesh billboardMesh = ResourceManager.GetBillboardMesh(p.Billboard);
                    Graphics.DrawMesh(
                            billboardMesh,
                            Matrix4x4.TRS(worldPos + Vector3.up * p.PivotOffset, rotation, tree.Scale),
                            p.Billboard.material,
                            p.Layer,
                            cam, //camera
                            0, //sub mesh index
                            null, //properties block
                            ShadowCastingMode.Off,
                            false, //receive shadow
                            null, //probe anchor
                            LightProbeUsage.BlendProbes,
                            null); //proxy volume

                    if (p.ShadowCastingMode == ShadowCastingMode.Off)
                        continue;
                    if (firstDirLight == null)
                        continue;

                    Graphics.DrawMesh(
                            billboardMesh,
                            Matrix4x4.TRS(worldPos + Vector3.up * p.PivotOffset, billboardShadowCasterRotation, tree.Scale),
                            p.Billboard.material,
                            p.Layer,
                            cam, //camera
                            0, //sub mesh index
                            null, //properties block
                            ShadowCastingMode.ShadowsOnly,
                            false, //receive shadow
                            null, //probe anchor
                            LightProbeUsage.Off,
                            null); //proxy volume
                }
            }
        }

        public static bool IsInstancingAvailable()
        {
#if UNITY_EDITOR
            return SystemInfo.supportsInstancing &&
                (EditorApplication.isPlaying);
#else
            return SystemInfo.supportsInstancing;
#endif
        }

        private static void RenderGrass(GStylizedTerrain t, Camera cam)
        {
            if (t.TerrainData.Foliage.Grasses == null ||
                t.TerrainData.Foliage.Grasses.Prototypes.Count == 0)
                return;
            List<GGrassPrototype> prototypes = t.TerrainData.Foliage.Grasses.Prototypes;

            GGrassPatch[] patches = t.TerrainData.Foliage.GrassPatches;
            if (patches == null)
                return;

            float grassDistance = t.TerrainData.Rendering.GrassDistance;
            Vector3 dimension = new Vector3(
                t.TerrainData.Geometry.Width,
                t.TerrainData.Geometry.Height,
                t.TerrainData.Geometry.Length);
            float patchWorldRadius = dimension.x * 0.5f / t.TerrainData.Foliage.PatchGridSize;
            Vector3 boundLocalPos = Vector3.zero;
            Vector3 boundWorldPos = Vector3.zero;

            int FLAG_CULLED = 0;
            int FLAG_DIRTY = 1;
            int FLAG_BATCHED = 2;
            int[] flags = new int[patches.Length];
            Rect[] dirtyRect = t.TerrainData.Foliage.GetGrassDirtyRegions();
            for (int i = 0; i < patches.Length; ++i)
            {
                Rect uvRect = patches[i].GetUvRange();
                boundLocalPos.Set(
                    uvRect.center.x * dimension.x,
                    0,
                    uvRect.center.y * dimension.z);
                boundWorldPos = t.transform.TransformPoint(boundLocalPos);
                boundWorldPos.y = cam.transform.position.y;
                if (Vector3.Distance(boundWorldPos, cam.transform.position) - patchWorldRadius > grassDistance)
                {
                    flags[i] = FLAG_CULLED;
                    continue;
                }

                for (int r = 0; r < dirtyRect.Length; ++r)
                {
                    if (dirtyRect[r].Overlaps(uvRect))
                    {
                        flags[i] = FLAG_DIRTY;
                        break;
                    }
                }

                if (flags[i] == FLAG_DIRTY)
                    continue;

                flags[i] = FLAG_BATCHED;
            }

            Vector3 meshLocalPos = Vector3.zero;
            Vector3 meshWorldPos = Vector3.zero;

            grassMaterialPropertyBlock.Clear();
            if (t.TerrainData.Foliage.EnableInteractiveGrass)
            {
                grassMaterialPropertyBlock.SetTexture(VECTOR_FIELD_PROPERTY, t.GetGrassVectorFieldRenderTexture());
                grassMaterialPropertyBlock.SetMatrix(WORLD_TO_NORMALIZED_PROPERTY, t.GetWorldToNormalizedMatrix());
            }

            grassMaterialPropertyBlock.SetTexture(NOISETEX_PROPERTY, GGriffinSettings.Instance.DefaultNoiseTexture);
            IEnumerator<GWindZone> windZone = GWindZone.ActiveWindZones.GetEnumerator();
            if (windZone.MoveNext())
            {
                GWindZone w = windZone.Current;
                grassMaterialPropertyBlock.SetVector(WIND_PROPERTY, w.GetWindParams());
            }

            //draw batched            
            //Material grassMat = GCommon.CurrentRenderPipeline == GRenderPipelineType.Lightweight ?
            //    GGriffinSettings.Instance.TerrainDataDefault.Foliage.GrassMaterialLW :
            //    GGriffinSettings.Instance.TerrainDataDefault.Foliage.GrassMaterial;
            Material grassMat = null;
            if (GCommon.CurrentRenderPipeline == GRenderPipelineType.Builtin)
            {
                grassMat = t.TerrainData.Foliage.EnableInteractiveGrass ?
                    GGriffinSettings.Instance.TerrainDataDefault.Foliage.GrassInteractiveMaterial :
                    GGriffinSettings.Instance.TerrainDataDefault.Foliage.GrassMaterial;
                if (grassMat == null)
                {
                    Debug.Log("Grass material missing. Try re-installing the Polaris V2 package.");
                }
            }
            else if (GCommon.CurrentRenderPipeline == GRenderPipelineType.Lightweight)
            {
                grassMat = t.TerrainData.Foliage.EnableInteractiveGrass ?
                    GGriffinSettings.Instance.TerrainDataDefault.Foliage.GrassInteractiveMaterialLWRP :
                    GGriffinSettings.Instance.TerrainDataDefault.Foliage.GrassMaterialLWRP;
                if (grassMat == null)
                {
                    Debug.Log("Grass material missing. Try installing the 'Lightweight Render Pipeline Support' extension from the Extension Window (Window>Griffin>Tools>Extension)");
                }
            }
            else if (GCommon.CurrentRenderPipeline == GRenderPipelineType.Universal)
            {
                grassMat = t.TerrainData.Foliage.EnableInteractiveGrass ?
                    GGriffinSettings.Instance.TerrainDataDefault.Foliage.GrassInteractiveMaterialURP :
                    GGriffinSettings.Instance.TerrainDataDefault.Foliage.GrassMaterialURP;
                if (grassMat == null)
                {
                    Debug.Log("Grass material missing. Try installing the 'Universal Render Pipeline Support' extension from the Extension Window (Window>Griffin>Tools>Extension)");
                }
            }

            if (grassMat == null)
            {
                return;
            }

            for (int i = 0; i < patches.Length; ++i)
            {
                if (flags[i] != FLAG_BATCHED)
                    continue;
                for (int p = 0; p < prototypes.Count; ++p)
                {
                    grassMaterialPropertyBlock.SetColor(COLOR_PROPERTY, prototypes[p].Color);
                    grassMaterialPropertyBlock.SetFloat(BEND_FACTOR_PROPERTY, prototypes[p].BendFactor);
                    if (prototypes[p].Shape != GGrassShape.DetailObject && prototypes[p].Texture != null)
                        grassMaterialPropertyBlock.SetTexture(MAINTEX_PROPERTY, prototypes[p].Texture);
                    Mesh mesh = patches[i].GetMesh(p);
                    if (mesh != null)
                    {
                        meshLocalPos = mesh.bounds.center;
                        meshWorldPos = t.transform.TransformPoint(meshLocalPos);
                        if (Vector3.Distance(meshWorldPos, cam.transform.position) - patchWorldRadius > grassDistance)
                            continue;

                        Graphics.DrawMesh(
                            mesh,
                            Matrix4x4.TRS(t.transform.position, Quaternion.identity, Vector3.one),
                            prototypes[p].Shape == GGrassShape.DetailObject ? prototypes[p].DetailMaterial : grassMat,
                            prototypes[p].Layer,
                            cam,
                            0,
                            prototypes[p].Shape == GGrassShape.DetailObject ? null : grassMaterialPropertyBlock,
                            prototypes[p].Shape == GGrassShape.DetailObject ? prototypes[p].ShadowCastingMode : ShadowCastingMode.On,
                            prototypes[p].Shape == GGrassShape.DetailObject ? prototypes[p].ReceiveShadow : true,
                            null,
                            LightProbeUsage.BlendProbes,
                            null);
                    }
                }
            }

            if (GCommon.CurrentRenderPipeline == GRenderPipelineType.Builtin)
                return;

            Vector3 localPos = Vector3.zero;
            Vector3 worldPos = Vector3.zero;
            Vector3 scale = Vector3.zero;

            Material previewMat = GGriffinSettings.Instance.TerrainDataDefault.Foliage.GrassPreviewMaterial;
            for (int pIndex = 0; pIndex < prototypes.Count; ++pIndex)
            {
                GGrassPrototype proto = prototypes[pIndex];
                //grassMaterialPropertyBlock.Clear();
                //grassMaterialPropertyBlock.SetTexture(MAINTEX_PROPERTY, proto.Texture);

                for (int i = 0; i < patches.Length; ++i)
                {
                    if (flags[i] != FLAG_DIRTY)
                        continue;
                    GGrassPatch patch = patches[i];
                    int instanceCount = patch.Instances.Count;
                    for (int j = 0; j < instanceCount; ++j)
                    {
                        GGrassInstance grass = patch.Instances[j];
                        if (grass.PrototypeIndex != pIndex)
                            continue;

                        localPos.Set(
                            dimension.x * grass.Position.x,
                            dimension.y * grass.Position.y,
                            dimension.z * grass.Position.z);
                        worldPos = t.transform.TransformPoint(localPos);
                        scale.Set(
                            prototypes[grass.PrototypeIndex].Size.x * grass.Scale.x,
                            prototypes[grass.PrototypeIndex].Size.y * grass.Scale.y,
                            prototypes[grass.PrototypeIndex].Size.z * grass.Scale.z);

                        Mesh mesh = prototypes[grass.PrototypeIndex].GetBaseMesh();
                        Graphics.DrawMesh(
                                mesh,
                                Matrix4x4.TRS(worldPos, grass.Rotation, scale),
                                previewMat,
                                prototypes[grass.PrototypeIndex].Layer,
                                cam,
                                0,
                                null, //grassMaterialPropertyBlock
                                ShadowCastingMode.On,
                                true,
                                null,
                                LightProbeUsage.BlendProbes,
                                null);
                    }
                }
            }
        }

        private static void RenderPreviewGrass(GStylizedTerrain t, Camera cam)
        {
            if (t.TerrainData.Foliage.Grasses == null ||
                t.TerrainData.Foliage.Grasses.Prototypes.Count == 0)
                return;

            List<GGrassPrototype> prototypes = t.TerrainData.Foliage.Grasses.Prototypes;
            Vector3 dimension = new Vector3(
                t.TerrainData.Geometry.Width,
                t.TerrainData.Geometry.Height,
                t.TerrainData.Geometry.Length);
            Vector3 localPos = Vector3.zero;
            Vector3 worldPos = Vector3.zero;
            Vector3 scale = Vector3.zero;

            Material mat = GGriffinSettings.Instance.TerrainDataDefault.Foliage.GrassPreviewMaterial;

            Rect[] dirtyRects = t.TerrainData.Foliage.GetGrassDirtyRegions();
            GGrassPatch[] patches = t.TerrainData.Foliage.GrassPatches;
            bool[] dirtyFlags = new bool[patches.Length];
            for (int p = 0; p < patches.Length; ++p)
            {
                dirtyFlags[p] = false;
                Rect uvRange = patches[p].GetUvRange();
                for (int r = 0; r < dirtyRects.Length; ++r)
                {
                    if (uvRange.Overlaps(dirtyRects[r]))
                    {
                        dirtyFlags[p] = true;
                        break;
                    }
                }
            }

            for (int pIndex = 0; pIndex < prototypes.Count; ++pIndex)
            {
                GGrassPrototype proto = prototypes[pIndex];
                //mat.SetTexture(MAINTEX_PROPERTY, proto.Texture);
                mat.SetPass(0);
                for (int p = 0; p < patches.Length; ++p)
                {
                    if (dirtyFlags[p] == false)
                        continue;

                    int instanceCount = patches[p].Instances.Count;
                    for (int j = 0; j < instanceCount; ++j)
                    {
                        GGrassInstance grass = patches[p].Instances[j];
                        if (grass.PrototypeIndex != pIndex)
                            continue;

                        localPos.Set(
                            dimension.x * grass.Position.x,
                            dimension.y * grass.Position.y,
                            dimension.z * grass.Position.z);
                        worldPos = t.transform.TransformPoint(localPos);
                        scale.Set(
                            prototypes[grass.PrototypeIndex].Size.x * grass.Scale.x,
                            prototypes[grass.PrototypeIndex].Size.y * grass.Scale.y,
                            prototypes[grass.PrototypeIndex].Size.z * grass.Scale.z);

                        Mesh mesh = prototypes[grass.PrototypeIndex].GetBaseMesh();
                        if (mesh != null)
                        {
                            Graphics.DrawMeshNow(
                                mesh,
                                Matrix4x4.TRS(worldPos, grass.Rotation, scale));
                        }
                    }
                }
            }
        }

        private static void RenderTreesInstanced(GStylizedTerrain t, Camera cam)
        {
            if (t.TerrainData.Foliage.Trees == null ||
                t.TerrainData.Foliage.Trees.Prototypes.Count == 0)
                return;

            PreprocessRendering(t, cam, preprocessFlags, preprocessWorldPositions);
            RenderTreesInstanced_NonInstanced(t, cam, preprocessFlags, preprocessWorldPositions);
            RenderTreesInstanced_NonInstancedBillboard(t, cam, preprocessFlags, preprocessWorldPositions);
            RenderTreesInstanced_Instanced(t, cam, preprocessFlags, preprocessWorldPositions);
            RenderTreesInstanced_InstancedBillboard(t, cam, preprocessFlags, preprocessWorldPositions);
        }

        private static void PreprocessRendering(GStylizedTerrain t, Camera cam, List<int> flags, List<Vector3> worldPositions)
        {
            flags.Clear();
            worldPositions.Clear();

            List<GTreePrototype> prototypes = t.TerrainData.Foliage.Trees.Prototypes;
            List<GTreeInstance> instances = t.TerrainData.Foliage.TreeInstances;
            int prototypeCount = prototypes.Count;
            int instanceCount = instances.Count;
            bool[] prototypeValidation = new bool[prototypeCount];
            for (int pIndex = 0; pIndex < prototypeCount; ++pIndex)
            {
                prototypeValidation[pIndex] = prototypes[pIndex].IsValid;
            }

            float sqrTreeDistance = t.TerrainData.Rendering.TreeDistance * t.TerrainData.Rendering.TreeDistance;
            float sqrBillboardDistance = t.TerrainData.Rendering.BillboardStart * t.TerrainData.Rendering.BillboardStart;
            float sqrDistance = 0;
            Bounds bound = new Bounds();
            Vector3 boundSize = Vector3.zero;
            Vector3 boundCenter = Vector3.zero;
            Vector3 dimension = new Vector3(
                t.TerrainData.Geometry.Width,
                t.TerrainData.Geometry.Height,
                t.TerrainData.Geometry.Length);
            Vector3 localPos = Vector3.zero;
            Vector3 worldPos = Vector3.zero;
            bool isInstancingEnabledForAllSharedMaterials = true;
            frustumPlanes = GeometryUtility.CalculateFrustumPlanes(cam);

            for (int i = 0; i < instanceCount; ++i)
            {
                GTreeInstance tree = instances[i];
                //invalid prototype index
                if (tree.PrototypeIndex < 0 || tree.PrototypeIndex >= prototypeCount)
                {
                    flags.Add(FLAG_CULLED);
                    worldPositions.Add(Vector3.zero);
                    continue;
                }

                if (prototypeValidation[tree.PrototypeIndex] == false)
                {
                    flags.Add(FLAG_CULLED);
                    worldPositions.Add(Vector3.zero);
                    continue;
                }

                GTreePrototype proto = prototypes[tree.PrototypeIndex];
                //cull layer
                if (cam.cullingMask >= 0 && (cam.cullingMask & (1 << proto.Layer)) == 0)
                {
                    flags.Add(FLAG_CULLED);
                    worldPositions.Add(Vector3.zero);
                    continue;
                }

                localPos.Set(
                    dimension.x * tree.Position.x,
                    dimension.y * tree.Position.y,
                    dimension.z * tree.Position.z);
                worldPos = t.transform.TransformPoint(localPos);
                sqrDistance = Vector3.SqrMagnitude(worldPos - cam.transform.position);

                //cull distance
                if (sqrDistance > sqrTreeDistance)
                {
                    flags.Add(FLAG_CULLED);
                    worldPositions.Add(Vector3.zero);
                    continue;
                }

                //cull frustum
                boundSize.Set(
                    proto.SharedMesh.bounds.size.x * tree.Scale.x,
                    proto.SharedMesh.bounds.size.y * tree.Scale.y,
                    proto.SharedMesh.bounds.size.z * tree.Scale.z);
                boundCenter = worldPos;
                bound.size = boundSize;
                bound.center = boundCenter;

                if (!GeometryUtility.TestPlanesAABB(frustumPlanes, bound))
                {
                    flags.Add(FLAG_CULLED);
                    worldPositions.Add(Vector3.zero);
                    continue;
                }

                //cull no billboard
                if (sqrDistance >= sqrBillboardDistance &&
                    (proto.Billboard == null || proto.Billboard.material == null))
                {
                    flags.Add(FLAG_CULLED);
                    worldPositions.Add(Vector3.zero);
                    continue;
                }

                //the object will be rendered
                worldPositions.Add(worldPos);

                //determine render mode
                if (sqrDistance >= sqrBillboardDistance)
                {
                    if (proto.Billboard.material.enableInstancing)
                    {
                        flags.Add(FLAG_INSTANCED_BILLBOARD);
                    }
                    else
                    {
                        flags.Add(FLAG_NON_INSTANCED_BILLBOARD);
                    }
                }
                else
                {
                    isInstancingEnabledForAllSharedMaterials = true;
                    for (int mIndex = 0; mIndex < proto.SharedMaterials.Length; ++mIndex)
                    {
                        if (!proto.SharedMaterials[mIndex].enableInstancing)
                        {
                            isInstancingEnabledForAllSharedMaterials = false;
                            break;
                        }
                    }

                    if (isInstancingEnabledForAllSharedMaterials)
                    {
                        flags.Add(FLAG_INSTANCED);
                    }
                    else
                    {
                        flags.Add(FLAG_NON_INSTANCED);
                    }
                }
            }
        }

        private static void RenderTreesInstanced_NonInstanced(GStylizedTerrain t, Camera cam, List<int> flags, List<Vector3> worldPositions)
        {
            List<GTreePrototype> prototypes = t.TerrainData.Foliage.Trees.Prototypes;
            List<GTreeInstance> instances = t.TerrainData.Foliage.TreeInstances;
            int instanceCount = instances.Count;
            int subMeshCount = 0;
            int materialCount = 0;
            int drawCallCount = 0;
            int d;

            for (int i = 0; i < instanceCount; ++i)
            {
                if (flags[i] != FLAG_NON_INSTANCED)
                    continue;
                GTreeInstance tree = instances[i];
                GTreePrototype proto = prototypes[tree.PrototypeIndex];
                subMeshCount = proto.SharedMesh.subMeshCount;
                materialCount = proto.SharedMaterials.Length;
                drawCallCount = Mathf.Min(subMeshCount, materialCount);
                for (d = 0; d < drawCallCount; ++d)
                {
                    Graphics.DrawMesh(
                        proto.SharedMesh,
                        Matrix4x4.TRS(worldPositions[i] + Vector3.up * proto.PivotOffset, tree.Rotation * proto.BaseRotation, tree.Scale.Mul(proto.BaseScale)),
                        proto.SharedMaterials[d],
                        proto.Layer,
                        cam, //camera
                        d, //sub mesh index
                        null, //properties block
                        proto.ShadowCastingMode,
                        proto.ReceiveShadow,
                        null, //probe anchor
                        LightProbeUsage.BlendProbes,
                        null); //proxy volume
                }
            }
        }

        private static void RenderTreesInstanced_NonInstancedBillboard(GStylizedTerrain t, Camera cam, List<int> flags, List<Vector3> worldPositions)
        {
            List<GTreePrototype> prototypes = t.TerrainData.Foliage.Trees.Prototypes;
            List<GTreeInstance> instances = t.TerrainData.Foliage.TreeInstances;
            int instanceCount = instances.Count;

            Light[] directionalLights = Light.GetLights(LightType.Directional, 0);
            Light firstDirLight = null;
            if (directionalLights.Length > 0)
            {
                firstDirLight = directionalLights[0];
            }
            Quaternion billboardShadowCasterRotation = Quaternion.Euler(0, firstDirLight.transform.root.eulerAngles.y, 0);

            for (int i = 0; i < instanceCount; ++i)
            {
                if (flags[i] != FLAG_NON_INSTANCED_BILLBOARD)
                    continue;
                GTreeInstance tree = instances[i];
                GTreePrototype proto = prototypes[tree.PrototypeIndex];

                Vector3 lookDir = cam.transform.position - worldPositions[i];
                lookDir.y = 0;
                Quaternion rotation = Quaternion.LookRotation(-lookDir, Vector3.up);
                Mesh billboardMesh = ResourceManager.GetBillboardMesh(proto.Billboard);
                Graphics.DrawMesh(
                        billboardMesh,
                        Matrix4x4.TRS(worldPositions[i] + Vector3.up * proto.PivotOffset, rotation, instances[i].Scale),
                        proto.Billboard.material,
                        proto.Layer,
                        cam, //camera
                        0, //sub mesh index
                        null, //properties block
                        ShadowCastingMode.Off,
                        false, //receive shadow
                        null, //probe anchor
                        LightProbeUsage.BlendProbes,
                        null); //proxy volume

                if (proto.ShadowCastingMode == ShadowCastingMode.Off)
                    continue;
                if (firstDirLight == null)
                    continue;

                Graphics.DrawMesh(
                        billboardMesh,
                        Matrix4x4.TRS(worldPositions[i], billboardShadowCasterRotation, instances[i].Scale),
                        proto.Billboard.material,
                        proto.Layer,
                        cam, //camera
                        0, //sub mesh index
                        null, //properties block
                        ShadowCastingMode.ShadowsOnly,
                        false, //receive shadow
                        null, //probe anchor
                        LightProbeUsage.Off,
                        null); //proxy volume

            }
        }

        private static void RenderTreesInstanced_Instanced(GStylizedTerrain t, Camera cam, List<int> flags, List<Vector3> worldPositions)
        {
            List<GTreePrototype> prototypes = t.TerrainData.Foliage.Trees.Prototypes;
            List<GTreeInstance> instances = t.TerrainData.Foliage.TreeInstances;
            int prototypeCount = prototypes.Count;
            int instanceCount = instances.Count;
            int subMeshCount = 0;
            int materialCount = 0;
            int drawCallCount = 0;
            int batchInstanceCount = 0;

            for (int p = 0; p < prototypeCount; ++p)
            {
                for (int i = 0; i <= instanceCount; ++i)
                {
                    //render batch
                    if ((instanceCount == i ||
                        batchInstanceCount == MAX_INSTANCE_PER_BATCH) &&
                        batchInstanceCount > 0)
                    {
                        subMeshCount = prototypes[p].SharedMesh.subMeshCount;
                        materialCount = prototypes[p].SharedMaterials.Length;
                        drawCallCount = Mathf.Min(subMeshCount, materialCount);

                        for (int d = 0; d < drawCallCount; ++d)
                        {
                            Graphics.DrawMeshInstanced(
                                prototypes[p].SharedMesh,
                                d,
                                prototypes[p].SharedMaterials[d],
                                batchMatrices,
                                batchInstanceCount,
                                null, //properties
                                prototypes[p].ShadowCastingMode,
                                prototypes[p].ReceiveShadow,
                                prototypes[p].Layer,
                                cam,
                                LightProbeUsage.BlendProbes,
                                null); //proxy volume
                        }
                        batchInstanceCount = 0;
                    }

                    if (instanceCount == i)
                        break;

                    if (flags[i] != FLAG_INSTANCED)
                        continue;
                    GTreeInstance tree = instances[i];
                    if (tree.PrototypeIndex != p)
                        continue;
                    batchMatrices[batchInstanceCount] = Matrix4x4.TRS(
                        worldPositions[i] + Vector3.up * prototypes[p].PivotOffset,
                        tree.Rotation * prototypes[p].BaseRotation,
                        tree.Scale.Mul(prototypes[p].BaseScale));
                    batchInstanceCount += 1;
                }
            }
        }

        private static void RenderTreesInstanced_InstancedBillboard(GStylizedTerrain t, Camera cam, List<int> flags, List<Vector3> worldPositions)
        {
            List<GTreePrototype> prototypes = t.TerrainData.Foliage.Trees.Prototypes;
            List<GTreeInstance> instances = t.TerrainData.Foliage.TreeInstances;
            int prototypeCount = prototypes.Count;
            int instanceCount = instances.Count;
            int batchInstanceCount = 0;

            Light[] directionalLights = Light.GetLights(LightType.Directional, 0);
            Light firstDirLight = null;
            if (directionalLights.Length > 0)
            {
                firstDirLight = directionalLights[0];
            }
            Quaternion billboardShadowCasterRotation = Quaternion.Euler(0, firstDirLight.transform.root.eulerAngles.y, 0);

            for (int p = 0; p < prototypeCount; ++p)
            {
                for (int i = 0; i <= instanceCount; ++i)
                {
                    //render batch
                    if ((instanceCount == i ||
                        batchInstanceCount == MAX_INSTANCE_PER_BATCH) &&
                        batchInstanceCount > 0)
                    {
                        Mesh billboardMesh = ResourceManager.GetBillboardMesh(prototypes[p].Billboard);
                        Graphics.DrawMeshInstanced(
                                billboardMesh,
                                0, //submesh index
                                prototypes[p].Billboard.material,
                                batchMatrices,
                                batchInstanceCount,
                                null, //properties
                                ShadowCastingMode.Off,
                                false, //receive shadow
                                prototypes[p].Layer,
                                cam,
                                LightProbeUsage.BlendProbes,
                                null); //proxy volume

                        if (prototypes[p].ShadowCastingMode == ShadowCastingMode.Off)
                            continue;
                        if (firstDirLight == null)
                            continue;

                        Graphics.DrawMeshInstanced(
                                billboardMesh,
                                0, //submesh index
                                prototypes[p].Billboard.material,
                                batchMatricesShadowCaster,
                                batchInstanceCount,
                                null, //properties
                                ShadowCastingMode.ShadowsOnly,
                                false, //receive shadow
                                prototypes[p].Layer,
                                cam,
                                LightProbeUsage.BlendProbes,
                                null); //proxy volume

                        batchInstanceCount = 0;
                    }

                    if (instanceCount == i)
                        break;

                    if (flags[i] != FLAG_INSTANCED_BILLBOARD)
                        continue;
                    GTreeInstance tree = instances[i];
                    if (tree.PrototypeIndex != p)
                        continue;

                    Vector3 lookDir = cam.transform.position - worldPositions[i];
                    lookDir.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(-lookDir, Vector3.up);
                    batchMatrices[batchInstanceCount] = Matrix4x4.TRS(worldPositions[i] + Vector3.up * prototypes[p].PivotOffset, rotation, tree.Scale);
                    batchMatricesShadowCaster[batchInstanceCount] = Matrix4x4.TRS(worldPositions[i] + Vector3.up * prototypes[p].PivotOffset, billboardShadowCasterRotation, tree.Scale);
                    batchInstanceCount += 1;
                }
            }
        }
    }
}
