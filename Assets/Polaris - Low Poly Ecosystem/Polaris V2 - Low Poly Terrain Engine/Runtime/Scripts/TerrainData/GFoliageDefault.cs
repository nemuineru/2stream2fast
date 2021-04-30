using UnityEngine;

namespace Pinwheel.Griffin
{
    [System.Serializable]
    public struct GFoliageDefault
    {
        [SerializeField]
        private Material treeBillboardMaterial;
        public Material TreeBillboardMaterial
        {
            get
            {
                return treeBillboardMaterial;
            }
            set
            {
                treeBillboardMaterial = value;
            }
        }

        [SerializeField]
        private Material treeBillboardMaterialLWRP;
        public Material TreeBillboardMaterialLWRP
        {
            get
            {
                return treeBillboardMaterialLWRP;
            }
            set
            {
                treeBillboardMaterialLWRP = value;
            }
        }

        [SerializeField]
        private Material treeBillboardMaterialURP;
        public Material TreeBillboardMaterialURP
        {
            get
            {
                return treeBillboardMaterialURP;
            }
            set
            {
                treeBillboardMaterialURP = value;
            }
        }

        [SerializeField]
        private Material grassMaterial;
        public Material GrassMaterial
        {
            get
            {
                return grassMaterial;
            }
            set
            {
                grassMaterial = value;
            }
        }

        [SerializeField]
        private Material grassInteractiveMaterial;
        public Material GrassInteractiveMaterial
        {
            get
            {
                return grassInteractiveMaterial;
            }
            set
            {
                grassInteractiveMaterial = value;
            }
        }

        [SerializeField]
        private Material grassMaterialLWRP;
        public Material GrassMaterialLWRP
        {
            get
            {
                return grassMaterialLWRP;
            }
            set
            {
                grassMaterialLWRP = value;
            }
        }

        [SerializeField]
        private Material grassInteractiveMaterialLWRP;
        public Material GrassInteractiveMaterialLWRP
        {
            get
            {
                return grassInteractiveMaterialLWRP;
            }
            set
            {
                grassInteractiveMaterialLWRP = value;
            }
        }

        [SerializeField]
        private Material grassMaterialURP;
        public Material GrassMaterialURP
        {
            get
            {
                return grassMaterialURP;
            }
            set
            {
                grassMaterialURP = value;
            }
        }

        [SerializeField]
        private Material grassInteractiveMaterialURP;
        public Material GrassInteractiveMaterialURP
        {
            get
            {
                return grassInteractiveMaterialURP;
            }
            set
            {
                grassInteractiveMaterialURP = value;
            }
        }

        [SerializeField]
        private Material grassPreviewMaterial;
        public Material GrassPreviewMaterial
        {
            get
            {
                return grassPreviewMaterial;
            }
            set
            {
                grassPreviewMaterial = value;
            }
        }

        [SerializeField]
        private GTreePrototypeGroup trees;
        public GTreePrototypeGroup Trees
        {
            get
            {
                return trees;
            }
            set
            {
                trees = value;
            }
        }

        [SerializeField]
        private GSnapMode treeSnapMode;
        public GSnapMode TreeSnapMode
        {
            get
            {
                return treeSnapMode;
            }
            set
            {
                treeSnapMode = value;
            }
        }

        [SerializeField]
        private LayerMask treeSnapLayerMask;
        public LayerMask TreeSnapLayerMask
        {
            get
            {
                return treeSnapLayerMask;
            }
            set
            {
                treeSnapLayerMask = value;
            }
        }

        [SerializeField]
        private GGrassPrototypeGroup grasses;
        public GGrassPrototypeGroup Grasses
        {
            get
            {
                return grasses;
            }
            set
            {
                grasses = value;
            }
        }

        [SerializeField]
        private GSnapMode grassSnapMode;
        public GSnapMode GrassSnapMode
        {
            get
            {
                return grassSnapMode;
            }
            set
            {
                grassSnapMode = value;
            }
        }

        [SerializeField]
        private LayerMask grassSnapLayerMask;
        public LayerMask GrassSnapLayerMask
        {
            get
            {
                return grassSnapLayerMask;
            }
            set
            {
                grassSnapLayerMask = value;
            }
        }

        [SerializeField]
        private int patchGridSize;
        public int PatchGridSize
        {
            get
            {
                return patchGridSize;
            }
            set
            {
                patchGridSize = Mathf.Max(1, value);
            }
        }

        [SerializeField]
        private bool enableInteractiveGrass;
        public bool EnableInteractiveGrass
        {
            get
            {
                return enableInteractiveGrass;
            }
            set
            {
                enableInteractiveGrass = value;
            }
        }

        [SerializeField]
        private int vectorFieldMapResolution;
        public int VectorFieldMapResolution
        {
            get
            {
                return vectorFieldMapResolution;
            }
            set
            {
                vectorFieldMapResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(value), GCommon.TEXTURE_SIZE_MIN, GCommon.TEXTURE_SIZE_MAX);
            }
        }

        [SerializeField]
        private float bendSensitive;
        public float BendSensitive
        {
            get
            {
                return bendSensitive;
            }
            set
            {
                bendSensitive = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float restoreSensitive;
        public float RestoreSensitive
        {
            get
            {
                return restoreSensitive;
            }
            set
            {
                restoreSensitive = Mathf.Clamp01(value);
            }
        }
    }
}
