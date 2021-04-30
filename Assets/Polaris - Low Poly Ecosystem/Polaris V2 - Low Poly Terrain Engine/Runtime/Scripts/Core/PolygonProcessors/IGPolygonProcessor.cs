namespace Pinwheel.Griffin
{
    public interface IGPolygonProcessor
    {
        void Process(GTerrainChunk chunk, ref GPolygon p);
    }
}
