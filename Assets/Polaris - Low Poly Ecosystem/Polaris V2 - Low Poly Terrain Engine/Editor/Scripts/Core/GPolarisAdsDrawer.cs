using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEditor;

namespace Pinwheel.Griffin
{
    public class GPolarisAdsDrawer
    {
        [DidReloadScripts]
        private static void OnDidReloadScripts()
        {
            GStylizedTerrainInspector.GUIInject += GStylizedTerrainInspector_GUIInject;
        }

        private static void GStylizedTerrainInspector_GUIInject(GStylizedTerrain terrain, int order)
        {
            if (order != 7)
                return;
            string label = "Polaris 2020 is available!";
            string id = "polaris2020-ads";

            GEditorCommon.Foldout(label, true, id, () =>
            {
                GEditorCommon.Header("Polaris 2020");
                EditorGUILayout.LabelField("Harness the power of C# Job System and Burst Compiler, as well as instanced rendering, it brings you new features set and a massive performance boost!", GEditorCommon.WordWrapItalicLabel);
                if (GUILayout.Button("Learn more"))
                {
                    Application.OpenURL("https://assetstore.unity.com/packages/tools/terrain/low-poly-terrain-polaris-2020-170400?aid=1100l3QbW&pubref=p2-editor");
                }

                GEditorCommon.Header("Polaris LTS");
                EditorGUILayout.LabelField("The current version still receive full support, bugs fixing and improvements that brings you a stable terrain solution to achieve your vision.", GEditorCommon.WordWrapItalicLabel);
            },
            null,
            " ");
        }
    }
}
