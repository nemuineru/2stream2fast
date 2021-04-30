using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Pinwheel.Griffin.DataTool;
#if __MICROSPLAT_POLARIS__
using JBooth.MicroSplat;
#endif

namespace Pinwheel.Griffin
{
    [CustomEditor(typeof(GStylizedTerrain))]
    public class GStylizedTerrainInspector : Editor
    {
        public delegate void GTerrainGUIInjectionHandler(GStylizedTerrain terrain, int order);
        public static event GTerrainGUIInjectionHandler GUIInject;

        private GStylizedTerrain instance;
        private GTerrainData data;
        private bool isNeighboringFoldoutExpanded;
        private List<GGenericMenuItem> geometryAdditionalContextAction;
        private List<GGenericMenuItem> foliageAdditionalContextAction;

        private const string DEFERRED_UPDATE_KEY = "geometry-deferred-update";

        private void OnEnable()
        {
            instance = (GStylizedTerrain)target;
            SceneView.duringSceneGui += DuringSceneGUI;
            if (instance.TerrainData != null)
                instance.TerrainData.Shading.UpdateMaterials();

            geometryAdditionalContextAction = new List<GGenericMenuItem>();
            geometryAdditionalContextAction.Add(new GGenericMenuItem(
                "Add Height Map Filter",
                false,
                () =>
                {
                    GHeightMapFilter filterComponent = instance.GetComponent<GHeightMapFilter>();
                    if (filterComponent == null)
                    {
                        instance.gameObject.AddComponent<GHeightMapFilter>();
                    }
                }));

            foliageAdditionalContextAction = new List<GGenericMenuItem>();
            foliageAdditionalContextAction.Add(new GGenericMenuItem(
                "Update Trees",
                false,
                () =>
                {
                    if (instance.TerrainData != null)
                    {
                        instance.TerrainData.Foliage.SetTreeRegionDirty(new Rect(0, 0, 1, 1));
                        instance.UpdateTreesPosition(true);
                        instance.TerrainData.Foliage.ClearTreeDirtyRegions();
                        instance.TerrainData.SetDirty(GTerrainData.DirtyFlags.Foliage);
                    }
                }));
            foliageAdditionalContextAction.Add(new GGenericMenuItem(
                "Update Grasses",
                false,
                () =>
                {
                    if (instance.TerrainData != null)
                    {
                        instance.TerrainData.Foliage.SetGrassRegionDirty(new Rect(0, 0, 1, 1));
                        instance.UpdateGrassPatches(-1, true);
                        instance.TerrainData.Foliage.ClearGrassDirtyRegions();
                        instance.TerrainData.SetDirty(GTerrainData.DirtyFlags.Foliage);
                    }
                }));
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            if (instance.TerrainData == null)
            {
                DrawNullTerrainDataGUI();
            }
            else
            {
                DrawGUI();
            }
        }

        private void DrawNullTerrainDataGUI()
        {
            instance.TerrainData = EditorGUILayout.ObjectField("Terrain Data", instance.TerrainData, typeof(GTerrainData), false) as GTerrainData;
        }

        private void DrawGUI()
        {
            InjectGUI(0);
            instance.TerrainData = EditorGUILayout.ObjectField("Terrain Data", instance.TerrainData, typeof(GTerrainData), false) as GTerrainData;
            data = instance.TerrainData;
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Generated Geometry", data.GeometryData, typeof(GTerrainGeneratedData), false);
            EditorGUILayout.ObjectField("Generated Foliage", data.FoliageData, typeof(GTerrainGeneratedData), false);
            GUI.enabled = true;
            InjectGUI(1);
            DrawGeometryGUI();
            InjectGUI(2);
            DrawShadingGUI();
            InjectGUI(3);
            DrawRenderingGUI();
            InjectGUI(4);
            DrawFoliageGUI();
            InjectGUI(5);
            DrawDataGUI();
            InjectGUI(6);
            if (data != null)
                GEditorCommon.SetTerrainDataDirty(data);
            DrawNeighboringGUI();
            InjectGUI(7);
        }

        private void DrawGeometryGUI()
        {
            bool isGenerating = instance != null && instance.Internal_IsGeneratingInBackground;
            string label = "Geometry" + (isGenerating ? " (Working)" : "");
            string id = "geometry" + data.Id;

            bool deferredUpdate = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(DEFERRED_UPDATE_KEY), false);

