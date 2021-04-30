using UnityEditor;
using UnityEngine;

namespace Pinwheel.Griffin
{
    [CustomEditor(typeof(GTreePrototypeGroup))]
    public class GTreePrototypeGroupInspector : Editor
    {
        private GTreePrototypeGroup instance;

        private void OnEnable()
        {
            instance = target as GTreePrototypeGroup;
        }

        public override void OnInspectorGUI()
        {
            GTreePrototypeGroupInspectorDrawer.Create(instance).DrawGUI();
        }

        public override bool RequiresConstantRepaint()
        {
            return false;
        }

        [MenuItem("CONTEXT/GTreePrototypeGroup/Refresh Prototypes")]
        public static void RefreshPrototypes()
        {
            Object o = Selection.activeObject;
            if (o is GTreePrototypeGroup)
            {
                GTreePrototypeGroup group = o as GTreePrototypeGroup;
                for (int i = 0; i < group.Prototypes.Count; ++i)
                {
                    group.Prototypes[i].Refresh();
                }
                EditorUtility.SetDirty(group);
            }
        }

        [MenuItem("CONTEXT/GTreePrototypeGroup/Fix Missing Trees")]
        public static void FixMissingPrefab()
        {
            Object o = Selection.activeObject;
            if (o is GTreePrototypeGroup)
            {
                string[] prefabAssetPaths = new string[]
                {
                    "Assets/Griffin - PolarisV2/_Demo/Prefabs/AutumnTree1.prefab",
                    "Assets/Griffin - PolarisV2/_Demo/Prefabs/SpringTree1.prefab",
                    "Assets/Griffin - PolarisV2/_Demo/Prefabs/Pine_00.prefab",
                    "Assets/Griffin - PolarisV2/_Demo/Prefabs/Pine_01.prefab",
                    "Assets/Griffin - PolarisV2/_Demo/Prefabs/Dead.prefab",
                    "Assets/Griffin - PolarisV2/_Demo/Prefabs/Dead_Break.prefab"
                };

                GTreePrototypeGroup group = o as GTreePrototypeGroup;
                group.Prototypes.Clear();
                for (int i = 0; i < prefabAssetPaths.Length; ++i)
                {
                    GameObject p = AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPaths[i]);
                    GTreePrototype proto = GTreePrototype.Create(p);
                    proto.Refresh();
                    group.Prototypes.Add(proto);
                }

                EditorUtility.SetDirty(group);
            }
        }
    }
}
