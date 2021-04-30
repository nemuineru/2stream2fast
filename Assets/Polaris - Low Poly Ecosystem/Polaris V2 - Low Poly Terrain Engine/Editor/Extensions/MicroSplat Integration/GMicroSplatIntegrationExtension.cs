#if GRIFFIN && UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
#if __MICROSPLAT__
using JBooth.MicroSplat;
#endif

namespace Pinwheel.Griffin.MicroSplat.GriffinExtension
{
    public static class GMicroSplatIntegrationExtension
    {
        public static string GetExtensionName()
        {
            return "MicroSplat Integration";
        }

        public static string GetPublisherName()
        {
            return "Pinwheel Studio";
        }

        public static string GetDescription()
        {
            return "Provide support and make it easier to use MicroSplat shaders on Polaris terrain.";
        }

        public static string GetVersion()
        {
            return "1.0.0";
        }

        public static void OpenUserGuide()
        {
            Application.OpenURL("https://docs.google.com/document/d/1LQooyrEl2S5qP3w2cvX0RYy1CQvUs6mIBACJ8wNhSnE/edit#heading=h.1mgw1o27bmpg");
        }

        public static void OpenSupportLink()
        {
            GEditorCommon.OpenEmailEditor(
                GCommon.SUPPORT_EMAIL,
                "[Polaris V2] MicroSplat Integration",
                "YOUR_MESSAGE_HERE");
        }

        public static void OnGUI()
        {
            if (GUILayout.Button("Get MicroSplat Core Module"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/terrain/microsplat-96478");
            }
            if (GUILayout.Button("Get Polaris Integration Module"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/slug/166851");
            }
        }

#if __MICROSPLAT_POLARIS__
        [DidReloadScripts]
        public static void OnScriptReload()
        {
            GStylizedTerrainInspector.GUIInject += InjectTerrainGUI;
        }

        private static void InjectTerrainGUI(GStylizedTerrain terrain, int order)
        {
            if (order != 3)
                return;
            string label = "MicroSplat Integration";
            string id = "terrain-gui-microsplat-integration";
            GEditorCommon.Foldout(label, false, id, () =>
            {
                bool useMS = terrain.TerrainData.Shading.ShadingSystem == GShadingSystem.MicroSplat;
                useMS = EditorGUILayout.Toggle("Enable", useMS);
                terrain.TerrainData.Shading.ShadingSystem =
                    useMS ?
                    GShadingSystem.MicroSplat :
                    GShadingSystem.Polaris;

                GEditorCommon.Separator();
                EditorGUILayout.LabelField("Setup", EditorStyles.boldLabel);

                GMicroSplatIntegrationSettings settings = GMicroSplatIntegrationSettings.Instance;
                string dir = settings.DataDirectory;
                GEditorCommon.BrowseFolderMiniButton("Data Directory", ref dir);
                settings.DataDirectory = dir;
                settings.ShaderNamePrefix = EditorGUILayout.TextField("Shader Name Prefix", settings.ShaderNamePrefix);
                if (GUILayout.Button("Setup"))
                {
                    MicroSplatPolarisMesh pm = terrain.gameObject.GetComponent<MicroSplatPolarisMesh>();
                    if (pm != null)
                    {
                        GUtilities.DestroyObject(pm);
                    }

                    MeshRenderer[] renderers = terrain.Internal_ChunkRoot.GetComponentsInChildren<MeshRenderer>();
                    List<MeshRenderer> rendererList = new List<MeshRenderer>();
                    rendererList.AddRange(renderers);

                    MicroSplatPolarisMeshEditor.PolarisData data = new MicroSplatPolarisMeshEditor.PolarisData();
                    data.basePath = settings.DataDirectory;
                    data.name = string.Format("{0}_{1}", settings.ShaderNamePrefix, terrain.TerrainData.Id);
                    data.additionalKeywords = new string[0];
                    data.rootObject = terrain.gameObject;
                    data.renderers = rendererList;
                    MicroSplatPolarisMeshEditor.Setup(data);
                    terrain.PushControlTexturesToMicroSplat();

                    if (terrain.TerrainData.Shading.Splats == null ||
                        terrain.TerrainData.Shading.Splats.Prototypes.Count == 0)
                    {
                        return;
                    }

                    pm = terrain.gameObject.GetComponent<MicroSplatPolarisMesh>();
                    if (pm == null)
                    {
                        return;
                    }

                    string materialPath = AssetDatabase.GetAssetPath(pm.templateMaterial);
                    string directory = Path.GetDirectoryName(materialPath);
                    string configPath = string.Format("{0}/MicroSplatConfig.asset", directory);
                    TextureArrayConfig config = AssetDatabase.LoadAssetAtPath<TextureArrayConfig>(configPath);
                    if (config != null)
                    {
                        terrain.TerrainData.Shading.MicroSplatTextureArrayConfig = config;

                        GSplatPrototypeGroup splats = terrain.TerrainData.Shading.Splats;
                        List<GSplatPrototype> prototypes = splats.Prototypes;
                        for (int i = 0; i < prototypes.Count; ++i)
                        {
                            TextureArrayConfig.TextureEntry entry = new TextureArrayConfig.TextureEntry();
                            entry.diffuse = prototypes[i].Texture;
                            entry.normal = prototypes[i].NormalMap;
                            entry.aoChannel = TextureArrayConfig.TextureChannel.G;
                            entry.heightChannel = TextureArrayConfig.TextureChannel.G;
                            entry.smoothnessChannel = TextureArrayConfig.TextureChannel.G;
                            config.sourceTextures.Add(entry);
                        }

                        TextureArrayConfigEditor.CompileConfig(config);
                        terrain.PullMaterialAndSplatMapsFromMicroSplat();
                    }
                }
            });
        }
#endif
    }
}
#endif
