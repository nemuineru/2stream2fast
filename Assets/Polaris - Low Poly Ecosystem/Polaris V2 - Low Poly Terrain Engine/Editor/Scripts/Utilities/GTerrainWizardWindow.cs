using UnityEditor;
using UnityEngine;

namespace Pinwheel.Griffin
{
    public class GTerrainWizardWindow : EditorWindow
    {
        private MenuCommand menuCmd;
        private GTerrainData terrainData;

        private const int MODE_CREATE = 0;
        private const int MODE_SET_SHADER = 1;

        private int mode = 0;

        public bool BulkSetShader { get; set; }
        public int BulkSetShaderGroupId { get; set; }

        public static void ShowWindow(MenuCommand menuCmd = null)
        {
            GTerrainWizardWindow window = ScriptableObject.CreateInstance<GTerrainWizardWindow>();
            window.titleContent = new GUIContent(GVersionInfo.ProductNameAndVersionShort);
            window.minSize = new Vector2(400, 100);
            window.menuCmd = menuCmd;
            window.mode = MODE_CREATE;
            window.ShowUtility();
        }

        public static void ShowWindowSetShaderMode(GTerrainData terrainData)
        {
            if (terrainData == null)
                return;
            GTerrainWizardWindow window = ScriptableObject.CreateInstance<GTerrainWizardWindow>();
            window.titleContent = new GUIContent(GVersionInfo.ProductNameAndVersionShort);
            window.minSize = new Vector2(400, 100);
            window.terrainData = terrainData;
            window.mode = MODE_SET_SHADER;
            window.ShowUtility();
        }

        public static void ShowWindowSetShaderForTerrainGroupMode(int groupId)
        {
            GTerrainWizardWindow window = ScriptableObject.CreateInstance<GTerrainWizardWindow>();
            window.titleContent = new GUIContent(GVersionInfo.ProductNameAndVersionShort);
            window.minSize = new Vector2(400, 100);
            window.maxSize = new Vector2(401, 101);
            window.mode = MODE_SET_SHADER;
            window.BulkSetShader = true;
            window.BulkSetShaderGroupId = groupId;
            window.ShowUtility();
        }

        private void OnGUI()
        {
            if (mode == MODE_CREATE)
            {
                OnGUICreateTerrain();
            }
            else if (mode == MODE_SET_SHADER)
            {
                OnGUISetShader();
            }
        }

        private void DrawMaterialSettings()
        {
            EditorGUILayout.LabelField("Render Pipeline", GCommon.CurrentRenderPipeline.ToString());
            GCreateTerrainWizardSettings settings = GGriffinSettings.Instance.WizardSettings;
            GUI.enabled = GCommon.CurrentRenderPipeline == GRenderPipelineType.Builtin;
            settings.LightingModel = (GLightingModel)EditorGUILayout.EnumPopup("Lighting Model", settings.LightingModel);
            GUI.enabled = true;
            settings.TexturingModel = (GTexturingModel)EditorGUILayout.EnumPopup("Texturing Model", settings.TexturingModel);
            if (settings.TexturingModel == GTexturingModel.Splat)
            {
                settings.SplatsModel = (GSplatsModel)EditorGUILayout.EnumPopup("Splats Model", settings.SplatsModel);
            }

            GGriffinSettings.Instance.WizardSettings = settings;
        }

        private void OnGUICreateTerrain()
        {
            DrawMaterialSettings();

            if (GUILayout.Button("Create"))
            {
                RecordCreateTerrainAnalytics();
                RecordShadingAnalytics();
                GEditorMenus.CreateStylizedTerrain(menuCmd);
                Close();
            }
        }

        private void OnGUISetShader()
        {
            DrawMaterialSettings();

            if (GUILayout.Button("Set Shader"))
            {
                RecordSetShaderAnalytics();
                RecordShadingAnalytics();
                Material mat = GGriffinSettings.Instance.WizardSettings.GetClonedMaterial();
                if (BulkSetShader)
                {
                    GCommon.ForEachTerrain(
                        BulkSetShaderGroupId,
                        (t) =>
                        {
                            if (t.TerrainData == null)
                                return;
                            t.TerrainData.Shading.MaterialToRender.shader = mat.shader;
                            t.TerrainData.Shading.UpdateMaterials();
                            t.TerrainData.SetDirty(GTerrainData.DirtyFlags.Shading);
                        });
                }
                else
                {
                    terrainData.Shading.MaterialToRender.shader = mat.shader;
                    terrainData.Shading.UpdateMaterials();
                }
                GUtilities.DestroyObject(mat);
                Close();
            }
        }

        private void RecordCreateTerrainAnalytics()
        {
            GAnalytics.Record(GAnalytics.WIZARD_CREATE_TERRAIN);
        }

        private void RecordSetShaderAnalytics()
        {
            GAnalytics.Record(GAnalytics.WIZARD_SET_SHADER);
        }

        private void RecordShadingAnalytics()
        {
            GTexturingModel texturing = GGriffinSettings.Instance.WizardSettings.TexturingModel;
            string url =
                texturing == GTexturingModel.ColorMap ? GAnalytics.SHADING_COLOR_MAP :
                texturing == GTexturingModel.GradientLookup ? GAnalytics.SHADING_GRADIENT_LOOKUP :
                texturing == GTexturingModel.Splat ? GAnalytics.SHADING_SPLAT :
                texturing == GTexturingModel.VertexColor ? GAnalytics.SHADING_VERTEX_COLOR : string.Empty;
            GAnalytics.Record(url);
        }
    }
}
