using Pinwheel.Griffin.BackupTool;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pinwheel.Griffin.DataTool
{
    public class GUnityTerrainDataImporterWindow : EditorWindow
    {
        public Terrain SrcTerrain { get; set; }
        public TerrainData SrcData { get; set; }
        public GTerrainData DesData { get; set; }
        public GStylizedTerrain DesTerrain { get; set; }
        public bool ImportGeometry { get; set; }
        public bool UseUnityTerrainSize { get; set; }
        public bool ImportSplats { get; set; }
        public bool ImportSplatsAsAlbedo { get; set; }
        public bool CreateNewSplatPrototypesGroup { get; set; }
        public bool ImportTrees { get; set; }
        public bool CreateNewTreePrototypesGroup { get; set; }
        public bool ImportGrasses { get; set; }
        public bool CreateNewGrassPrototypesGroup { get; set; }
        public float GrassDensity { get; set; }

        public bool BulkImport { get; set; }
        public int BulkImportGroupId { get; set; }
        public string Directory { get; set; }

        private const string HISTORY_PREFIX = "Import Unity Terrain";
        private const string PREF_PREFIX = "unity-terrain-importer";
        private const string IMPORT_GEOMETRY_PREF_KEY = "import-geometry";
        private const string USE_UNITY_TERRAIN_SIZE_PREF_KEY = "use-unity-terrain-size";
        private const string IMPORT_SPLATS_PREF_KEY = "import-splats";
        private const string IMPORT_SPLATS_AS_ALBEDO_PREF_KEY = "import-splats-as-albedo";
        private const string NEW_SPLATS_GROUP_PREF_KEY = "new-splats-group";
        private const string IMPORT_TREES_PREF_KEY = "import-trees";
        private const string NEW_TREES_GROUP_PREF_KEY = "new-trees-group";
        private const string IMPORT_GRASS_PREF_KEY = "import-grasses";
        private const string NEW_GRASSES_GROUP_PREF_KEY = "new-grasses-group";
        private const string GRASS_DENSITY = "grass-density";
        private const string DIRECTORY_PREF_KEY = "directory";

        private const string INSTRUCTION =
            "Import data from Unity Terrain Data.\n" +
            "Note that this tool only do the import, other configuration like mesh resolution, material settings, etc. must be perform manually.\n" +
            "Sometime you can see splat textures are not rendered correctly, this is caused mostly because there are more splat textures than the material can support!";

        private long estimatedGrassStorage = 0;

        public static GUnityTerrainDataImporterWindow ShowWindow()
        {
            GUnityTerrainDataImporterWindow window = ScriptableObject.CreateInstance<GUnityTerrainDataImporterWindow>();
            window.titleContent = new GUIContent("Unity Terrain Data Importer");
            window.ShowUtility();
            return window;
        }

        private void OnEnable()
        {
            ImportGeometry = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_GEOMETRY_PREF_KEY), true);
            UseUnityTerrainSize = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, USE_UNITY_TERRAIN_SIZE_PREF_KEY), true);
            ImportSplats = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_SPLATS_PREF_KEY), true);
            ImportSplatsAsAlbedo = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_SPLATS_AS_ALBEDO_PREF_KEY), false);
            CreateNewSplatPrototypesGroup = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, NEW_SPLATS_GROUP_PREF_KEY), false);
            ImportTrees = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_TREES_PREF_KEY), true);
            CreateNewTreePrototypesGroup = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, NEW_TREES_GROUP_PREF_KEY), false);
            ImportGrasses = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_GRASS_PREF_KEY), false);
            CreateNewGrassPrototypesGroup = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, NEW_GRASSES_GROUP_PREF_KEY), false);
            GrassDensity = EditorPrefs.GetFloat(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, GRASS_DENSITY), 0.5f);
            Directory = EditorPrefs.GetString(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, DIRECTORY_PREF_KEY), string.Empty);

            CalculateEstimatedGrassStorage();
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_GEOMETRY_PREF_KEY), ImportGeometry);
            EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, USE_UNITY_TERRAIN_SIZE_PREF_KEY), UseUnityTerrainSize);
            EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_SPLATS_PREF_KEY), ImportSplats);
            EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_SPLATS_AS_ALBEDO_PREF_KEY), ImportSplatsAsAlbedo);
            EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, NEW_SPLATS_GROUP_PREF_KEY), CreateNewSplatPrototypesGroup);
            EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_TREES_PREF_KEY), ImportTrees);
            EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, NEW_TREES_GROUP_PREF_KEY), CreateNewTreePrototypesGroup);
            EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_GRASS_PREF_KEY), ImportGrasses);
            EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, NEW_GRASSES_GROUP_PREF_KEY), CreateNewGrassPrototypesGroup);
            EditorPrefs.SetFloat(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, GRASS_DENSITY), GrassDensity);
            EditorPrefs.SetString(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, DIRECTORY_PREF_KEY), Directory);
        }

        private void OnGUI()
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200;
            DrawInstructionGUI();
            DrawImportGUI();
            EditorGUIUtility.labelWidth = labelWidth;
            HandleRepaint();
        }

        private void HandleRepaint()
        {
            //if (Event.current != null)
            //{
            //    Repaint();
            //}
        }

        private void DrawInstructionGUI()
        {
            string label = "Instruction";
            string id = "unity-terrain-importer-instruction";

            GEditorCommon.Foldout(label, false, id, () =>
            {
                EditorGUILayout.LabelField(INSTRUCTION, GEditorCommon.WordWrapItalicLabel);
            });
        }

        private void DrawImportGUI()
        {
            string label = "Import";
            string id = "unity-terrain-importer-import";

            GEditorCommon.Foldout(label, true, id, () =>
            {
                if (BulkImport)
                {

                }
                else
                {
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField("Terrain", DesTerrain, typeof(GStylizedTerrain), true);
                    EditorGUILayout.ObjectField("Griffin Data", DesData, typeof(GTerrainData), false);
                    GUI.enabled = true;

                    EditorGUI.BeginChangeCheck();
                    SrcTerrain = EditorGUILayout.ObjectField("Unity Terrain", SrcTerrain, typeof(Terrain), true) as Terrain;
                    if (SrcTerrain != null)
                    {
                        SrcData = SrcTerrain.terrainData;
                    }

                    GUI.enabled = SrcTerrain == null;
                    SrcData = EditorGUILayout.ObjectField("Unity Terrain Data", SrcData, typeof(TerrainData), true) as TerrainData;
                    GUI.enabled = true;

                    if (SrcData != null)
                    {
                        if (SrcTerrain != null && SrcTerrain.terrainData != SrcData)
                        {
                            SrcTerrain = null;
                        }
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        CalculateEstimatedGrassStorage();
                        AdjustGrassDensity();
                    }

                    if (SrcData == null)
                        return;
                }

                ImportGeometry = EditorGUILayout.Toggle("Import Geometry", ImportGeometry);
                if (ImportGeometry)
                {
                    EditorGUI.indentLevel += 1;
                    UseUnityTerrainSize = EditorGUILayout.Toggle("Unity Terrain Size", UseUnityTerrainSize);
                    EditorGUI.indentLevel -= 1;
                }

                ImportSplats = EditorGUILayout.Toggle("Import Splats", ImportSplats);
                if (ImportSplats)
                {
                    EditorGUI.indentLevel += 1;
                    ImportSplatsAsAlbedo = EditorGUILayout.Toggle("As Albedo", ImportSplatsAsAlbedo);

                    if (BulkImport)
                    {
                        CreateNewSplatPrototypesGroup = EditorGUILayout.Toggle("New Splats Group", CreateNewSplatPrototypesGroup);
                    }
                    else
                    {
                        if (DesData.Shading.Splats == null ||
                            DesData.Shading.Splats == GGriffinSettings.Instance.TerrainDataDefault.Shading.Splats)
                        {
                            CreateNewSplatPrototypesGroup = true;
                        }
                        GUI.enabled = DesData.Shading.Splats != null && DesData.Shading.Splats != GGriffinSettings.Instance.TerrainDataDefault.Shading.Splats;
                        CreateNewSplatPrototypesGroup = EditorGUILayout.Toggle("New Splats Group", CreateNewSplatPrototypesGroup);
                        GUI.enabled = true;
                    }
                    EditorGUI.indentLevel -= 1;
                }

                ImportTrees = EditorGUILayout.Toggle("Import Trees", ImportTrees);
                if (ImportTrees)
                {
                    EditorGUI.indentLevel += 1;

                    if (BulkImport)
                    {
                        CreateNewTreePrototypesGroup = EditorGUILayout.Toggle("New Trees Group", CreateNewTreePrototypesGroup);
                    }
                    else
                    {
                        if (DesData.Foliage.Trees == null ||
                            DesData.Foliage.Trees == GGriffinSettings.Instance.TerrainDataDefault.Foliage.Trees)
                        {
                            CreateNewTreePrototypesGroup = true;
                        }
                        GUI.enabled = DesData.Foliage.Trees != null && DesData.Foliage.Trees != GGriffinSettings.Instance.TerrainDataDefault.Foliage.Trees;
                        CreateNewTreePrototypesGroup = EditorGUILayout.Toggle("New Trees Group", CreateNewTreePrototypesGroup);
                        GUI.enabled = true;
                    }
                    EditorGUI.indentLevel -= 1;
                }

                ImportGrasses = EditorGUILayout.Toggle("Import Grasses & Details", ImportGrasses);
                if (ImportGrasses)
                {
                    EditorGUI.indentLevel += 1;

                    if (BulkImport)
                    {
                        CreateNewGrassPrototypesGroup = EditorGUILayout.Toggle("New Grasses Group", CreateNewGrassPrototypesGroup);
                    }
                    else
                    {
                        if (DesData.Foliage.Grasses == null ||
                            DesData.Foliage.Grasses == GGriffinSettings.Instance.TerrainDataDefault.Foliage.Grasses)
                        {
                            CreateNewGrassPrototypesGroup = true;
                        }
                        GUI.enabled = DesData.Foliage.Grasses != null && DesData.Foliage.Grasses != GGriffinSettings.Instance.TerrainDataDefault.Foliage.Grasses;
                        CreateNewGrassPrototypesGroup = EditorGUILayout.Toggle("New Grasses Group", CreateNewGrassPrototypesGroup);
                        GUI.enabled = true;
                        EditorGUI.BeginChangeCheck();
                        GrassDensity = EditorGUILayout.Slider("Density", GrassDensity, 0f, 1f);
                        if (EditorGUI.EndChangeCheck())
                        {
                            CalculateEstimatedGrassStorage();
                        }
                        GUI.enabled = false;
                        string storageLabel = string.Format(
                            "{0} MB",
                            (estimatedGrassStorage / 1000000f).ToString("0.00"));
                        if (estimatedGrassStorage > 1000000000)
                        {
                            float gb = estimatedGrassStorage / 1000000000f;
                            storageLabel += " = " + gb.ToString("0.00") + " GB";
                        }

                        if (estimatedGrassStorage > 100 * 1000000)
                        {
                            storageLabel = string.Format("<color=red>{0}</color>", storageLabel);
                        }

                        EditorGUILayout.LabelField("Estimated Storage", storageLabel, GEditorCommon.RichTextLabel);
                        GUI.enabled = true;
                    }
                    EditorGUI.indentLevel -= 1;
                }

                if (BulkImport)
                {
                    string dir = Directory;
                    GEditorCommon.BrowseFolder("Directory", ref dir);
                    Directory = dir;

                    EditorGUILayout.LabelField("File Name Convention", "TerrainData_<Polaris Terrain Data Id>", GEditorCommon.WordWrapItalicLabel);
                }

                if (GUILayout.Button("Import"))
                {
                    if (!BulkImport && ImportGrasses && estimatedGrassStorage > 100 * 1000000)
                    {
                        ConfirmAndImport();
                    }
                    else
                    {
                        GAnalytics.Record(GAnalytics.IMPORT_UNITY_TERRAIN_DATA);
                        Import();
                    }
                }
            });
        }

        private void ConfirmAndImport()
        {
            int selection = EditorUtility.DisplayDialogComplex(
                "Confirm",
                "Grasses and Details storage memory is too large, you may encounter:\n" +
                "- Extremely long time to save project.\n" +
                "- Serialization issues, long or unable to load player.\n" +
                "- System crash.\n" +
                "Consider lowering Grass Density to reduce memory usage!",
                "Continue Anyway", "Cancel", "Fix it for me, please!");
            if (selection == 0) //OK
            {
                GAnalytics.Record(GAnalytics.IMPORT_UNITY_TERRAIN_DATA);
                Import();
            }
            else if (selection == 2) //Alt
            {
                AdjustGrassDensity();
            }
        }

        private void CalculateEstimatedGrassStorage()
        {
            estimatedGrassStorage = GUnityTerrainDataImporter.CalculateEstimatedGrassStorage(SrcData, GrassDensity);
        }

        private void AdjustGrassDensity()
        {
            GrassDensity = GUnityTerrainDataImporter.GetAdjustedGrassDensity(SrcData, GrassDensity);
            CalculateEstimatedGrassStorage();
        }

        private void Import()
        {
            if (BulkImport)
            {
                DoBulkImport();
            }
            else
            {
                DoImport();
            }
        }

        private void DoImport()
        {
            if (DesTerrain != null)
            {
                GBackup.TryCreateInitialBackup(HISTORY_PREFIX, DesTerrain, GCommon.AllResourceFlags);
            }

            GUnityTerrainDataImporter importer = new GUnityTerrainDataImporter();
            importer.SrcTerrain = SrcTerrain;
            importer.SrcData = SrcData;
            importer.DesData = DesData;
            importer.DesTerrain = DesTerrain;
            importer.ImportGeometry = ImportGeometry;
            importer.UseUnityTerrainSize = UseUnityTerrainSize;
            importer.ImportSplats = ImportSplats;
            importer.ImportSplatsAsAlbedo = ImportSplatsAsAlbedo;
            importer.CreateNewSplatPrototypesGroup = CreateNewSplatPrototypesGroup;
            importer.ImportTrees = ImportTrees;
            importer.CreateNewTreePrototypesGroup = CreateNewTreePrototypesGroup;
            importer.ImportGrasses = ImportGrasses;
            importer.CreateNewGrassPrototypesGroup = CreateNewGrassPrototypesGroup;
            importer.GrassDensity = GrassDensity;
            importer.Import();

            if (DesTerrain != null)
            {
                GBackup.TryCreateBackup(HISTORY_PREFIX, DesTerrain, GCommon.AllResourceFlags);
            }
        }

        private void DoBulkImport()
        {
            string[] guid = AssetDatabase.FindAssets("t:TerrainData", new string[] { Directory });
            List<TerrainData> terrainDatas = new List<TerrainData>();
            for (int i = 0; i < guid.Length; ++i)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid[i]);
                TerrainData data = AssetDatabase.LoadAssetAtPath<TerrainData>(path);
                terrainDatas.Add(data);
            }

            GCommon.ForEachTerrain(
                BulkImportGroupId,
                (t) =>
                {
                    if (t == null || t.TerrainData == null)
                        return;
                    TerrainData srcData = terrainDatas.Find(d => d.name.StartsWith("TerrainData") && d.name.EndsWith(t.TerrainData.Id));
                    if (srcData == null)
                        return;

                    GBackup.TryCreateInitialBackup(HISTORY_PREFIX, t, GCommon.AllResourceFlags);

                    GUnityTerrainDataImporter importer = new GUnityTerrainDataImporter();
                    importer.SrcTerrain = null;
                    importer.SrcData = srcData;
                    importer.DesData = t.TerrainData;
                    importer.DesTerrain = t;
                    importer.ImportGeometry = ImportGeometry;
                    importer.UseUnityTerrainSize = UseUnityTerrainSize;

                    importer.ImportSplats = ImportSplats;
                    importer.ImportSplatsAsAlbedo = ImportSplatsAsAlbedo;
                    bool createNewSplatGroup = CreateNewSplatPrototypesGroup;
                    if (t.TerrainData.Shading.Splats == null ||
                        t.TerrainData.Shading.Splats == GGriffinSettings.Instance.TerrainDataDefault.Shading.Splats)
                    {
                        createNewSplatGroup = true;
                    }
                    importer.CreateNewSplatPrototypesGroup = createNewSplatGroup;

                    importer.ImportTrees = ImportTrees;
                    bool createNewTreeGroup = CreateNewTreePrototypesGroup;
                    if (t.TerrainData.Foliage.Trees == null ||
                        t.TerrainData.Foliage.Trees == GGriffinSettings.Instance.TerrainDataDefault.Foliage.Trees)
                    {
                        createNewTreeGroup = true;
                    }
                    importer.CreateNewTreePrototypesGroup = createNewTreeGroup;

                    importer.ImportGrasses = ImportGrasses;
                    bool createNewGrassGroup = CreateNewGrassPrototypesGroup;
                    if (t.TerrainData.Foliage.Grasses == null ||
                        t.TerrainData.Foliage.Grasses == GGriffinSettings.Instance.TerrainDataDefault.Foliage.Grasses)
                    {
                        createNewGrassGroup = true;
                    }
                    importer.CreateNewGrassPrototypesGroup = createNewGrassGroup;

                    GrassDensity = GUnityTerrainDataImporter.GetAdjustedGrassDensity(srcData);
                    importer.GrassDensity = GrassDensity;
                    importer.Import();

                    GBackup.TryCreateBackup(HISTORY_PREFIX, t, GCommon.AllResourceFlags);
                });

            GStylizedTerrain.MatchEdges(BulkImportGroupId);
        }
    }
}
