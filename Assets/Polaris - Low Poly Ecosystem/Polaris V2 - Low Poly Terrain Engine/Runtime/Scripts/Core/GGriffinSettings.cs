using Pinwheel.Griffin.BackupTool;
using Pinwheel.Griffin.BillboardTool;
using Pinwheel.Griffin.PaintTool;
using Pinwheel.Griffin.SplineTool;
using Pinwheel.Griffin.StampTool;
using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Griffin
{
    //[CreateAssetMenu(fileName = "GriffinSettings", menuName = "Griffin/Settings")]
    public class GGriffinSettings : ScriptableObject
    {
        private static GGriffinSettings instance;
        public static GGriffinSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<GGriffinSettings>("GriffinSettings");
                    if (instance == null)
                    {
                        instance = ScriptableObject.CreateInstance<GGriffinSettings>();
                    }
                }
                return instance;
            }
        }

        [Header("Core")]
        [SerializeField]
        private bool enableEditorAnalytics;
        public bool EnableEditorAnalytics
        {
            get
            {
                return enableEditorAnalytics;
            }
            set
            {
                enableEditorAnalytics = value;
            }
        }

        [SerializeField]
        private bool debugMode;
        public bool DebugMode
        {
            get
            {
                return debugMode;
            }
            set
            {
                debugMode = value;
            }
        }

        [SerializeField]
        private bool showLivePreview;
        public bool ShowLivePreview
        {
            get
            {
                return showLivePreview;
            }
            set
            {
                showLivePreview = value;
            }
        }

#pragma warning disable 0649
        [SerializeField]
        //[HideInInspector]
        public Mesh[] livePreviewMeshes;

        [SerializeField]
        //[HideInInspector]
        public Mesh[] livePreviewWireframeMeshes;

        [SerializeField]
        //[HideInInspector]
        private Mesh grassQuad;

        [SerializeField]
        //[HideInInspector]
        private Mesh grassCross;

        [SerializeField]
        //[HideInInspector]
        private Mesh grassTriCross;
#pragma warning restore 0649

        [SerializeField]
        private bool showGeometryChunksInHierarchy;
        public bool ShowGeometryChunksInHierarchy
        {
            get
            {
                return showGeometryChunksInHierarchy;
            }
            set
            {
                showGeometryChunksInHierarchy = value;
                IEnumerator<GStylizedTerrain> terrains = GStylizedTerrain.ActiveTerrains.GetEnumerator();
                while (terrains.MoveNext())
                {
                    GStylizedTerrain t = terrains.Current;
                    Transform chunkRoot = t.Internal_ChunkRoot;
                    chunkRoot.gameObject.hideFlags = showGeometryChunksInHierarchy ? HideFlags.None : HideFlags.HideInHierarchy;
                }
                GUtilities.MarkCurrentSceneDirty();
            }
        }

        [SerializeField]
        private int triangulateIteration;
        public int TriangulateIteration
        {
            get
            {
                return triangulateIteration;
            }
            set
            {
                triangulateIteration = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private AnimationCurve lodTransition;
        public AnimationCurve LodTransition
        {
            get
            {
                if (lodTransition == null)
                {
                    lodTransition = AnimationCurve.EaseInOut(0, 1, 0, 0);
                }
                return lodTransition;
            }
        }

        [SerializeField]
        private GTerrainDataDefault terrainDataDefault;
        public GTerrainDataDefault TerrainDataDefault
        {
            get
            {
                return terrainDataDefault;
            }
            set
            {
                terrainDataDefault = value;
            }
        }

        private bool isHidingFoliageOnEditing;
        public bool IsHidingFoliageOnEditing
        {
            get
            {
                return isHidingFoliageOnEditing;
            }
            set
            {
                isHidingFoliageOnEditing = value;
            }
        }

        [SerializeField]
        private string defaultTerrainDataDirectory;
        public string DefaultTerrainDataDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(defaultTerrainDataDirectory))
                {
                    return "Assets/";
                }
                else
                {
                    return defaultTerrainDataDirectory;
                }
            }
            private set
            {
                defaultTerrainDataDirectory = value;
            }
        }

        [SerializeField]
        private Texture2D defaultNoiseTexture;
        public Texture2D DefaultNoiseTexture
        {
            get
            {
                return defaultNoiseTexture;
            }
            set
            {
                defaultNoiseTexture = value;
            }
        }

        [Header("Backup Tool")]
        [SerializeField]
        private GBackupToolSettings backupToolSettings;
        public GBackupToolSettings BackupToolSettings
        {
            get
            {
                return backupToolSettings;
            }
            set
            {
                backupToolSettings = value;
            }
        }

        [Header("Paint Tool")]
        [SerializeField]
        private GPaintToolSettings paintToolSettings;
        public GPaintToolSettings PaintToolSettings
        {
            get
            {
                return paintToolSettings;
            }
            set
            {
                paintToolSettings = value;
            }
        }

        [Header("Spline Tool")]
        [SerializeField]
        private GSplineToolSettings splineToolSettings;
        public GSplineToolSettings SplineToolSettings
        {
            get
            {
                return splineToolSettings;
            }
            set
            {
                splineToolSettings = value;
            }
        }

        [Header("Billboard Tool")]
        [SerializeField]
        private GBillboardToolSettings billboardToolSettings;
        public GBillboardToolSettings BillboardToolSettings
        {
            get
            {
                return billboardToolSettings;
            }
            set
            {
                billboardToolSettings = value;
            }
        }

        [Header("Stamp Tool")]
        [SerializeField]
        private GStampToolSettings stampToolSettings;
        public GStampToolSettings StampToolSettings
        {
            get
            {
                return stampToolSettings;
            }
            set
            {
                stampToolSettings = value;
            }
        }

        [Header("Wizard Settings")]
        [SerializeField]
        private GCreateTerrainWizardSettings wizardSettings;
        public GCreateTerrainWizardSettings WizardSettings
        {
            get
            {
                return wizardSettings;
            }
            set
            {
                wizardSettings = value;
            }
        }

        [Header("Internal Shader Settings")]
        [SerializeField]
        private GInternalShaderSettings internalShaderSettings;
        public GInternalShaderSettings InternalShaderSettings
        {
            get
            {
                return internalShaderSettings;
            }
            set
            {
                internalShaderSettings = value;
            }
        }

        private void OnValidate()
        {
            ShowGeometryChunksInHierarchy = showGeometryChunksInHierarchy;
        }

        public void Reset()
        {
            TriangulateIteration = 100;
            DefaultTerrainDataDirectory = string.Empty;
        }

        public Mesh GetLivePreviewMesh(int detail)
        {
            if (livePreviewMeshes == null || livePreviewMeshes.Length == 0)
                return null;
            detail = Mathf.Clamp(detail, 0, livePreviewMeshes.Length - 1);
            return livePreviewMeshes[detail];
        }

        public Mesh GetLivePreviewWireframeMesh(int detail)
        {
            if (livePreviewWireframeMeshes == null || livePreviewWireframeMeshes.Length == 0)
                return null;
            detail = Mathf.Clamp(detail, 0, livePreviewWireframeMeshes.Length - 1);
            return livePreviewWireframeMeshes[detail];
        }

        public Mesh GetGrassMesh(GGrassShape shape)
        {
            if (shape == GGrassShape.Quad)
                return grassQuad;
            else if (shape == GGrassShape.Cross)
                return grassCross;
            else if (shape == GGrassShape.TriCross)
                return grassTriCross;
            else
                return null;
        }
    }
}
