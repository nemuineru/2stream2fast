using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Griffin
{
    [GDisplayName("Albedo To Vertex Color (Sharp)")]
    public class GAlbedoToSharpVertexColorPolygonProcessor : IGPolygonProcessor
    {
        public void Process(GTerrainChunk chunk, ref GPolygon p)
        {
            Texture2D albedo = chunk.Terrain.TerrainData.Shading.AlbedoMap;
            Vector2 uv = (p.Uvs[0] + p.Uvs[1] + p.Uvs[2]) / 3;

            Color c = albedo.GetPixelBilinear(uv.x, uv.y);

            p.VertexColors = new List<Color32>(3);
            p.VertexColors.Add(c);
            p.VertexColors.Add(c);
            p.VertexColors.Add(c);
        }
    }
}
