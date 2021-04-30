using Pinwheel.Griffin.BackupTool;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pinwheel.Griffin.SplineTool
{
    [CustomEditor(typeof(GRampMaker))]
    public class GRampMakerInspector : Editor
    {
        private GRampMaker instance;
        private Dictionary<GStylizedTerrain, RenderTexture> previewTextures;
        private MaterialPropertyBlock previewPropertyBlock;

        private static readonly string HISTORY_PREFIX = "Make Ramp";

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedo;
            instance = (GRampMaker)target;
            instance.Internal_UpdateFalloffTexture();

            previewPropertyBlock = new MaterialPropertyBlock();
            GCommon.RegisterBeginRender(OnCameraRender);
            GCommon.RegisterBeginRenderSRP(OnCameraRenderSRP);
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
            GCommon.UnregisterBeginRender(OnCameraRender);
            GCommon.UnregisterBeginRenderSRP(OnCameraRenderSRP);
            if (previewTextures != null)
            {
                foreach (GStylizedTerrain t in previewTextures.Keys)
                {
                    RenderTexture rt = previewTextures[t];
                    if (rt == null)
                        continue;
                    rt.Release();
                    Object.DestroyImmediate(rt);
                }
            }
        }

        private void OnUndoRedo()
        {
            if (Selection.activeGameObject != instance.gameObject)
                return;
            if (string.IsNullOrEmpty(GUndoCompatibleBuffer.Instance.CurrentBackupName))
                return;
            GBackup.Restore(GUndoCompatibleBuffer.Instance.CurrentBackupName);
        }

        public override void OnInspectorGUI()
        {
            instance.SplineCreator = EditorGUILayout.ObjectField("Spline Creator", instance.SplineCreator, typeof(GSplineCreator), true) as GSplineCreator;
            if (instance.SplineCreator == null)
                return;
            bool wideMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;
            EditorGUI.BeginChangeCheck();
            instance.Falloff = EditorGUILayout.CurveField("Falloff", instance.Falloff, Color.red, new Rect(0, 0, 1, 1));
            if (EditorGUI.EndChangeCheck())
            {
                instance.Internal_UpdateFalloffTexture();
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Falloff Noise");
            instance.FalloffNoise = EditorGUILayout.ObjectField(instance.FalloffNoise, typeof(Texture2D), false) as Texture2D;
            EditorGUILayout.EndHorizontal();
            if (instance.FalloffNoise != null)
                instance.FalloffNoiseSize = EditorGUILayout.Vector2Field("Falloff Noise Size", instance.FalloffNoiseSize);

            instance.AdditionalMeshResolution = EditorGUILayout.IntField("Additional Mesh Resolution", instance.AdditionalMeshResolution);
            instance.RaiseHeight = EditorGUILayout.Toggle("Raise Height", instance.RaiseHeight);
            instance.LowerHeight = EditorGUILayout.Toggle("Lower Height", instance.LowerHeight);
            instance.Editor_ShowLivePreview = EditorGUILayout.Toggle("Live Preview", instance.Editor_ShowLivePreview);
            EditorGUIUtility.wideMode = wideMode;

            EditorGUILayout.Space();
            if (GUILayout.Button("Apply"))
            {
                GAnalytics.Record(GAnalytics.SPLINE_RAMP_MAKER);
                CreateInitialBackup();
                ApplyRamp();
                CreateBackupAfterApplyRamp();
            }
        }

        private void CreateInitialBackup()
        {
            GBackup.TryCreateInitialBackup(HISTORY_PREFIX, instance.SplineCreator.GroupId, GCommon.HeightMapAndFoliageResourceFlags);
        }

        private void ApplyRamp()
        {
            EditorUtility.DisplayProgressBar("Applying", "Creating ramp...", 1f);
            try
            {
                instance.Apply();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
            }
            EditorUtility.ClearProgressBar();
        }

        private void CreateBackupAfterApplyRamp()
        {
            GBackup.TryCreateBackup(HISTORY_PREFIX, instance.SplineCreator.GroupId, GCommon.HeightMapAndFoliageResourceFlags);
        }

        private void OnCameraRender(Camera cam)
        {
            if (GGriffinSettings.Instance.ShowLivePreview &&
                instance.Editor_ShowLivePreview)
                DrawLivePreview(cam);
        }

        private void OnCameraRenderSRP(UnityEngine.Rendering.ScriptableRenderContext context, Camera cam)
        {
            OnCameraRender(cam);
        }

        private void DrawLivePreview(Camera cam)
        {
            IEnumerator<GStylizedTerrain> terrains = GStylizedTerrain.ActiveTerrains.GetEnumerator();
            while (terrains.MoveNext())
            {
                GStylizedTerrain t = terrains.Current;
                if (t.TerrainData == null)
                    continue;
                if (instance.SplineCreator.GroupId >= 0 &&
                    instance.SplineCreator.GroupId != t.GroupId)
                    continue;
                DrawLivePreview(t, cam);
            }
        }

        private void DrawLivePreview(GStylizedTerrain t, Camera cam)
        {
            if (t.transform.rotation != Quaternion.identity ||
                t.transform.lossyScale != Vector3.one)
                return;

            Mesh previewMesh = GGriffinSettings.Instance.GetLivePreviewMesh(t.TerrainData.Geometry.MeshResolution);
            RenderTexture rt = GetPreviewTexture(t);
            instance.Internal_Apply(t, rt, instance.SplineCreator.Editor_Vertices);
            Vector3 terrainSize = new Vector3(
                t.TerrainData.Geometry.Width,
                t.TerrainData.Geometry.Height,
                t.TerrainData.Geometry.Length);

            previewPropertyBlock.Clear();
            previewPropertyBlock.SetTexture("_OldHeightMap", t.TerrainData.Geometry.HeightMap);
            previewPropertyBlock.SetTexture("_NewHeightMap", rt);
            previewPropertyBlock.SetTexture("_MainTex", rt);
            previewPropertyBlock.SetFloat("_Height", t.TerrainData.Geometry.Height);
            previewPropertyBlock.SetVector("_BoundMin", t.transform.position);
            previewPropertyBlock.SetVector("_BoundMax", t.transform.TransformPoint(terrainSize));

            Material mat = GInternalMaterials.GeometryLivePreviewMaterial;

            List<Rect> uvRects = new List<Rect>(instance.SplineCreator.SweepDirtyRect(t));
            for (int i = 0; i < uvRects.Count; ++i)
            {
                Rect r = uvRects[i];
                Vector3 localPos = new Vector3(
                    terrainSize.x * r.x,
                    0f,
                    terrainSize.z * r.y);
                Vector3 worldPos = t.transform.TransformPoint(localPos);
                Quaternion rotation = Quaternion.identity;
                Vector3 scale = new Vector3(terrainSize.x * r.width, 1, terrainSize.z * r.height);

                Graphics.DrawMesh(
                    previewMesh,
                    Matrix4x4.TRS(worldPos, rotation, scale),
                    mat,
                    LayerMask.NameToLayer("Default"),
                    cam,
                    0,
                    previewPropertyBlock);
            }
        }

        private RenderTexture GetPreviewTexture(GStylizedTerrain t)
        {
            if (previewTextures == null)
            {
                previewTextures = new Dictionary<GStylizedTerrain, RenderTexture>();
            }

            int resolution = t.TerrainData.Geometry.HeightMapResolution;
            if (!previewTextures.ContainsKey(t) ||
                previewTextures[t] == null)
            {
                RenderTexture rt = new RenderTexture(resolution, resolution, 0, GGeometry.HeightMapRTFormat, RenderTextureReadWrite.Linear);
                previewTextures[t] = rt;
            }
            else if (previewTextures[t].width != resolution ||
                previewTextures[t].height != resolution ||
                previewTextures[t].format != GGeometry.HeightMapRTFormat)
            {
                previewTextures[t].Release();
                Object.DestroyImmediate(previewTextures[t]);
                RenderTexture rt = new RenderTexture(resolution, resolution, 0, GGeometry.HeightMapRTFormat, RenderTextureReadWrite.Linear);
                previewTextures[t] = rt;
            }

            previewTextures[t].wrapMode = TextureWrapMode.Clamp;

            return previewTextures[t];
        }
    }
}
