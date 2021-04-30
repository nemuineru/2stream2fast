//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using Pinwheel.MeshToFile;

//namespace Pinwheel.Griffin
//{
//    public class GGrassMeshGenerator
//    {
//        [MenuItem("Window/Griffin/Internal/GenerateGrassMeshes")]
//        public static void Generate()
//        {
//            Mesh quad = GenerateQuad();
//            Mesh cross = GenerateCross();
//            Mesh triCross = GenerateTriCross();
//            Mesh singleBlade = GenerateSingleBlade();
//            SaveToAsset(quad, cross, triCross, singleBlade);
//        }

//        private static Mesh GenerateQuad()
//        {
//            Vector3[] vertices = new Vector3[]
//            {
//                new Vector3(-0.5f, 0, 0), new Vector3(-0.5f, 1, 0), new Vector3(0.5f, 1, 0), new Vector3(0.5f, 0, 0)
//            };
//            Vector2[] uvs = new Vector2[]
//            {
//                Vector2.zero, Vector2.up, Vector2.one, Vector2.right
//            };
//            int[] indices = new int[] { 0, 1, 2, 0, 2, 3 };
//            Mesh quad = new Mesh();
//            quad.vertices = vertices;
//            quad.uv = uvs;
//            quad.triangles = indices;
//            quad.name = "Quad";
//            quad.RecalculateBounds();
//            quad.RecalculateNormals();
//            quad.RecalculateTangents();
//            return quad;
//        }

//        private static Mesh GenerateCross()
//        {
//            Vector3[] vertices = new Vector3[]
//            {
//                new Vector3(-0.5f, 0, 0), new Vector3(-0.5f, 1, 0), new Vector3(0.5f, 1, 0), new Vector3(0.5f, 0, 0),
//                new Vector3(0, 0, -0.5f), new Vector3(0, 1, -0.5f), new Vector3(0, 1, 0.5f), new Vector3(0, 0, 0.5f)
//            };
//            Vector2[] uvs = new Vector2[]
//            {
//                Vector2.zero, Vector2.up, Vector2.one, Vector2.right,
//                Vector2.zero, Vector2.up, Vector2.one,Vector2.right
//            };

//            int[] indices = new int[]
//            {
//                0, 1, 2, 0, 2, 3,
//                4, 5, 6, 4, 6, 7
//            };
//            Mesh cross = new Mesh();
//            cross.vertices = vertices;
//            cross.uv = uvs;
//            cross.triangles = indices;
//            cross.name = "Cross";
//            cross.RecalculateBounds();
//            cross.RecalculateNormals();
//            cross.RecalculateTangents();
//            return cross;
//        }

//        private static Mesh GenerateTriCross()
//        {
//            Vector3[] quads = new Vector3[]
//            {
//                new Vector3(-0.5f, 0, 0), new Vector3(-0.5f, 1, 0), new Vector3(0.5f, 1, 0),new Vector3(0.5f, 0, 0)
//            };

//            List<Vector3> vertices = new List<Vector3>();
//            vertices.AddRange(quads);

//            Matrix4x4 rotate60 = Matrix4x4.Rotate(Quaternion.Euler(0, -60, 0));
//            for (int i = 0; i < quads.Length; ++i)
//            {
//                vertices.Add(rotate60.MultiplyPoint(quads[i]));
//            }

//            Matrix4x4 rotate120 = Matrix4x4.Rotate(Quaternion.Euler(0, -120, 0));
//            for (int i = 0; i < quads.Length; ++i)
//            {
//                vertices.Add(rotate120.MultiplyPoint(quads[i]));
//            }

//            Vector2[] uvs = new Vector2[]
//            {
//                Vector2.zero, Vector2.up, Vector2.one, Vector2.right,
//                Vector2.zero, Vector2.up, Vector2.one, Vector2.right,
//                Vector2.zero, Vector2.up, Vector2.one, Vector2.right
//            };

