using UnityEngine;

namespace Pinwheel.Griffin
{
    [System.Serializable]
    public struct GTerrainDataDefault
    {
        [SerializeField]
        private GGeometryDefault geometry;
        public GGeometryDefault Geometry
        {
            get
            {
                return geometry;
            }
            set
            {
                geometry = value;
            }
        }

        [SerializeField]
        private GShadingDefault shading;
        public GShadingDefault Shading
        {
            get
            {
                return shading;
            }
            set
            {
                shading = value;
            }
        }

        [SerializeField]
        private GRenderingDefault rendering;
        public GRenderingDefault Rendering
        {
            get
            {
                return rendering;
            }
            set
            {
                rendering = value;
            }
        }

        [SerializeField]
        private GFoliageDefault foliage;
        public GFoliageDefault Foliage
        {
            get
            {
                return foliage;
            }
            set
            {
                foliage = value;
            }
        }
    }
}
