//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace Pinwheel.Griffin
//{
//    public class GGridlineMeshCreatorWindow : EditorWindow
//    {
//        public GTerrainGeneratedData Container { get; set; }

//        //[MenuItem("Window/Griffin/Tools/Gridline Mesh Creator")]
//        public static void ShowWindow()
//        {
//            GGridlineMeshCreatorWindow window = GetWindow<GGridlineMeshCreatorWindow>();
//            window.titleContent = new GUIContent("GGridlineMeshCreator");
//            window.Show();
//        }

//        public void OnGUI()
//        {
//            Container = EditorGUILayout.ObjectField("Container", Container, typeof(GTerrainGeneratedData), false) as GTerrainGeneratedData;

//            if (GUILayout.Button("Generate"))
//            {
//                Generate();
//            }
//        }

//        private void Generate()
//        {
//            List<string> keys = Container.GetKeys();
//            for (int i = 0; i < keys.Count; ++i)
//            {
//                Container.DeleteMesh(keys[i]);
//            }

//            Mesh[] wireframeMeshes = new Mesh[GGriffinSettings.Instance.livePreviewMeshes.Length];
//            for (int i = 0; i < wireframeMeshes.Length; ++i)
//            {
//                Mesh wm = ConvertToLineMesh(GGriffinSettings.Instance.livePreviewMeshes[i]);
//                wm.name = "Wireframe Grid " + i.ToString();
//                wireframeMeshes[i] = wm;
//                Container.SetMesh(wm.name, wm);
//            }

//            GGriffinSettings.Instance.livePreviewWireframeMeshes = wireframeMeshes;
//            EditorUtility.SetDirty(Container);
//            EditorUtility.SetDirty(GGriffinSettings.Instance);
//        }

//        private Mesh ConvertToLineMesh(Mesh m)
//        {
//            Mesh wm = new Mesh();
//            wm.vertices = m.vertices;
//            wm.uv = m.uv;
//            wm.colors = m.colors;

//            int[] tris = m.triangles;
//            int trisCount = tris.Length / 3;

//            List<int> indices = new List<int>();

//            for (int i = 0; i < trisCount; ++i)
//            {
//                indices.Add(tris[i * 3 + 0]);
//                indices.Add(tris[i * 3 + 1]);

//                indices.Add(tris[i * 3 + 1]);
//                indices.Add(tris[i * 3 + 2]);

//                indices.Add(tris[i * 3 + 2]);
//                indices.Add(tris[i * 3 + 0]);
//            }

//            wm.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

//            return wm;
//        }
//    }
//}
