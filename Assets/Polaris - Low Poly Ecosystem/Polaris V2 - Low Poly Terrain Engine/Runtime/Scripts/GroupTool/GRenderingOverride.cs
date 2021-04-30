using UnityEngine;

namespace Pinwheel.Griffin.GroupTool
{
    [System.Serializable]
    public struct GRenderingOverride
    {
        [SerializeField]
        private bool overrideCastShadow;
        public bool OverrideCastShadow
        {
            get
            {
                return overrideCastShadow;
            }
            set
            {
                overrideCastShadow = value;
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
        private bool overrideReceiveShadow;
        public bool OverrideReceiveShadow
        {
            get
            {
                return overrideReceiveShadow;
            }
            set
            {
                overrideReceiveShadow = value;
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
        private bool overrideDrawFoliage;
        public bool OverrideDrawFoliage
        {
            get
            {
                return overrideDrawFoliage;
            }
            set
            {
                overrideDrawFoliage = value;
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
        private bool overrideEnableInstancing;
        public bool OverrideEnableInstancing
        {
            get
            {
                return overrideEnableInstancing;
            }
            set
            {
                overrideEnableInstancing = value;
            }
        }

        [SerializeField]
        private bool enableInstancing;
        public bool EnableInstancing
        {
            get
            {
                return enableInstancing;
            }
            set
            {
                enableInstancing = value;
            }
        }

        [SerializeField]
        private bool overrideBillboardStart;
        public bool OverrideBillboardStart
        {
            get
            {
                return overrideBillboardStart;
            }
            set
            {
                overrideBillboardStart = value;
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
        private bool overrideTreeDistance;
        public bool OverrideTreeDistance
        {
            get
            {
                return overrideTreeDistance;
            }
            set
            {
                overrideTreeDistance = value;
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
        private bool overrideGrassDistance;
        public bool OverrideGrassDistance
        {
            get
            {
                return overrideGrassDistance;
            }
            set
            {
                overrideGrassDistance = value;
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

        public void Reset()
        {
            OverrideCastShadow = false;
            OverrideReceiveShadow = false;
            OverrideDrawFoliage = false;
            OverrideEnableInstancing = false;
            OverrideBillboardStart = false;
            OverrideTreeDistance = false;
            OverrideGrassDistance = false;

            CastShadow = GGriffinSettings.Instance.TerrainDataDefault.Rendering.CastShadow;
            ReceiveShadow = GGriffinSettings.Instance.TerrainDataDefault.Rendering.ReceiveShadow;
            DrawFoliage = GGriffinSettings.Instance.TerrainDataDefault.Rendering.DrawFoliage;
            EnableInstancing = GGriffinSettings.Instance.TerrainDataDefault.Rendering.EnableInstancing;
            BillboardStart = GGriffinSettings.Instance.TerrainDataDefault.Rendering.BillboardStart;
            TreeDistance = GGriffinSettings.Instance.TerrainDataDefault.Rendering.TreeDistance;
            GrassDistance = GGriffinSettings.Instance.TerrainDataDefault.Rendering.GrassDistance;
        }

        public void Override(GRendering r)
        {
            if (OverrideCastShadow)
                r.CastShadow = CastShadow;
            if (OverrideReceiveShadow)
                r.ReceiveShadow = ReceiveShadow;
            if (OverrideDrawFoliage)
                r.DrawFoliage = DrawFoliage;
            if (OverrideEnableInstancing)
                r.EnableInstancing = EnableInstancing;
            if (OverrideBillboardStart)
                r.BillboardStart = BillboardStart;
            if (OverrideTreeDistance)
                r.TreeDistance = TreeDistance;
            if (OverrideGrassDistance)
                r.GrassDistance = GrassDistance;
        }
    }
}