            GenericMenu menu = new GenericMenu();
            menu.AddItem(
                new GUIContent("Reset"),
                false,
                () => { ConfirmAndResetGeometry(); });
            menu.AddItem(
                new GUIContent("Update"),
                false,
                () => { data.Geometry.SetRegionDirty(GCommon.UnitRect); data.SetDirty(GTerrainData.DirtyFlags.GeometryAsync); });
            menu.AddItem(
                new GUIContent("Clean Up"),
                false,
                () => { data.Geometry.CleanUp(); });

            if (geometryAdditionalContextAction != null && geometryAdditionalContextAction.Count > 0)
            {
                menu.AddSeparator(null);
                for (int i = 0; i < geometryAdditionalContextAction.Count; ++i)
                {
                    GGenericMenuItem item = geometryAdditionalContextAction[i];
                    menu.AddItem(
                        new GUIContent(item.Name),
                        item.IsOn,
                        item.Action);
                }
            }

            menu.AddSeparator(null);
            menu.AddItem(
                new GUIContent("Toggle Deferred Update"),
                deferredUpdate,
                () =>
                {
                    deferredUpdate = !deferredUpdate;
                    EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(DEFERRED_UPDATE_KEY), deferredUpdate);
                });

            string headerWarning = null;
            if (instance != null &&
                instance.geometryVersion < GStylizedTerrain.GEOMETRY_VERSION_CHUNK_POSITION_AT_CHUNK_CENTER)
            {
                headerWarning = "Chunk position placement has been changed for better level streaming and baking. Go to CONTEXT>Update to re-generate the terrain.";
            }

