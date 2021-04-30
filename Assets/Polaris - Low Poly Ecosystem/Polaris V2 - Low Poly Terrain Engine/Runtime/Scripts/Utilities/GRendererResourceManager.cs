using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Griffin
{
    public class GRendererResourceManager
    {
        private static List<GRendererResourceManager> activeInstances;
        public static List<GRendererResourceManager> ActiveInstances
        {
            get
            {
                if (activeInstances == null)
                {
                    activeInstances = new List<GRendererResourceManager>();
                }
                return activeInstances;
            }
        }

        private static Dictionary<BillboardAsset, Mesh> billboardMeshes;
        private static Dictionary<BillboardAsset, Mesh> BillboardMeshes
        {
            get
            {
                if (billboardMeshes == null)
                {
                    billboardMeshes = new Dictionary<BillboardAsset, Mesh>();
                }
                return billboardMeshes;
            }
        }

        public GRendererResourceManager()
        {
            billboardMeshes = new Dictionary<BillboardAsset, Mesh>();
            ActiveInstances.Add(this);
        }

        ~GRendererResourceManager()
        {
            ActiveInstances.Remove(this);
            Release();
        }

        private void Release()
        {
            //foreach (Mesh m in BillboardMeshes.Values)
            //{
            //    Object.Destroy(m);
            //}
        }

        public Mesh GetBillboardMesh(BillboardAsset billboard)
        {
            if (BillboardMeshes.ContainsKey(billboard) && BillboardMeshes[billboard] != null)
            {
                return BillboardMeshes[billboard];
            }
            else if (BillboardMeshes.ContainsKey(billboard) && BillboardMeshes[billboard] == null)
            {
                BillboardMeshes.Remove(billboard);
            }

            Mesh m = CreateBillboardMesh(billboard);
            BillboardMeshes.Add(billboard, m);
            return m;
        }

        private Mesh CreateBillboardMesh(BillboardAsset billboard)
        {
            Mesh m = new Mesh();
            Vector2[] uvs = billboard.GetVertices();
            Vector3[] vertices = new Vector3[billboard.vertexCount];
            for (int i = 0; i < vertices.Length; ++i)
            {
                vertices[i] = new Vector3(
                    (uvs[i].x - 0.5f) * billboard.width,
                    uvs[i].y * billboard.height + billboard.bottom,
                    0);
            }
            ushort[] tris = billboard.GetIndices();
            int[] trisInt = new int[tris.Length];
            for (int i = 0; i < trisInt.Length; ++i)
            {
                trisInt[i] = tris[i];
            }

            m.vertices = vertices;
            m.uv = uvs;
            m.triangles = trisInt;
            m.name = billboard.name;
            return m;
        }

        public void UpdateBillboardMesh(BillboardAsset billboard)
        {
            if (BillboardMeshes.ContainsKey(billboard) && BillboardMeshes[billboard] != null)
            {
                GUtilities.DestroyObject(BillboardMeshes[billboard]);
                BillboardMeshes.Remove(billboard);
            }

            Mesh m = CreateBillboardMesh(billboard);
            BillboardMeshes.Add(billboard, m);
        }
    }
}
