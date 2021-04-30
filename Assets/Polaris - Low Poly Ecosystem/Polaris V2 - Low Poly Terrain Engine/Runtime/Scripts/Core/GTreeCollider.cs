using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Griffin
{
    [ExecuteInEditMode]
    public class GTreeCollider : MonoBehaviour
    {
        [SerializeField]
        private GStylizedTerrain terrain;
        public GStylizedTerrain Terrain
        {
            get
            {
                return terrain;
            }
            set
            {
                GStylizedTerrain oldValue = terrain;
                GStylizedTerrain newValue = value;
                terrain = newValue;
                if (oldValue != newValue)
                {
                    CopyTreeInstances();
                }
            }
        }

        [SerializeField]
        private GameObject target;
        public GameObject Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }

        [SerializeField]
        private int colliderBudget;
        public int ColliderBudget
        {
            get
            {
                return colliderBudget;
            }
            set
            {
                colliderBudget = (int)Mathf.Clamp(value, 1, GCommon.MAX_COLLIDER_BUDGET);
            }
        }

        [SerializeField]
        private float distance;
        public float Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = Mathf.Max(1, value);
            }
        }

        [SerializeField]
        private CapsuleCollider[] colliders;
        private CapsuleCollider[] Colliders
        {
            get
            {
                if (colliders == null || colliders.Length != ColliderBudget)
                {
                    GUtilities.ClearChildren(transform);
                    colliders = new CapsuleCollider[ColliderBudget];
                }
                return colliders;
            }
        }

        private List<GTreeInstance> treeInstances;
        private List<GTreeInstance> TreeInstances
        {
            get
            {
                if (treeInstances == null)
                    treeInstances = new List<GTreeInstance>();
                return treeInstances;
            }
            set
            {
                treeInstances = value;
            }
        }

        private void OnEnable()
        {
            GTerrainData.GlobalDirty += OnTerrainDataDirty;
            CopyTreeInstances();
        }

        private void OnDisable()
        {
            GTerrainData.GlobalDirty -= OnTerrainDataDirty;
        }

        private void OnTerrainDataDirty(GTerrainData data, GTerrainData.DirtyFlags flag)
        {
            if (Terrain != null &&
                Terrain.TerrainData == data)
            {
                if (flag == GTerrainData.DirtyFlags.All ||
                    flag == GTerrainData.DirtyFlags.Foliage)
                {
                    CopyTreeInstances();
                }
            }
        }

        private void CopyTreeInstances()
        {
            TreeInstances.Clear();
            if (Terrain != null &&
                Terrain.TerrainData != null &&
                Terrain.TerrainData.Foliage.Trees != null &&
                Terrain.TerrainData.Foliage.Trees.Prototypes.Count > 0)
            {
                TreeInstances = new List<GTreeInstance>(Terrain.TerrainData.Foliage.TreeInstances);
            }
        }

        public void Reset()
        {
            Terrain = GetComponentInParent<GStylizedTerrain>();
            if (Terrain == null)
                Terrain = GetComponent<GStylizedTerrain>();
            Target = null;
            ColliderBudget = 25;
            Distance = 50;
            CopyTreeInstances();
        }

        private void LateUpdate()
        {
            if (Terrain == null)
                return;
            if (Terrain.TerrainData == null)
                return;
            if (Terrain.TerrainData.Foliage.Trees == null)
                return;
            if (Terrain.TerrainData.Foliage.Trees.Prototypes.Count == 0)
                return;

            GameObject actualTarget = null;
            if (Target != null)
                actualTarget = Target;
            else if (Camera.main != null)
                actualTarget = Camera.main.gameObject;

            if (actualTarget == null)
                return;
            Vector3 terrainSize = new Vector3(
                Terrain.TerrainData.Geometry.Width,
                Terrain.TerrainData.Geometry.Height,
                Terrain.TerrainData.Geometry.Length);
            Vector3 targetLocalPos = Terrain.transform.InverseTransformPoint(actualTarget.transform.position);
            Vector3 treeLocalPos = Vector3.zero;
            float sqrDistance = distance * distance;

            TreeInstances.Clear();
            List<GTreeInstance> instances = Terrain.TerrainData.Foliage.TreeInstances;
            for (int i = 0; i < instances.Count; ++i)
            {
                GTreeInstance tree = instances[i];
                treeLocalPos.Set(
                    tree.Position.x * terrainSize.x,
                    tree.Position.y * terrainSize.y,
                    tree.Position.z * terrainSize.z);
                if (Vector3.SqrMagnitude(targetLocalPos - treeLocalPos) <= sqrDistance)
                {
                    TreeInstances.Add(tree);
                }
            }

            Vector3 targetNormalizePos = Terrain.WorldPointToNormalized(actualTarget.transform.position);
            TreeInstances.Sort((t0, t1) =>
            {
                float d0 = Vector3.SqrMagnitude(targetNormalizePos - t0.Position);
                float d1 = Vector3.SqrMagnitude(targetNormalizePos - t1.Position);
                return d0.CompareTo(d1);
            });

            List<GTreePrototype> prototypes = Terrain.TerrainData.Foliage.Trees.Prototypes;
            int colliderIndex = 0;
            for (int i = 0; i < TreeInstances.Count; ++i)
            {
                GTreeInstance tree = TreeInstances[i];
                GTreePrototype prototype = prototypes[tree.PrototypeIndex];
                if (!prototype.HasCollider)
                    continue;

                if (colliderIndex >= ColliderBudget)
                    break;
                CapsuleCollider col = GetCollider(colliderIndex);
                colliderIndex += 1;

                Vector3 localPos = new Vector3(
                    terrainSize.x * tree.Position.x,
                    terrainSize.y * tree.Position.y,
                    terrainSize.z * tree.Position.z);
                Vector3 worldPos = Terrain.transform.TransformPoint(localPos);
                col.transform.position = worldPos;
                col.transform.rotation = tree.Rotation;
                col.transform.localScale = tree.Scale;
                GTreeColliderInfo colliderInfo = prototype.ColliderInfo;
                col.center = colliderInfo.Center;
                col.radius = colliderInfo.Radius;
                col.height = colliderInfo.Height;
                col.direction = colliderInfo.Direction;
                col.gameObject.layer = prototype.Layer;
                col.gameObject.tag = prototype.Prefab.tag;
                col.gameObject.SetActive(true);
            }

            for (int i = colliderIndex; i < ColliderBudget; ++i)
            {
                CapsuleCollider col = GetCollider(i);
                col.gameObject.SetActive(false);
            }
        }

        private CapsuleCollider GetCollider(int index)
        {
            if (Colliders[index] == null)
            {
                GameObject g = new GameObject("Collider");
                GUtilities.ResetTransform(g.transform, transform);

                CapsuleCollider col = g.AddComponent<CapsuleCollider>();
                Colliders[index] = col;
            }
            return Colliders[index];
        }
    }
}