            GEditorCommon.Foldout(label, false, id, () =>
            {
                GGeometry settings = data.Geometry;
                EditorGUI.BeginChangeCheck();

                GUI.enabled = !isGenerating;
                EditorGUILayout.LabelField("Use with cause, takes time to process.", GEditorCommon.WordWrapItalicLabel);
                settings.Width = EditorGUILayout.DelayedFloatField("Width", settings.Width);
                settings.Height = EditorGUILayout.DelayedFloatField("Height", settings.Height);
                settings.Length = EditorGUILayout.DelayedFloatField("Length", settings.Length);
                settings.HeightMapResolution = EditorGUILayout.DelayedIntField("Height Map Resolution", settings.HeightMapResolution);
                if (settings.PolygonDistribution == GPolygonDistributionMode.Dynamic)
                {
                    settings.MeshBaseResolution = EditorGUILayout.DelayedIntField("Mesh Base Resolution", settings.MeshBaseResolution);
                }
                settings.MeshResolution = EditorGUILayout.DelayedIntField("Mesh Resolution", settings.MeshResolution);
                settings.ChunkGridSize = EditorGUILayout.DelayedIntField("Grid Size", settings.ChunkGridSize);
                settings.LODCount = EditorGUILayout.DelayedIntField("LOD Count", settings.LODCount);
                settings.DisplacementSeed = EditorGUILayout.DelayedIntField("Displacement Seed", settings.DisplacementSeed);
                settings.DisplacementStrength = EditorGUILayout.DelayedFloatField("Displacement Strength", settings.DisplacementStrength);
                settings.PolygonDistribution = (GPolygonDistributionMode)EditorGUILayout.EnumPopup("Polygon Distribution", settings.PolygonDistribution);
                try
                {
                    settings.PolygonProcessorName = GEditorCommon.PolygonProcessorSelector("Polygon Processor", settings.PolygonProcessorName);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                    settings.PolygonProcessorName = null;
                }

                if (EditorGUI.EndChangeCheck() && !deferredUpdate)
                {
                    data.Geometry.SetRegionDirty(new Rect(0, 0, 1, 1));
                    data.SetDirty(GTerrainData.DirtyFlags.GeometryAsync);
                }

                if (deferredUpdate)
                {
                    GEditorCommon.Separator();
                    if (GUILayout.Button("Update"))
                    {
                        data.Geometry.SetRegionDirty(new Rect(0, 0, 1, 1));
                        data.SetDirty(GTerrainData.DirtyFlags.GeometryAsync);
                    }
                }
                GUI.enabled = true;
            },
            menu,
            headerWarning);
        }

        private void ConfirmAndResetGeometry()
        {
            if (EditorUtility.DisplayDialog(
                "Confirm",
                "Reset geometry data on this terrain? This action cannot be undone!",
                "OK", "Cancel"))
            {
                data.Geometry.ResetFull();
            }
        }

        private void DrawRenderingGUI()
        {
            string label = "Rendering";
            string id = "rendering" + data.Id;

            GenericMenu menu = new GenericMenu();
            menu.AddItem(
                new GUIContent("Reset"),
                false,
                () => { data.Rendering.ResetFull(); });
            GEditorCommon.Foldout(label, false, id, () =>
            {
                GRendering settings = data.Rendering;
                EditorGUI.BeginChangeCheck();
                settings.CastShadow = EditorGUILayout.Toggle("Cast Shadow", settings.CastShadow);
                settings.ReceiveShadow = EditorGUILayout.Toggle("Receive Shadow", settings.ReceiveShadow);
                settings.DrawFoliage = EditorGUILayout.Toggle("Draw Foliage", settings.DrawFoliage);
                GUI.enabled = SystemInfo.supportsInstancing;
                settings.EnableInstancing = EditorGUILayout.Toggle("Enable Instancing", settings.EnableInstancing);
                GUI.enabled = true;
                settings.BillboardStart = EditorGUILayout.Slider("Billboard Start", settings.BillboardStart, 0f, GCommon.MAX_TREE_DISTANCE);
                settings.TreeDistance = EditorGUILayout.Slider("Tree Distance", settings.TreeDistance, 0f, GCommon.MAX_TREE_DISTANCE);
                settings.GrassDistance = EditorGUILayout.Slider("Grass Distance", settings.GrassDistance, 0f, GCommon.MAX_GRASS_DISTANCE);
                if (EditorGUI.EndChangeCheck())
                {
                    data.SetDirty(GTerrainData.DirtyFlags.Rendering);
                    if (settings.EnableInstancing)
                    {
                        GAnalytics.Record(GAnalytics.ENABLE_INSTANCING, true);
                    }
                }
            }, menu);
        }

        private void DrawShadingGUI()
        {
            string label = "Shading";
            string id = "shading" + data.Id;

            GenericMenu menu = new GenericMenu();
            if (data.Shading.ShadingSystem == GShadingSystem.Polaris)
            {
                menu.AddItem(
                    new GUIContent("Reset"),
                    false,
                    () => { data.Shading.ResetFull(); });
                menu.AddItem(
                    new GUIContent("Refresh"),
                    false,
                    () => { data.Shading.UpdateLookupTextures(); data.Shading.UpdateMaterials(); });
                menu.AddItem(
                    new GUIContent("Set Shader"),
                    false,
                    () => { GTerrainWizardWindow.ShowWindowSetShaderMode(data); });
            }

            GEditorCommon.Foldout(label, false, id, () =>
            {
                GShading settings = data.Shading;
                EditorGUI.BeginChangeCheck();

#if __MICROSPLAT_POLARIS__
                if (settings.ShadingSystem == GShadingSystem.MicroSplat)
                {
                    EditorGUILayout.LabelField("Terrain shading is controlled by MicroSplat.", GEditorCommon.WordWrapItalicLabel);
                    GUI.enabled = false;
                    settings.CustomMaterial = EditorGUILayout.ObjectField("Material", settings.CustomMaterial, typeof(Material), false) as Material;
                    GUI.enabled = true;
                    settings.Splats = EditorGUILayout.ObjectField("Splats", settings.Splats, typeof(GSplatPrototypeGroup), false) as GSplatPrototypeGroup;
                    settings.MicroSplatTextureArrayConfig = EditorGUILayout.ObjectField("Texture Array Config", settings.MicroSplatTextureArrayConfig, typeof(TextureArrayConfig), false) as TextureArrayConfig;
                    return;
                }
#endif
                settings.CustomMaterial = EditorGUILayout.ObjectField("Material", settings.CustomMaterial, typeof(Material), false) as Material;
                if (settings.CustomMaterial != null)
                {
                    GUI.enabled = false;
                    EditorGUILayout.LabelField("Shader", settings.CustomMaterial.shader.name);
                    GUI.enabled = true;
                }

                settings.AlbedoMapResolution = EditorGUILayout.DelayedIntField("Albedo Map Resolution", settings.AlbedoMapResolution);
                settings.MetallicMapResolution = EditorGUILayout.DelayedIntField("Metallic Map Resolution", settings.MetallicMapResolution);
                if (EditorGUI.EndChangeCheck())
                {
                    data.SetDirty(GTerrainData.DirtyFlags.Shading);
                }

                EditorGUI.BeginChangeCheck();
                SerializedObject so = new SerializedObject(settings);
                SerializedProperty colorByNormalProps = so.FindProperty("colorByNormal");
                EditorGUILayout.PropertyField(colorByNormalProps);
                settings.ColorBlendCurve = EditorGUILayout.CurveField("Blend By Height", settings.ColorBlendCurve, Color.red, new Rect(0, 0, 1, 1));
                SerializedProperty colorByHeightProps = so.FindProperty("colorByHeight");
                EditorGUILayout.PropertyField(colorByHeightProps);
                if (EditorGUI.EndChangeCheck())
                {
                    so.ApplyModifiedProperties();
                    settings.UpdateLookupTextures();
                    data.SetDirty(GTerrainData.DirtyFlags.Shading);
                }
                colorByHeightProps.Dispose();
                colorByNormalProps.Dispose();
                so.Dispose();

                EditorGUI.BeginChangeCheck();
                settings.Splats = EditorGUILayout.ObjectField("Splats", settings.Splats, typeof(GSplatPrototypeGroup), false) as GSplatPrototypeGroup;
                settings.SplatControlResolution = EditorGUILayout.DelayedIntField("Splat Control Resolution", settings.SplatControlResolution);
                DrawAdvancedShading();
                if (EditorGUI.EndChangeCheck())
                {
                    data.SetDirty(GTerrainData.DirtyFlags.Shading);
                }
            }, menu);
        }

        private void DrawAdvancedShading()
        {
            string prefKey = GEditorCommon.GetProjectRelatedEditorPrefsKey("foldout", "shading", "properties-name", data.Id);
            bool expanded = EditorPrefs.GetBool(prefKey, false);
            expanded = EditorGUILayout.Foldout(expanded, "Properties Name");
            EditorPrefs.SetBool(prefKey, expanded);
            if (expanded)
            {
                EditorGUI.indentLevel += 1;

                GShading settings = data.Shading;
                settings.AlbedoMapPropertyName = EditorGUILayout.DelayedTextField("Albedo Map", settings.AlbedoMapPropertyName);
                settings.MetallicMapPropertyName = EditorGUILayout.DelayedTextField("Metallic Map", settings.MetallicMapPropertyName);
                settings.ColorByHeightPropertyName = EditorGUILayout.DelayedTextField("Color By Height", settings.ColorByHeightPropertyName);
                settings.ColorByNormalPropertyName = EditorGUILayout.DelayedTextField("Color By Height", settings.ColorByNormalPropertyName);
                settings.ColorBlendPropertyName = EditorGUILayout.DelayedTextField("Color Blend", settings.ColorBlendPropertyName);
                settings.DimensionPropertyName = EditorGUILayout.DelayedTextField("Dimension", settings.DimensionPropertyName);
                settings.SplatControlMapPropertyName = EditorGUILayout.DelayedTextField("Splat Control Map", settings.SplatControlMapPropertyName);
                settings.SplatMapPropertyName = EditorGUILayout.DelayedTextField("Splat Map", settings.SplatMapPropertyName);
                settings.SplatNormalPropertyName = EditorGUILayout.DelayedTextField("Splat Normal Map", settings.SplatNormalPropertyName);
                settings.SplatMetallicPropertyName = EditorGUILayout.DelayedTextField("Splat Metallic", settings.SplatMetallicPropertyName);
                settings.SplatSmoothnessPropertyName = EditorGUILayout.DelayedTextField("Splat Smoothness", settings.SplatSmoothnessPropertyName);

                EditorGUI.indentLevel -= 1;
            }
        }

        private void DrawFoliageGUI()
        {
            string label = "Foliage";
            string id = "foliage" + data.Id;

            GenericMenu menu = new GenericMenu();
            menu.AddItem(
                new GUIContent("Reset"),
                false,
                () => { ConfirmAndResetFoliage(); });
            menu.AddItem(
                new GUIContent("Refresh"),
                false,
                () => { data.Foliage.Refresh(); });
            menu.AddItem(
                new GUIContent("Clean Up"),
                false,
                () => { data.Foliage.CleanUp(); });
            menu.AddItem(
                new GUIContent("Clear All Trees"),
                false,
                () => { ConfirmAndClearAllTrees(); });
            menu.AddItem(
                new GUIContent("Clear All Grasses"),
                false,
                () => { ConfirmAndClearAllGrasses(); });

            if (foliageAdditionalContextAction != null && foliageAdditionalContextAction.Count > 0)
            {
                menu.AddSeparator(null);
                for (int i = 0; i < foliageAdditionalContextAction.Count; ++i)
                {
                    menu.AddItem(
                        new GUIContent(foliageAdditionalContextAction[i].Name),
                        foliageAdditionalContextAction[i].IsOn,
                        foliageAdditionalContextAction[i].Action);
                }
            }

            GEditorCommon.Foldout(label, false, id, () =>
            {
                GFoliage settings = data.Foliage;
                EditorGUI.BeginChangeCheck();
                settings.Trees = EditorGUILayout.ObjectField("Trees", settings.Trees, typeof(GTreePrototypeGroup), false) as GTreePrototypeGroup;
                settings.TreeSnapMode = (GSnapMode)EditorGUILayout.EnumPopup("Snap Mode", settings.TreeSnapMode);
                SerializedObject so = new SerializedObject(settings);
                SerializedProperty sp0 = so.FindProperty("treeSnapLayerMask");
                if (settings.TreeSnapMode == GSnapMode.World && sp0 != null)
                {
                    EditorGUILayout.PropertyField(sp0, new GUIContent("Snap Layers"));
                }

                GUI.enabled = false;
                EditorGUILayout.LabelField("Tree Instance Count", settings.TreeInstances.Count.ToString());
                GUI.enabled = true;
                if (EditorGUI.EndChangeCheck())
                {
                    so.ApplyModifiedProperties();
                    data.SetDirty(GTerrainData.DirtyFlags.Foliage);
                }

                sp0.Dispose();

                EditorGUI.BeginChangeCheck();
                settings.Grasses = EditorGUILayout.ObjectField("Grasses", settings.Grasses, typeof(GGrassPrototypeGroup), false) as GGrassPrototypeGroup;
                settings.PatchGridSize = EditorGUILayout.IntField("Patch Grid Size", settings.PatchGridSize);
                settings.GrassSnapMode = (GSnapMode)EditorGUILayout.EnumPopup("Snap Mode", settings.GrassSnapMode);
                SerializedProperty sp1 = so.FindProperty("grassSnapLayerMask");
                if (settings.GrassSnapMode == GSnapMode.World && sp1 != null)
                {
                    EditorGUILayout.PropertyField(sp1, new GUIContent("Snap Layers"));
                }
                settings.EnableInteractiveGrass = EditorGUILayout.Toggle("Interactive Grass", settings.EnableInteractiveGrass);
                if (settings.EnableInteractiveGrass)
                {
                    settings.VectorFieldMapResolution = EditorGUILayout.DelayedIntField("Vector Field Map Resolution", settings.VectorFieldMapResolution);
                    settings.BendSensitive = EditorGUILayout.Slider("Bend Sensitive", settings.BendSensitive, 0f, 1f);
                    settings.RestoreSensitive = EditorGUILayout.Slider("Restore Sensitive", settings.RestoreSensitive, 0f, 1f);
                }

                GUI.enabled = false;
                EditorGUILayout.LabelField("Grass Instance Count", settings.GrassInstanceCount.ToString());

                string storageLabel =
                    settings.EstimatedGrassStorageMB < 1000 ? string.Format("{0} MB", settings.EstimatedGrassStorageMB.ToString("0.00")) :
                    string.Format("{0} GB", (settings.EstimatedGrassStorageMB / 1000f).ToString("0.00"));
                if (settings.EstimatedGrassStorageMB >= 100)
                {
                    storageLabel = string.Format("<color=red>{0}</color>", storageLabel);
                }

                string storageWarning = null;
                if (settings.EstimatedGrassStorageMB >= 100)
                {
                    storageWarning = "Grass storage is quite high, try reduce some instances for better save/load and serialization!";
                }

                EditorGUILayout.LabelField(new GUIContent("Estimated Storage"), new GUIContent(storageLabel, storageWarning), GEditorCommon.RichTextLabel);

                GUI.enabled = true;
                if (EditorGUI.EndChangeCheck())
                {
                    so.ApplyModifiedProperties();
                    data.SetDirty(GTerrainData.DirtyFlags.Foliage);
                    if (settings.EnableInteractiveGrass)
                    {
                        GAnalytics.Record(GAnalytics.ENABLE_INTERACTIVE_GRASS, true);
                    }
                }

                sp1.Dispose();
                so.Dispose();
            }, menu);
        }

        private void ConfirmAndResetFoliage()
        {
            if (EditorUtility.DisplayDialog(
                "Confirm",
                "Reset foliage data on this terrain? This action cannot be undone!",
                "OK", "Cancel"))
            {
                data.Foliage.ResetFull();
            }
        }

        private void ConfirmAndClearAllTrees()
        {
            if (EditorUtility.DisplayDialog(
                "Confirm",
                "Clear all trees on this terrain? This action cannot be undone!",
                "OK", "Cancel"))
            {
                data.Foliage.TreeInstances.Clear();
            }
        }

        private void ConfirmAndClearAllGrasses()
        {
            if (EditorUtility.DisplayDialog(
                "Confirm",
                "Clear all grasses on this terrain? This action cannot be undone!",
                "OK", "Cancel"))
            {
                data.Foliage.ClearGrassInstances();
                GGrassPatch[] patches = data.Foliage.GrassPatches;
                for (int i = 0; i < patches.Length; ++i)
                {
                    patches[i].UpdateMeshes();
                }
            }
        }

        private void DrawDataGUI()
        {
            string label = "Data";
            string id = "data" + data.GetInstanceID();

            GEditorCommon.Foldout(label, false, id, () =>
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Import", EditorStyles.miniButtonLeft))
                {
                    ShowImportContext();
                }
                if (GUILayout.Button("Export", EditorStyles.miniButtonRight))
                {
                    ShowExportContext();
                }
                EditorGUILayout.EndHorizontal();
            });
        }

        private void ShowImportContext()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(
                new GUIContent("Unity Terrain Data"),
                false,
                () =>
                {
                    ShowUnityTerrainDataImporter();
                });
