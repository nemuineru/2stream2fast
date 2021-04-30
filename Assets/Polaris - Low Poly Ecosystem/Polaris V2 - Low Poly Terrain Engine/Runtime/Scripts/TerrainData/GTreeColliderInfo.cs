using UnityEngine;

namespace Pinwheel.Griffin
{
    [System.Serializable]
    public struct GTreeColliderInfo
    {
        [SerializeField]
        private Vector3 center;
        public Vector3 Center
        {
            get
            {
                return center;
            }
            set
            {
                center = value;
            }
        }

        [SerializeField]
        private float radius;
        public float Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
            }
        }

        [SerializeField]
        private float height;
        public float Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        [SerializeField]
        private int direction;
        public int Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }

        public GTreeColliderInfo(CapsuleCollider col)
        {
            center = col.center;
            radius = col.radius;
            height = col.height;
            direction = col.direction;
        }
    }
}
