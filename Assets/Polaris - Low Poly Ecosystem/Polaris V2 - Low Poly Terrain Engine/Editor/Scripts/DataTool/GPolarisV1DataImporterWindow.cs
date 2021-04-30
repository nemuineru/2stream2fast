#if POLARIS
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Pinwheel.Polaris;

namespace Pinwheel.Griffin.DataTool
{
    public class GPolarisV1DataImporterWindow : EditorWindow
    {
        public LPTData SrcData { get; set; }
        public GTerrainData DesData { get; set; }
        public bool ImportGeometry { get; set; }
        public bool UsePolarisV1TerrainSize { get; set; }
        public bool ImportColors { get; set; }
        public bool ImportSplats { get; set; }
        public bool CreateNewSplatPrototypesGroup { get; set; }

        private const string PREF_PREFIX = "polarisv1-terrain-importer";
        private const string IMPORT_GEOMETRY_PREF_KEY = "import-geometry";
        private const string USE_POLARISV1_TERRAIN_SIZE_PREF_KEY = "use-polaris-terrain-size";
        private const string IMPORT_COLORS_PREF_KEY = "import-color";
        private const string IMPORT_SPLATS_PREF_KEY = "import-splats";
        private const string NEW_SPLATS_GROUP_PREF_KEY = "new-splats-group";

        private const string INSTRUCTION =
            "Import data from Polaris V1 Terrain Data.\n" +
            "Note that this tool only do the import, other configuration like mesh resolution, material settings, etc. must be perform manually.\n" +
            "Sometime you can see splat textures are not rendered correctly, this is caused mostly because there are more splat textures than the material can support!";

        public static GPolarisV1DataImporterWindow ShowWindow()
        {
            GPolarisV1DataImporterWindow window = ScriptableObject.CreateInstance<GPolarisV1DataImporterWindow>();
            window.titleContent = new GUIContent("Polaris V1 Terrain Data Importer");
            window.ShowUtility();
            return window;
        }

        private void OnEnable()
        {
            ImportGeometry = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_GEOMETRY_PREF_KEY), true);
            UsePolarisV1TerrainSize = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, USE_POLARISV1_TERRAIN_SIZE_PREF_KEY), true);
            ImportColors = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_COLORS_PREF_KEY), true);
            ImportSplats = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_SPLATS_PREF_KEY), true);
            CreateNewSplatPrototypesGroup = EditorPrefs.GetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, NEW_SPLATS_GROUP_PREF_KEY), false);
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_GEOMETRY_PREF_KEY), ImportGeometry);
            EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, USE_POLARISV1_TERRAIN_SIZE_PREF_KEY), UsePolarisV1TerrainSize);
            EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_COLORS_PREF_KEY), ImportColors);
            EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, IMPORT_SPLATS_PREF_KEY), ImportSplats);
            EditorPrefs.SetBool(GEditorCommon.GetProjectRelatedEditorPrefsKey(PREF_PREFIX, NEW_SPLATS_GROUP_PREF_KEY), CreateNewSplatPrototypesGroup);
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
            string id = "polarisv1-terrain-importer-instruction";

            GEditorCommon.Foldout(label, false, id, () =>
            {
                EditorGUILayout.LabelField(INSTRUCTION, GEditorCommon.WordWrapItalicLabel);
            });
        }

        private void DrawImportGUI()
        {
            string label = "Import";
            string id = "polarisv1-terrain-importer-import";

            GEditorCommon.Foldout(label, true, id, () =>
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Griffin Data", DesData, typeof(GTerrainData), false);
                GUI.enabled = true;
                
                SrcData = EditorGUILayout.ObjectField("Polaris V1 Terrain Data", SrcData, typeof(LPTData), true) as LPTData;
                if (SrcData == null)
                    return;

                ImportGeometry = EditorGUILayout.Toggle("Import Geometry", ImportGeometry);
                if (ImportGeometry)
                {
                    EditorGUI.indentLevel += 1;
                    UsePolarisV1TerrainSize = EditorGUILayout.Toggle("Unity Polaris V1 Size", UsePolarisV1TerrainSize);
                    EditorGUI.indentLevel -= 1;
                }

                ImportColors = EditorGUILayout.Toggle("Import Colors", ImportColors);
                ImportSplats = EditorGUILayout.Toggle("Import Splats", ImportSplats);
                if (ImportSplats)
                {
                    EditorGUI.indentLevel += 1;
                    if (DesData.Shading.Splats == null ||
                        DesData.Shading.Splats == GGriffinSettings.Instance.TerrainDataDefault.Shading.Splats)
                    {
                        CreateNewSplatPrototypesGroup = true;
                    }
                    GUI.enabled = DesData.Shading.Splats != null && DesData.Shading.Splats != GGriffinSettings.Instance.TerrainDataDefault.Shading.Splats;
                    CreateNewSplatPrototypesGroup = EditorGUILayout.Toggle("New Splats Group", CreateNewSplatPrototypesGroup);
                    GUI.enabled = true;
                    EditorGUI.indentLevel -= 1;
                }

                if (GUILayout.Button("Import"))
                {
                    GAnalytics.Record(GAnalytics.IMPORT_POLARIS_V1_DATA);
                    Import();
                }
            });
        }

        private void Import()
        {
            GPolarisV1DataImporter importer = new GPolarisV1DataImporter();
            importer.SrcData = SrcData;
            importer.DesData = DesData;
            importer.ImportGeometry = ImportGeometry;
            importer.UsePolarisV1TerrainSize = UsePolarisV1TerrainSize;
            importer.ImportColors = ImportColors;
            importer.ImportSplats = ImportSplats;
            importer.Import();
        }
    }
}
#endif