#if POLARIS
                    menu.AddItem(
                        new GUIContent("Polaris V1 Terrain Data"),
                        false,
                        () =>
                        {
                            ShowPolarisV1TerrainDataImporter();
                        });
#else
            menu.AddDisabledItem(new GUIContent("Polaris V1 Terrain Data"));

#endif
            menu.AddItem(
                new GUIContent("Raw"),
                false,
                () =>
                {
                    ShowRawImporter();
                });
            menu.AddItem(
                new GUIContent("Textures"),
                false,
                () =>
                {
                    ShowTextureImporter();
                });

            menu.ShowAsContext();
        }

        private void ShowUnityTerrainDataImporter()
        {
            GUnityTerrainDataImporterWindow window = GUnityTerrainDataImporterWindow.ShowWindow();
            window.DesData = data;

            GameObject g = Selection.activeGameObject;
            if (g != null)
            {
                GStylizedTerrain t = g.GetComponent<GStylizedTerrain>();
                if (t != null && t.TerrainData == data)
                {
                    window.DesTerrain = t;
                }
            }
        }

#if POLARIS
        private void ShowPolarisV1TerrainDataImporter()
        {
            GPolarisV1DataImporterWindow window = GPolarisV1DataImporterWindow.ShowWindow();
            window.DesData = data;
        }
