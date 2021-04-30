using UnityEngine;
using System.Collections.Generic;
using Type = System.Type;
using Rand = System.Random;
#if UNITY_EDITOR
using Pinwheel.Griffin.BackupTool;
#endif

namespace Pinwheel.Griffin.PaintTool
{
    [System.Serializable]
    [ExecuteInEditMode]
    public class GTerrainTexturePainter : MonoBehaviour
    {
        public const float GEOMETRY_OPACITY_EXPONENT = 3;

        private static readonly List<string> BUILTIN_PAINTER_NAME = new List<string>(new string[]
        {
            "GElevationPainter",
            "GHeightSamplingPainter",
            "GTerracePainter",
            "GRemapPainter",
            "GNoisePainter",
            "GSubDivPainter",
            "GVisibilityPainter",
            "GAlbedoPainter",
            "GMetallicPainter",
            "GSmoothnessPainter",
            "GSplatPainter"
        });

        private static List<Type> customPainterTypes;
        private static List<Type> CustomPainterTypes
        {
            get
            {
                if (customPainterTypes == null)
                    customPainterTypes = new List<Type>();
                return customPainterTypes;
            }
            set
            {
                customPainterTypes = value;
            }
        }

        public static string TexturePainterInterfaceName
        {
            get
            {
                return typeof(IGTexturePainter).Name;
            }
        }

        static GTerrainTexturePainter()
        {
            RefreshCustomPainterTypes();
        }

        public static void RefreshCustomPainterTypes()
        {
            List<Type> loadedTypes = GCommon.GetAllLoadedTypes();
            CustomPainterTypes = loadedTypes.FindAll(
                t => t.GetInterface(TexturePainterInterfaceName) != null &&
                !BUILTIN_PAINTER_NAME.Contains(t.Name));
        }

        public static List<Type> GetCustomPainterTypes()
        {
            return CustomPainterTypes;
        }

        [SerializeField]
        private int groupId;
        public int GroupId
        {
            get
            {
                return groupId;
            }
            set
            {
                groupId = value;
            }
        }

        [SerializeField]
        private GTexturePaintingMode mode;
        public GTexturePaintingMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
            }
        }

        [SerializeField]
        private int customPainterIndex;
        public int CustomPainterIndex
        {
            get
            {
                return customPainterIndex;
            }
            set
            {
                customPainterIndex = value;
            }
        }

        [SerializeField]
        private string customPainterArgs;
        public string CustomPainterArgs
        {
            get
            {
                return customPainterArgs;
            }
            set
            {
                customPainterArgs = value;
            }
        }

#if UNITY_EDITOR
        [SerializeField]
        private bool editor_EnableHistory = true;
        public bool Editor_EnableHistory
        {
            get
            {
                return editor_EnableHistory;
            }
            set
            {
                editor_EnableHistory = value;
            }
        }

        [SerializeField]
        private bool editor_EnableLivePreview = true;
        public bool Editor_EnableLivePreview
        {
            get
            {
                return editor_EnableLivePreview;
            }
            set
            {
                editor_EnableLivePreview = value;
            }
        }
