using UnityEngine;

namespace Pinwheel.Griffin
{
    [GDisplayName("Midpoint Split")]
    public class GMidpointSplitPolygonProcessor : IGPolygonProcessor
    {
        public void Process(GTerrainChunk chunk, ref GPolygon p)
        {
            Vector2 uv0 = p.Uvs[0];
            Vector2 uv1 = p.Uvs[1];
            Vector2 uv2 = p.Uvs[2];
            Vector2 uv01 = (uv0 + uv1) * 0.5f;
            Vector2 uv12 = (uv1 + uv2) * 0.5f;
            Vector2 uv20 = (uv2 + uv0) * 0.5f;

            Vector3 v0 = p.Vertices[0];
            Vector3 v1 = p.Vertices[1];
            Vector3 v2 = p.Vertices[2];
            Vector3 v01 = (v0 + v1) * 0.5f;
            Vector3 v12 = (v1 + v2) * 0.5f;
            Vector3 v20 = (v2 + v0) * 0.5f;

            v01.y = chunk.Terrain.GetInterpolatedHeightMapSample(uv01).x * chunk.Terrain.TerrainData.Geometry.Height;
            v12.y = chunk.Terrain.GetInterpolatedHeightMapSample(uv12).x * chunk.Terrain.TerrainData.Geometry.Height;
            v20.y = chunk.Terrain.GetInterpolatedHeightMapSample(uv20).x * chunk.Terrain.TerrainData.Geometry.Height;

            p.Vertices.Clear();
            p.Vertices.Add(v0); p.Vertices.Add(v01); p.Vertices.Add(v20);
            p.Vertices.Add(v1); p.Vertices.Add(v12); p.Vertices.Add(v01);
            p.Vertices.Add(v2); p.Vertices.Add(v20); p.Vertices.Add(v12);
            p.Vertices.Add(v01); p.Vertices.Add(v12); p.Vertices.Add(v20);

            p.Uvs.Clear();
            p.Uvs.Add(uv0); p.Uvs.Add(uv01); p.Uvs.Add(uv20);
            p.Uvs.Add(uv1); p.Uvs.Add(uv12); p.Uvs.Add(uv01);
            p.Uvs.Add(uv2); p.Uvs.Add(uv20); p.Uvs.Add(uv12);
            p.Uvs.Add(uv01); p.Uvs.Add(uv12); p.Uvs.Add(uv20);

            p.Triangles.Clear();
            p.Triangles.Add(0); p.Triangles.Add(1); p.Triangles.Add(2);
            p.Triangles.Add(3); p.Triangles.Add(4); p.Triangles.Add(5);
            p.Triangles.Add(6); p.Triangles.Add(7); p.Triangles.Add(8);
            p.Triangles.Add(9); p.Triangles.Add(10); p.Triangles.Add(11);
        }
    }
}