#endif

        private void ShowRawImporter()
        {
            GRawImporterWindow window = GRawImporterWindow.ShowWindow();
            window.DesData = data;
            GameObject g = Selection.activeGameObject;
            if (g != null)
            {
                GStylizedTerrain t = g.GetComponent<GStylizedTerrain>();
                if (t != null && t.TerrainData == data)
                {
                    window.Terrain = t;
                }
            }
        }

        private void ShowTextureImporter()
        {
            GTextureImporterWindow window = GTextureImporterWindow.ShowWindow();
            window.DesData = data;
            GameObject g = Selection.activeGameObject;
            if (g != null)
            {
                GStylizedTerrain t = g.GetComponent<GStylizedTerrain>();
                if (t != null && t.TerrainData == data)
                {
                    window.Terrain = t;
                }
            }
        }

        private void ShowExportContext()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(
                new GUIContent("Unity Terrain Data"),
                false,
                () =>
                {
                    ShowUnityTerrainDataExporter();
                });
            menu.AddItem(
                new GUIContent("Raw"),
                false,
                () =>
                {
                    ShowRawExporter();
                });
            menu.AddItem(
                new GUIContent("Textures"),
                false,
                () =>
                {
                    ShowTexturesExporter();
                });

            menu.ShowAsContext();
        }

        private void ShowUnityTerrainDataExporter()
        {
            GUnityTerrainDataExporterWindow window = GUnityTerrainDataExporterWindow.ShowWindow();
            window.SrcData = data;
        }

        private void ShowRawExporter()
        {
            GRawExporterWindow window = GRawExporterWindow.ShowWindow();
            window.SrcData = data;
        }

        private void ShowTexturesExporter()
        {
            GTextureExporterWindow window = GTextureExporterWindow.ShowWindow();
            window.SrcData = data;
        }

        private void DrawNeighboringGUI()
        {
            string label = "Neighboring";
            string id = "neighboring" + instance.GetInstanceID().ToString();

            GenericMenu menu = new GenericMenu();
            menu.AddItem(
                new GUIContent("Reset"),
                false,
                () => { ResetNeighboring(); });
            menu.AddItem(
                new GUIContent("Connect"),
                false,
                () => { GStylizedTerrain.ConnectAdjacentTiles(); });

            isNeighboringFoldoutExpanded = GEditorCommon.Foldout(label, false, id, () =>
             {
                 EditorGUI.BeginChangeCheck();
                 instance.AutoConnect = EditorGUILayout.Toggle("Auto Connect", instance.AutoConnect);
                 instance.GroupId = EditorGUILayout.DelayedIntField("Group Id", instance.GroupId);
                 instance.TopNeighbor = EditorGUILayout.ObjectField("Top Neighbor", instance.TopNeighbor, typeof(GStylizedTerrain), true) as GStylizedTerrain;
                 instance.BottomNeighbor = EditorGUILayout.ObjectField("Bottom Neighbor", instance.BottomNeighbor, typeof(GStylizedTerrain), true) as GStylizedTerrain;
                 instance.LeftNeighbor = EditorGUILayout.ObjectField("Left Neighbor", instance.LeftNeighbor, typeof(GStylizedTerrain), true) as GStylizedTerrain;
                 instance.RightNeighbor = EditorGUILayout.ObjectField("Right Neighbor", instance.RightNeighbor, typeof(GStylizedTerrain), true) as GStylizedTerrain;

                 if (EditorGUI.EndChangeCheck())
                 {
                     if (instance.TopNeighbor != null || instance.BottomNeighbor != null || instance.LeftNeighbor != null || instance.RightNeighbor != null)
                     {
                         GAnalytics.Record(GAnalytics.MULTI_TERRAIN, true);
                     }
                 }
             }, menu);
        }

        public static string GetNeighboringFoldoutID(GStylizedTerrain t)
        {
            string id = "neighboring" + t.GetInstanceID().ToString();
            return id;
        }

        public override bool RequiresConstantRepaint()
        {
            return false;
        }

        private void DuringSceneGUI(SceneView sv)
        {
            if (instance.TerrainData == null)
                return;
            if (!instance.AutoConnect)
                return;
            if (!isNeighboringFoldoutExpanded)
                return;

            Vector3 terrainSizeXZ = new Vector3(
                instance.TerrainData.Geometry.Width,
                0,
                instance.TerrainData.Geometry.Length);

            if (instance.LeftNeighbor == null)
            {
                Vector3 pos = instance.transform.position + Vector3.left * terrainSizeXZ.x + terrainSizeXZ * 0.5f;
                if (Handles.Button(pos, Quaternion.Euler(90, 0, 0), terrainSizeXZ.z * 0.5f, terrainSizeXZ.z * 0.5f, Handles.RectangleHandleCap) && Event.current.button == 0)
                {
                    GStylizedTerrain t = CreateNeighborTerrain();
                    t.transform.parent = instance.transform.parent;
                    t.transform.position = instance.transform.position + Vector3.left * terrainSizeXZ.x;
                    t.name = string.Format("{0}-{1}", t.name, t.transform.position.ToString());
                    Selection.activeGameObject = t.gameObject;
                    GStylizedTerrain.ConnectAdjacentTiles();
                }
            }
            if (instance.TopNeighbor == null)
            {
                Vector3 pos = instance.transform.position + Vector3.forward * terrainSizeXZ.z + terrainSizeXZ * 0.5f;
                if (Handles.Button(pos, Quaternion.Euler(90, 0, 0), terrainSizeXZ.z * 0.5f, terrainSizeXZ.z * 0.5f, Handles.RectangleHandleCap) && Event.current.button == 0)
                {
                    GStylizedTerrain t = CreateNeighborTerrain();
                    t.transform.parent = instance.transform.parent;
                    t.transform.position = instance.transform.position + Vector3.forward * terrainSizeXZ.z;
                    t.name = string.Format("{0}-{1}", t.name, t.transform.position.ToString());
                    Selection.activeGameObject = t.gameObject;
                    GStylizedTerrain.ConnectAdjacentTiles();
                }
            }
            if (instance.RightNeighbor == null)
            {
                Vector3 pos = instance.transform.position + Vector3.right * terrainSizeXZ.z + terrainSizeXZ * 0.5f;
                if (Handles.Button(pos, Quaternion.Euler(90, 0, 0), terrainSizeXZ.z * 0.5f, terrainSizeXZ.z * 0.5f, Handles.RectangleHandleCap) && Event.current.button == 0)
                {
                    GStylizedTerrain t = CreateNeighborTerrain();
                    t.transform.parent = instance.transform.parent;
                    t.transform.position = instance.transform.position + Vector3.right * terrainSizeXZ.x;
                    t.name = string.Format("{0}-{1}", t.name, t.transform.position.ToString());
                    Selection.activeGameObject = t.gameObject;
                    GStylizedTerrain.ConnectAdjacentTiles();
                }
            }
            if (instance.BottomNeighbor == null)
            {
                Vector3 pos = instance.transform.position + Vector3.back * terrainSizeXZ.z + terrainSizeXZ * 0.5f;
                if (Handles.Button(pos, Quaternion.Euler(90, 0, 0), terrainSizeXZ.z * 0.5f, terrainSizeXZ.z * 0.5f, Handles.RectangleHandleCap) && Event.current.button == 0)
                {
                    GStylizedTerrain t = CreateNeighborTerrain();
                    t.transform.parent = instance.transform.parent;
                    t.transform.position = instance.transform.position + Vector3.back * terrainSizeXZ.z;
                    t.name = string.Format("{0}-{1}", t.name, t.transform.position.ToString());
                    Selection.activeGameObject = t.gameObject;
                    GStylizedTerrain.ConnectAdjacentTiles();
                }
            }
        }

        private GStylizedTerrain CreateNeighborTerrain()
        {
            GStylizedTerrain t = GEditorMenus.CreateStylizedTerrain(instance.TerrainData);
            GEditorCommon.ExpandFoldout(GetNeighboringFoldoutID(t));
            if (instance.TerrainData != null && instance.TerrainData.Shading.CustomMaterial != null)
            {
                t.TerrainData.Shading.CustomMaterial.shader = instance.TerrainData.Shading.CustomMaterial.shader;
                t.TerrainData.Shading.UpdateMaterials();
            }

            return t;
        }

        private void ResetNeighboring()
        {
            instance.AutoConnect = true;
            instance.GroupId = 0;
            instance.TopNeighbor = null;
            instance.BottomNeighbor = null;
            instance.LeftNeighbor = null;
            instance.RightNeighbor = null;
        }

        private void InjectGUI(int order)
        {
            if (GUIInject != null)
            {
                GUIInject.Invoke(instance, order);
            }
        }
    }
}