#endif

        [SerializeField]
        private bool forceUpdateGeometry;
        public bool ForceUpdateGeometry
        {
            get
            {
                return forceUpdateGeometry;
            }
            set
            {
                forceUpdateGeometry = value;
            }
        }

        public IGTexturePainter ActivePainter
        {
            get
            {
                if (Mode == GTexturePaintingMode.Elevation)
                {
                    return new GElevationPainter();
                }
                else if (Mode == GTexturePaintingMode.HeightSampling)
                {
                    return new GHeightSamplingPainter();
                }
                else if (Mode == GTexturePaintingMode.Terrace)
                {
                    return new GTerracePainter();
                }
                else if (Mode == GTexturePaintingMode.Remap)
                {
                    return new GRemapPainter();
                }
                else if (Mode == GTexturePaintingMode.Noise)
                {
                    return new GNoisePainter();
                }
                else if (Mode == GTexturePaintingMode.SubDivision)
                {
                    return new GSubDivPainter();
                }
                else if (Mode == GTexturePaintingMode.Visibility)
                {
                    return new GVisibilityPainter();
                }
                else if (Mode == GTexturePaintingMode.Albedo)
                {
                    return new GAlbedoPainter();
                }
                else if (Mode == GTexturePaintingMode.Metallic)
                {
                    return new GMetallicPainter();
                }
                else if (Mode == GTexturePaintingMode.Smoothness)
                {
                    return new GSmoothnessPainter();
                }
                else if (Mode == GTexturePaintingMode.Splat)
                {
                    return new GSplatPainter();
                }
                else if (mode == GTexturePaintingMode.Custom)
                {
                    if (CustomPainterIndex >= 0 && CustomPainterIndex < CustomPainterTypes.Count)
                        return System.Activator.CreateInstance(CustomPainterTypes[CustomPainterIndex]) as IGTexturePainter;
                }
                return null;
            }
        }

        [SerializeField]
        private float brushRadius;
        public float BrushRadius
        {
            get
            {
                return brushRadius;
            }
            set
            {
                brushRadius = Mathf.Max(0.01f, value);
            }
        }

        [SerializeField]
        private float brushRadiusJitter;
        public float BrushRadiusJitter
        {
            get
            {
                return brushRadiusJitter;
            }
            set
            {
                brushRadiusJitter = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float brushRotation;
        public float BrushRotation
        {
            get
            {
                return brushRotation;
            }
            set
            {
                brushRotation = value;
            }
        }

        [SerializeField]
        private float brushRotationJitter;
        public float BrushRotationJitter
        {
            get
            {
                return brushRotationJitter;
            }
            set
            {
                brushRotationJitter = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float brushOpacity;
        public float BrushOpacity
        {
            get
            {
                return brushOpacity;
            }
            set
            {
                brushOpacity = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float brushOpacityJitter;
        public float BrushOpacityJitter
        {
            get
            {
                return brushOpacityJitter;
            }
            set
            {
                brushOpacityJitter = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float brushTargetStrength = 1;
        public float BrushTargetStrength
        {
            get
            {
                return brushTargetStrength;
            }
            set
            {
                brushTargetStrength = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float brushScatter;
        public float BrushScatter
        {
            get
            {
                return brushScatter;
            }
            set
            {
                brushScatter = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float brushScatterJitter;
        public float BrushScatterJitter
        {
            get
            {
                return brushScatterJitter;
            }
            set
            {
                brushScatterJitter = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private Color brushColor;
        public Color BrushColor
        {
            get
            {
                return brushColor;
            }
            set
            {
                brushColor = value;
            }
        }

        [SerializeField]
        private List<Texture2D> brushMasks;
        public List<Texture2D> BrushMasks
        {
            get
            {
                if (brushMasks == null)
                    brushMasks = new List<Texture2D>();
                return brushMasks;
            }
            set
            {
                brushMasks = value;
            }
        }

        [SerializeField]
        private int selectedBrushMaskIndex;
        public int SelectedBrushMaskIndex
        {
            get
            {
                return selectedBrushMaskIndex;
            }
            set
            {
                if (BrushMasks.Count > 0)
                    selectedBrushMaskIndex = Mathf.Clamp(value, 0, BrushMasks.Count);
                else
                    selectedBrushMaskIndex = -1;
            }
        }

        [SerializeField]
        private int selectedSplatIndex;
        public int SelectedSplatIndex
        {
            get
            {
                return selectedSplatIndex;
            }
            set
            {
                selectedSplatIndex = value;
            }
        }

        [SerializeField]
        private Vector3 samplePoint;
        public Vector3 SamplePoint
        {
            get
            {
                return samplePoint;
            }
            set
            {
                samplePoint = value;
            }
        }

        internal static Dictionary<string, RenderTexture> internal_RenderTextures;

        private void OnEnable()
        {
            ReloadBrushMasks();
        }

        private void OnDisable()
        {
            Internal_ReleaseRenderTextures();
        }

        private void Reset()
        {
            GroupId = 0;
            Mode = GTexturePaintingMode.Elevation;
            BrushRadius = 50;
            BrushRadiusJitter = 0;
            BrushOpacity = 0.5f;
            BrushOpacityJitter = 0;
            BrushTargetStrength = 1;
            BrushRotation = 0;
            BrushRotationJitter = 0;
            BrushColor = Color.white;
        }

        public void ReloadBrushMasks()
        {
            BrushMasks = new List<Texture2D>(Resources.LoadAll<Texture2D>(GCommon.BRUSH_MASK_RESOURCES_PATH));
        }

        public void Paint(GTexturePainterArgs args)
        {
            IGTexturePainter p = ActivePainter;
            if (p == null)
                return;

            FillArgs(ref args);

#if UNITY_EDITOR
            if (args.MouseEventType == GPainterMouseEventType.Down)
            {
                Editor_CreateInitialHistoryEntry(args);
            }
#endif

            IEnumerator<GStylizedTerrain> terrains = GStylizedTerrain.ActiveTerrains.GetEnumerator();
            while (terrains.MoveNext())
            {
                if (terrains.Current.GroupId != GroupId && GroupId >= 0)
                    continue;
                p.Paint(terrains.Current, args);
            }

#if UNITY_EDITOR
            if (args.MouseEventType == GPainterMouseEventType.Up)
            {
                Editor_CreateHistory(args);
            }
#endif
        }

#if UNITY_EDITOR
        private void Editor_CreateInitialHistoryEntry(GTexturePainterArgs args)
        {
            if (!Editor_EnableHistory)
                return;

            List<GTerrainResourceFlag> flags = new List<GTerrainResourceFlag>();
            flags.AddRange(ActivePainter.GetResourceFlagForHistory(args));
            GBackup.TryCreateInitialBackup(ActivePainter.HistoryPrefix, GroupId, flags, false);
        }

        private void Editor_CreateHistory(GTexturePainterArgs args)
        {
            if (!Editor_EnableHistory)
                return;
            List<GTerrainResourceFlag> flags = new List<GTerrainResourceFlag>();
            flags.AddRange(ActivePainter.GetResourceFlagForHistory(args));
            GBackup.TryCreateBackup(ActivePainter.HistoryPrefix, GroupId, flags, false);
        }
#endif

        internal static RenderTexture Internal_GetRenderTexture(GStylizedTerrain t, int resolution, int id = 0)
        {
            if (internal_RenderTextures == null)
            {
                internal_RenderTextures = new Dictionary<string, RenderTexture>();
            }

            string key = string.Format("{0}_{1}", t != null ? t.GetInstanceID() : 0, id);
            if (!internal_RenderTextures.ContainsKey(key) ||
                internal_RenderTextures[key] == null)
            {
                RenderTexture rt = new RenderTexture(resolution, resolution, 0, GGeometry.HeightMapRTFormat, RenderTextureReadWrite.Linear);
                internal_RenderTextures[key] = rt;
            }
            else if (internal_RenderTextures[key].width != resolution ||
                internal_RenderTextures[key].height != resolution ||
                internal_RenderTextures[key].format != GGeometry.HeightMapRTFormat)
            {
                internal_RenderTextures[key].Release();
                Object.DestroyImmediate(internal_RenderTextures[key]);
                RenderTexture rt = new RenderTexture(resolution, resolution, 0, GGeometry.HeightMapRTFormat, RenderTextureReadWrite.Linear);
                internal_RenderTextures[key] = rt;
            }

            internal_RenderTextures[key].wrapMode = TextureWrapMode.Clamp;

            return internal_RenderTextures[key];
        }

        public static void Internal_ReleaseRenderTextures()
        {
            if (internal_RenderTextures != null)
            {
                foreach (string k in internal_RenderTextures.Keys)
                {
                    RenderTexture rt = internal_RenderTextures[k];
                    if (rt == null)
                        continue;
                    rt.Release();
                    Object.DestroyImmediate(rt);
                }
            }
        }

        internal static RenderTexture Internal_GetRenderTexture(int resolution, int id = 0)
        {
            return Internal_GetRenderTexture(null, resolution, id);
        }

        private Rand GetRandomGenerator()
        {
            return new Rand(Time.frameCount);
        }

        private void ProcessBrushDynamic(ref GTexturePainterArgs args)
        {
            Rand rand = GetRandomGenerator();
            args.Radius -= BrushRadius * BrushRadiusJitter * (float)rand.NextDouble();
            args.Rotation += Mathf.Sign((float)rand.NextDouble() - 0.5f) * BrushRotation * BrushRotationJitter * (float)rand.NextDouble();
            args.Opacity -= BrushOpacity * BrushOpacityJitter * (float)rand.NextDouble();

            Vector3 scatterDir = new Vector3((float)(rand.NextDouble() * 2 - 1), 0, (float)(rand.NextDouble() * 2 - 1)).normalized;
            float scatterLengthMultiplier = BrushScatter - (float)rand.NextDouble() * BrushScatterJitter;
            float scatterLength = args.Radius * scatterLengthMultiplier;

            args.HitPoint += scatterDir * scatterLength;
        }

        public void FillArgs(ref GTexturePainterArgs args, bool useBrushDynamic = true)
        {
            args.Radius = BrushRadius;
            args.Rotation = BrushRotation;
            args.Opacity = BrushOpacity * BrushTargetStrength;
            args.Color = BrushColor;
            args.SplatIndex = SelectedSplatIndex;
            args.SamplePoint = SamplePoint;
            args.CustomArgs = CustomPainterArgs;
            args.ForceUpdateGeometry = ForceUpdateGeometry;
            if (SelectedBrushMaskIndex >= 0 && SelectedBrushMaskIndex < BrushMasks.Count)
            {
                args.Mask = BrushMasks[SelectedBrushMaskIndex];
            }

            if (args.ActionType == GPainterActionType.Alternative &&
                args.MouseEventType == GPainterMouseEventType.Down)
            {
                SamplePoint = args.HitPoint;
                args.SamplePoint = args.HitPoint;
            }

            if (useBrushDynamic)
            {
                ProcessBrushDynamic(ref args);
            }

            Vector3[] corners = GCommon.GetBrushQuadCorners(args.HitPoint, args.Radius, args.Rotation);
            args.WorldPointCorners = corners;
        }
    }
}
