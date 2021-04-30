using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.Griffin
{
    [CreateAssetMenu(fileName = "Terrain Data", menuName = "Polaris V2/Terrain Data")]
    public class GTerrainData : ScriptableObject
    {
        public delegate void GlobalDirtyHandler(GTerrainData data, DirtyFlags flag);
        public static event GlobalDirtyHandler GlobalDirty;

        public delegate void DirtyHandler(DirtyFlags flag);
        public event DirtyHandler Dirty;

        [System.Flags]
        public enum DirtyFlags : byte
        {
            None = 0,
            Geometry = 1,
            GeometryAsync = 2,
            Rendering = 4,
            Shading = 8,
            Foliage = 16,
            All = byte.MaxValue
        }

        [SerializeField]
        private string id;
        public string Id
        {
            get
            {
                return id;
            }
            private set
            {
                id = value;
            }
        }

        [SerializeField]
        private GGeometry geometry;
        public GGeometry Geometry
        {
            get
            {
                if (geometry == null)
                {
                    geometry = ScriptableObject.CreateInstance<GGeometry>();
                    geometry.TerrainData = this;
                    geometry.ResetFull();
                }
                GCommon.TryAddObjectToAsset(geometry, this);
                geometry.TerrainData = this;
                return geometry;
            }
        }

        [SerializeField]
        private GShading shading;
        public GShading Shading
        {
            get
            {
                if (shading == null)
                {
                    shading = ScriptableObject.CreateInstance<GShading>();
                    shading.TerrainData = this;
                    shading.ResetFull();
                }
                GCommon.TryAddObjectToAsset(shading, this);
                shading.TerrainData = this;
                return shading;
            }
        }

        [SerializeField]
        private GRendering rendering;
        public GRendering Rendering
        {
            get
            {
                if (rendering == null)
                {
                    rendering = ScriptableObject.CreateInstance<GRendering>();
                    rendering.TerrainData = this;
                    rendering.ResetFull();
                }
                GCommon.TryAddObjectToAsset(rendering, this);
                rendering.TerrainData = this;
                return rendering;
            }
        }

        [SerializeField]
        private GFoliage foliage;
        public GFoliage Foliage
        {
            get
            {
                if (foliage == null)
                {
                    foliage = ScriptableObject.CreateInstance<GFoliage>();
                    foliage.TerrainData = this;
                    foliage.ResetFull();
                }
                GCommon.TryAddObjectToAsset(foliage, this);
                foliage.TerrainData = this;
                return foliage;
            }
        }

        [SerializeField]
        [FormerlySerializedAs("generatedData")]
        private GTerrainGeneratedData geometryData;
        public GTerrainGeneratedData GeometryData
        {
            get
            {
                if (geometryData == null)
                {
                    geometryData = GCommon.GetTerrainGeneratedDataAsset(this, "GeneratedGeometry");
                }
                geometryData.TerrainData = this;
                return geometryData;
            }
        }

        [SerializeField]
        private GTerrainGeneratedData foliageData;
        public GTerrainGeneratedData FoliageData
        {
            get
            {
                if (foliageData == null)
                {
                    foliageData = GCommon.GetTerrainGeneratedDataAsset(this, "GeneratedFoliage");
                }
                foliageData.TerrainData = this;
                return foliageData;
            }
        }

        public void Reset()
        {
            id = GCommon.GetUniqueID();
        }

        public void SetDirty(DirtyFlags flag)
        {
#if UNITY_EDITOR
            //EditorUtility.SetDirty(this);
            if (flag == DirtyFlags.All || flag == DirtyFlags.Geometry)
            {
                EditorUtility.SetDirty(Geometry);
            }
            if (flag == DirtyFlags.All || flag == DirtyFlags.Shading)
            {
                EditorUtility.SetDirty(Shading);
            }
            if (flag == DirtyFlags.All || flag == DirtyFlags.Rendering)
            {
                EditorUtility.SetDirty(Rendering);
            }
            if (flag == DirtyFlags.All || flag == DirtyFlags.Foliage)
            {
                EditorUtility.SetDirty(Foliage);
            }
#endif
            Shading.UpdateMaterials();
            if (Dirty != null)
            {
                Dirty(flag);
            }

            if (GlobalDirty != null)
            {
                GlobalDirty(this, flag);
            }
        }

        public void CopyTo(GTerrainData des)
        {
            Geometry.CopyTo(des.Geometry);
            Shading.CopyTo(des.Shading);
            Rendering.CopyTo(des.Rendering);
            Foliage.CopyTo(des.Foliage);
        }
    }
}
