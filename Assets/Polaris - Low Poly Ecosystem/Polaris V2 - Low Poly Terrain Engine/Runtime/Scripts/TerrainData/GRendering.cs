using UnityEngine;

namespace Pinwheel.Griffin
{
    public class GRendering : ScriptableObject
    {
        [SerializeField]
        private GTerrainData terrainData;
        public GTerrainData TerrainData
        {
            get
            {
                return terrainData;
            }
            internal set
            {
                terrainData = value;
            }
        }

        [SerializeField]
        private bool castShadow;
        public bool CastShadow
        {
            get
            {
                return castShadow;
            }
            set
            {
                castShadow = value;
            }
        }

        [SerializeField]
        private bool receiveShadow;
        public bool ReceiveShadow
        {
            get
            {
                return receiveShadow;
            }
            set
            {
                receiveShadow = value;
            }
        }

        [SerializeField]
        private bool drawFoliage;
        public bool DrawFoliage
        {
            get
            {
                return drawFoliage;
            }
            set
            {
                drawFoliage = value;
            }
        }

        [SerializeField]
        private bool enableInstancing;
        public bool EnableInstancing
        {
            get
            {
                if (!SystemInfo.supportsInstancing)
                    enableInstancing = false;
                return enableInstancing;
            }
            set
            {
                if (SystemInfo.supportsInstancing)
                {
                    enableInstancing = value;
                }
                else
                {
                    enableInstancing = false;
                }
            }
        }

        [SerializeField]
        private float billboardStart;
        public float BillboardStart
        {
            get
            {
                return billboardStart;
            }
            set
            {
                billboardStart = Mathf.Clamp(value, 0, GCommon.MAX_TREE_DISTANCE);
            }
        }

        [SerializeField]
        private float treeDistance;
        public float TreeDistance
        {
            get
            {
                return treeDistance;
            }
            set
            {
                treeDistance = Mathf.Clamp(value, 0, GCommon.MAX_TREE_DISTANCE);
            }
        }

        [SerializeField]
        private float grassDistance;
        public float GrassDistance
        {
            get
            {
                return grassDistance;
            }
            set
            {
                grassDistance = Mathf.Clamp(value, 0, GCommon.MAX_GRASS_DISTANCE);
            }
        }

        private void Reset()
        {
            name = "Rendering";
            CastShadow = GGriffinSettings.Instance.TerrainDataDefault.Rendering.CastShadow;
            ReceiveShadow = GGriffinSettings.Instance.TerrainDataDefault.Rendering.ReceiveShadow;
            DrawFoliage = GGriffinSettings.Instance.TerrainDataDefault.Rendering.DrawFoliage;
            EnableInstancing = GGriffinSettings.Instance.TerrainDataDefault.Rendering.EnableInstancing;
            BillboardStart = GGriffinSettings.Instance.TerrainDataDefault.Rendering.BillboardStart;
            TreeDistance = GGriffinSettings.Instance.TerrainDataDefault.Rendering.TreeDistance;
            GrassDistance = GGriffinSettings.Instance.TerrainDataDefault.Rendering.GrassDistance;
        }

        public void ResetFull()
        {
            Reset();
        }

        public void CopyTo(GRendering des)
        {
            des.CastShadow = CastShadow;
            des.ReceiveShadow = ReceiveShadow;
            des.DrawFoliage = DrawFoliage;
            des.EnableInstancing = EnableInstancing;
            des.BillboardStart = BillboardStart;
            des.TreeDistance = TreeDistance;
            des.GrassDistance = GrassDistance;
        }
    }
}
