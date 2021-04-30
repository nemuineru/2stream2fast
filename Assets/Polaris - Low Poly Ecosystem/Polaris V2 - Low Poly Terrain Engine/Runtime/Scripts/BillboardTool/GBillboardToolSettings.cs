using UnityEngine;

namespace Pinwheel.Griffin.BillboardTool
{
    [System.Serializable]
    public struct GBillboardToolSettings
    {
        [SerializeField]
        private Material atlasMaterial;
        public Material AtlasMaterial
        {
            get
            {
                return atlasMaterial;
            }
            set
            {
                atlasMaterial = value;
            }
        }

        [SerializeField]
        private Material normalMaterial;
        public Material NormalMaterial
        {
            get
            {
                return normalMaterial;
            }
            set
            {
                normalMaterial = value;
            }
        }
    }
}