//            int[] indices = new int[]
//            {
//                0, 1, 2, 0, 2, 3,
//                4, 5, 6, 4, 6, 7,
//                8, 9,10, 8,10,11
//            };
//            Mesh triCross = new Mesh();
//            triCross.vertices = vertices.ToArray();
//            triCross.uv = uvs;
//            triCross.triangles = indices;
//            triCross.name = "TriCross";
//            triCross.RecalculateBounds();
//            triCross.RecalculateNormals();
//            triCross.RecalculateTangents();
//            return triCross;
//        }

//        private static Mesh GenerateSingleBlade()
//        {
//            Vector3[] vertices = new Vector3[]
//            {
//                new Vector3(-0.05f, 0, 0), new Vector3(-0.1f, 0.3f, 0), new Vector3(0.1f, 0.3f, 0),
//                new Vector3(-0.05f, 0, 0), new Vector3(0.1f, 0.3f, 0), new Vector3(0.05f, 0, 0),

//                new Vector3(-0.1f, 0.3f, 0), new Vector3(-0.15f, 0.6f, 0), new Vector3(0.15f, 0.6f, 0),
//                new Vector3(-0.1f, 0.3f, 0), new Vector3(0.15f, 0.6f, 0), new Vector3(0.1f, 0.3f, 0),

//                new Vector3(-0.15f, 0.6f, 0), new Vector3(-0.12f, 1f, 0), new Vector3(0.12f, 1f, 0),
//                new Vector3(-0.15f, 0.6f, 0), new Vector3(0.12f, 1f, 0), new Vector3(0.15f, 0.6f, 0),
//            };
//            Vector2[] uvs = new Vector2[]
//            {
//                new Vector2(0, 0), new Vector2(0, 0.3f), new Vector2(1, 0.3f),
//                new Vector2(0, 0), new Vector2(1, 0.3f), new Vector2(1, 0),

//                new Vector2(0, 0.3f), new Vector2(0, 0.6f), new Vector2(1, 0.6f),
//                new Vector2(0, 0.3f), new Vector2(1, 0.6f), new Vector2(1, 0.3f),

//                new Vector2(0, 0.6f), new Vector2(0, 1f), new Vector2(1, 1f),
//                new Vector2(0, 0.6f), new Vector2(1, 1f), new Vector2(1, 0.6f)
//            };
//            Color[] colors = new Color[]
//            {
//                new Color(0, 0, 0, 0), new Color(0, 0, 0, 0.15f), new Color(0, 0, 0, 0.15f),
//                new Color(0, 0, 0, 0), new Color(0, 0, 0, 0.15f), new Color(0, 0, 0, 0f),

//                new Color(0, 0, 0, 0.15f), new Color(0, 0, 0, 0.45f), new Color(0, 0, 0, 0.45f),
//                new Color(0, 0, 0, 0.15f), new Color(0, 0, 0, 0.45f), new Color(0, 0, 0, 0.15f),

//                new Color(0, 0, 0, 0.45f), new Color(0, 0, 0, 1f), new Color(0, 0, 0, 1f),
//                new Color(0, 0, 0, 0.45f), new Color(0, 0, 0, 1f), new Color(0, 0, 0, 0.45f),
//            };
//            int[] indices = GUtilities.GetIndicesArray(18);
//            Mesh singleBlade = new Mesh();
//            singleBlade.vertices = vertices;
//            singleBlade.uv = uvs;
//            singleBlade.colors = colors;
//            singleBlade.triangles = indices;
//            singleBlade.name = "Single Blade";
//            singleBlade.RecalculateBounds();
//            singleBlade.RecalculateNormals();
//            singleBlade.RecalculateTangents();
//            return singleBlade;
//        }

//        private static void SaveToAsset(params Mesh[] meshes)
//        {
//            MeshSaver.SaveFbxMultipleMesh(meshes, "Assets/", "GrassMeshes");
//            AssetDatabase.Refresh();
//        }
//    }
//}
