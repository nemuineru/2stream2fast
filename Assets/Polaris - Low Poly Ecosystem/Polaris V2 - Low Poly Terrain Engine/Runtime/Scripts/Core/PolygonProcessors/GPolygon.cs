using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Griffin
{
    public class GPolygon
    {
        private List<Vector3> vertices;
        public List<Vector3> Vertices
        {
            get
            {
                if (vertices == null)
                {
                    vertices = new List<Vector3>(3);
                }
                return vertices;
            }
            set
            {
                vertices = value;
            }
        }

        private List<Vector2> uvs;
        public List<Vector2> Uvs
        {
            get
            {
                if (uvs == null)
                {
                    uvs = new List<Vector2>(3);
                }
                return uvs;
            }
            set
            {
                uvs = value;
            }
        }

        private List<Color32> vertexColors;
        public List<Color32> VertexColors
        {
            get
            {
                return vertexColors;
            }
            set
            {
                vertexColors = value;
            }
        }

        private List<int> triangles;
        public List<int> Triangles
        {
            get
            {
                if (triangles == null)
                {
                    triangles = new List<int>(3);
                }
                return triangles;
            }
            set
            {
                triangles = value;
            }
        }

        public void Clear()
        {
            if (Vertices != null)
                Vertices.Clear();
            if (Uvs != null)
                Uvs.Clear();
            if (VertexColors != null)
                VertexColors.Clear();
            if (Triangles != null)
                Triangles.Clear();
        }
    }
}
