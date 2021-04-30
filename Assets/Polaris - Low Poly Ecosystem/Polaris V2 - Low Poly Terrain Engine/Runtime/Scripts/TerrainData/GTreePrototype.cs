using UnityEngine;
using UnityEngine.Rendering;

namespace Pinwheel.Griffin
{
    [System.Serializable]
    public class GTreePrototype
    {
        [SerializeField]
        private GameObject prefab;
        public GameObject Prefab
        {
            get
            {
                return prefab;
            }
            set
            {
                prefab = value;
                Refresh();
            }
        }

        [HideInInspector]
        [SerializeField]
        private Mesh sharedMesh;
        public Mesh SharedMesh
        {
            get
            {
                return sharedMesh;
            }
            private set
            {
                sharedMesh = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        private Material[] sharedMaterials;
        public Material[] SharedMaterials
        {
            get
            {
                return sharedMaterials;
            }
            private set
            {
                sharedMaterials = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        private ShadowCastingMode shadowCastingMode;
        public ShadowCastingMode ShadowCastingMode
        {
            get
            {
                return shadowCastingMode;
            }
            private set
            {
                shadowCastingMode = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        private bool receiveShadow;
        public bool ReceiveShadow
        {
            get
            {
                return receiveShadow;
            }
            private set
            {
                receiveShadow = value;
            }
        }

        [SerializeField]
        private int layer;
        public int Layer
        {
            get
            {
                return layer;
            }
            set
            {
                layer = value;
            }
        }

        [SerializeField]
        private bool keepPrefabLayer;
        public bool KeepPrefabLayer
        {
            get
            {
                return keepPrefabLayer;
            }
            set
            {
                keepPrefabLayer = value;
            }
        }

        [SerializeField]
        private BillboardAsset billboard;
        public BillboardAsset Billboard
        {
            get
            {
                return billboard;
            }
            set
            {
                billboard = value;
            }
        }

        [SerializeField]
        private bool hasCollider;
        public bool HasCollider
        {
            get
            {
                return hasCollider;
            }
            set
            {
                hasCollider = value;
            }
        }

        [SerializeField]
        private GTreeColliderInfo colliderInfo;
        public GTreeColliderInfo ColliderInfo
        {
            get
            {
                return colliderInfo;
            }
            set
            {
                colliderInfo = value;
            }
        }

        [SerializeField]
        private float pivotOffset;
        public float PivotOffset
        {
            get
            {
                return pivotOffset;
            }
            set
            {
                pivotOffset = Mathf.Clamp(value, -1, 1);
            }
        }

        [SerializeField]
        private Quaternion baseRotation = Quaternion.identity;
        public Quaternion BaseRotation
        {
            get
            {
                if (baseRotation.x == 0 &&
                    baseRotation.y == 0 &&
                    baseRotation.z == 0 &&
                    baseRotation.w == 0)
                {
                    baseRotation = Quaternion.identity;
                }
                return baseRotation;
            }
            set
            {
                baseRotation = value;
                if (baseRotation.x == 0 &&
                     baseRotation.y == 0 &&
                     baseRotation.z == 0 &&
                     baseRotation.w == 0)
                {
                    baseRotation = Quaternion.identity;
                }
            }
        }

        [SerializeField]
        private Vector3 baseScale = Vector3.one;
        public Vector3 BaseScale
        {
            get
            {
                return baseScale;
            }
            set
            {
                baseScale = value;
            }
        }

#if UNITY_EDITOR
        [SerializeField]
        private string editor_PrefabAssetPath;
        public string Editor_PrefabAssetPath
        {
            get
            {
                return editor_PrefabAssetPath;
            }
            set
            {
                editor_PrefabAssetPath = value;
            }
        }
#endif

        public bool IsValid
        {
            get
            {
                return Prefab != null && SharedMesh != null && SharedMaterials != null;
            }
        }

        public static GTreePrototype Create(GameObject g)
        {
            GTreePrototype prototype = new GTreePrototype();
            prototype.Prefab = g;
            prototype.PivotOffset = 0;
            prototype.BaseRotation = Quaternion.identity;
            prototype.BaseScale = Vector3.one;
            return prototype;
        }

        public void Refresh()
        {
            if (Prefab == null)
            {
                SharedMesh = null;
                SharedMaterials = null;
                ShadowCastingMode = ShadowCastingMode.Off;
                ReceiveShadow = false;
                Layer = 0;
            }
            else
            {
                MeshFilter mf = Prefab.GetComponentInChildren<MeshFilter>();
                if (mf != null)
                {
                    SharedMesh = mf.sharedMesh;
                }

                MeshRenderer mr = Prefab.GetComponentInChildren<MeshRenderer>();
                if (mr != null)
                {
                    SharedMaterials = mr.sharedMaterials;
                    ShadowCastingMode = mr.shadowCastingMode;
                    ReceiveShadow = mr.receiveShadows;
                }

                CapsuleCollider col = Prefab.GetComponentInChildren<CapsuleCollider>();
                hasCollider = col != null;
                if (col != null)
                {
                    ColliderInfo = new GTreeColliderInfo(col);
                }

                if (KeepPrefabLayer)
                {
                    Layer = Prefab.layer;
                }
            }

            if (BaseScale == Vector3.zero)
                BaseScale = Vector3.one;
        }

        public static explicit operator GTreePrototype(TreePrototype p)
        {
            return Create(p.prefab);
        }

        public static explicit operator TreePrototype(GTreePrototype p)
        {
            TreePrototype prototype = new TreePrototype();
            prototype.prefab = p.Prefab;
            return prototype;
        }

        public bool Equals(TreePrototype treePrototype)
        {
            return Prefab == treePrototype.prefab;
        }
    }
}
