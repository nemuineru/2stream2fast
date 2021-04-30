using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Griffin.SplineTool
{
    [System.Serializable]
    [ExecuteInEditMode]
    public class GSplineCreator : MonoBehaviour
    {
        public delegate void SplineChangedHandler(GSplineCreator sender);
        public static event SplineChangedHandler Editor_SplineChanged;

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
        private Vector3 positionOffset;
        public Vector3 PositionOffset
        {
            get
            {
                return positionOffset;
            }
            set
            {
                positionOffset = value;
            }
        }

        [SerializeField]
        private Quaternion initialRotation;
        public Quaternion InitialRotation
        {
            get
            {
                return initialRotation;
            }
            set
            {
                initialRotation = value;
            }
        }

        [SerializeField]
        private Vector3 initialScale;
        public Vector3 InitialScale
        {
            get
            {
                return initialScale;
            }
            set
            {
                initialScale = value;
            }
        }

        [SerializeField]
        private int smoothness;
        public int Smoothness
        {
            get
            {
                return smoothness;
            }
            set
            {
                smoothness = Mathf.Max(2, value);
            }
        }

        [SerializeField]
        private float width;
        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float falloffWidth;
        public float FalloffWidth
        {
            get
            {
                return falloffWidth;
            }
            set
            {
                falloffWidth = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private GSpline spline;
        public GSpline Spline
        {
            get
            {
                if (spline == null)
                {
                    spline = new GSpline();
                }
                return spline;
            }
            set
            {
                spline = value;
            }
        }

#if UNITY_EDITOR
        private List<Vector4> vertices;
        public List<Vector4> Editor_Vertices
        {
            get
            {
                if (vertices == null)
                    vertices = new List<Vector4>();
                return vertices;
            }
            set
            {
                vertices = value;
            }
        }
#endif

        public void Reset()
        {
            PositionOffset = Vector3.zero;
            InitialRotation = Quaternion.identity;
            InitialScale = Vector3.one;
            Smoothness = 20;
            Width = 1;
            FalloffWidth = 1;
        }

        public GSplineAnchor AddAnchorAutoTangent(Vector3 worldPositionNoOffset, int activeAnchorIndex = -1)
        {
            Vector3 position = worldPositionNoOffset + PositionOffset;
            Quaternion rotation = InitialRotation;
            Vector3 scale = InitialScale;
            GSplineAnchor a = new GSplineAnchor();
            a.Position = position;
            a.Rotation = rotation;
            a.Scale = scale;
            Spline.Anchors.Add(a);
            if (activeAnchorIndex >= 0 && activeAnchorIndex < Spline.Anchors.Count)
            {
                Spline.AddSegment(activeAnchorIndex, Spline.Anchors.Count - 1);
            }

            return a;
        }

        public void AdjustTangent(int segmentIndex)
        {
            GSplineSegment s = Spline.Segments[segmentIndex];
            int startCount = 0;
            Vector3 startAvgTangent = Vector3.zero;
            int endCount = 0;
            Vector3 endAvgTangent = Vector3.zero;
            for (int i = 0; i < Spline.Segments.Count; ++i)
            {
                GSplineSegment s0 = Spline.Segments[i];
                if (s0 == s)
                    continue;
                if (s0.StartIndex == s.StartIndex)
                {
                    startAvgTangent += s0.StartTangent;
                    startCount += 1;
                }
                if (s0.EndIndex == s.StartIndex)
                {
                    startAvgTangent += s0.EndTangent;
                    startCount += 1;
                }
                if (s0.StartIndex == s.EndIndex)
                {
                    endAvgTangent += s0.StartTangent;
                    endCount += 1;
                }
                if (s0.EndIndex == s.EndIndex)
                {
                    endAvgTangent += s0.EndTangent;
                    endCount += 1;
                }
            }

            if (startAvgTangent == Vector3.zero)
                startCount = 0;
            if (endAvgTangent == Vector3.zero)
                endCount = 0;

            if (startCount == 0 && endCount == 0)
            {
                GSplineAnchor startAnchor = Spline.Anchors[s.StartIndex];
                GSplineAnchor endAnchor = Spline.Anchors[s.EndIndex];
                s.StartTangent = (endAnchor.Position - startAnchor.Position) / 3f;
                s.EndTangent = -s.StartTangent;
            }
            else if (startCount == 0 && endCount > 0)
            {
                s.EndTangent = -endAvgTangent / endCount;
                s.StartTangent = s.EndTangent;
            }
            else if (startCount > 0 && endCount == 0)
            {
                s.StartTangent = -startAvgTangent / startCount;
                s.EndTangent = s.StartTangent;
            }
            else
            {
                s.StartTangent = -startAvgTangent / startCount;
                s.EndTangent = -endAvgTangent / endCount;
            }
        }

        public List<Vector3> GenerateVertices()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<GSplineAnchor> anchors = Spline.Anchors;
            List<GSplineSegment> segments = Spline.Segments;

            for (int sIndex = 0; sIndex < segments.Count; ++sIndex)
            {
                float tStep = 1f / (Smoothness - 1);
                for (int tIndex = 0; tIndex < Smoothness - 1; ++tIndex)
                {
                    float halfWidth = Width * 0.5f;
                    float t0 = tIndex * tStep;
                    Vector3 translation0 = Spline.EvaluatePosition(sIndex, t0);
                    Quaternion rotation0 = Spline.EvaluateRotation(sIndex, t0);
                    Vector3 scale0 = Spline.EvaluateScale(sIndex, t0);

                    Matrix4x4 matrix0 = Matrix4x4.TRS(translation0, rotation0, scale0);
                    Vector3 bl = matrix0.MultiplyPoint(new Vector3(-halfWidth, 0, 0));
                    Vector3 br = matrix0.MultiplyPoint(new Vector3(halfWidth, 0, 0));

                    float t1 = (tIndex + 1) * tStep;
                    Vector3 translation1 = Spline.EvaluatePosition(sIndex, t1);
                    Quaternion rotation1 = Spline.EvaluateRotation(sIndex, t1);
                    Vector3 scale1 = Spline.EvaluateScale(sIndex, t1);

                    Matrix4x4 matrix1 = Matrix4x4.TRS(translation1, rotation1, scale1);
                    Vector3 tl = matrix1.MultiplyPoint(new Vector3(-halfWidth, 0, 0));
                    Vector3 tr = matrix1.MultiplyPoint(new Vector3(halfWidth, 0, 0));

                    vertices.Add(bl);
                    vertices.Add(tl);
                    vertices.Add(tr);

                    vertices.Add(bl);
                    vertices.Add(tr);
                    vertices.Add(br);
                }
            }

            return vertices;
        }

        public List<Vector4> GenerateVerticesWithFalloff()
        {
            List<Vector4> vertices = new List<Vector4>();
            List<GSplineAnchor> anchors = Spline.Anchors;
            List<GSplineSegment> segments = Spline.Segments;

            for (int sIndex = 0; sIndex < segments.Count; ++sIndex)
            {
                float tStep = 1f / (Smoothness - 1);
                for (int tIndex = 0; tIndex < Smoothness - 1; ++tIndex)
                {
                    float t0 = tIndex * tStep;
                    Vector3 translation0 = Spline.EvaluatePosition(sIndex, t0);
                    Quaternion rotation0 = Spline.EvaluateRotation(sIndex, t0);
                    Vector3 scale0 = Spline.EvaluateScale(sIndex, t0);

                    float t1 = (tIndex + 1) * tStep;
                    Vector3 translation1 = Spline.EvaluatePosition(sIndex, t1);
                    Quaternion rotation1 = Spline.EvaluateRotation(sIndex, t1);
                    Vector3 scale1 = Spline.EvaluateScale(sIndex, t1);

                    Matrix4x4 matrix0 = Matrix4x4.TRS(translation0, rotation0, scale0);
                    Matrix4x4 matrix1 = Matrix4x4.TRS(translation1, rotation1, scale1);

                    Vector3 bl, tl, tr, br;
                    float halfWidth = Width * 0.5f;

                    if (FalloffWidth > 0)
                    {
                        //Left falloff
                        bl = matrix0.MultiplyPoint(new Vector3(-halfWidth - FalloffWidth, 0, 0));
                        tl = matrix1.MultiplyPoint(new Vector3(-halfWidth - FalloffWidth, 0, 0));
                        tr = matrix1.MultiplyPoint(new Vector3(-halfWidth, 0, 0));
                        br = matrix0.MultiplyPoint(new Vector3(-halfWidth, 0, 0));
                        vertices.Add(new Vector4(bl.x, bl.y, bl.z, 0));
                        vertices.Add(new Vector4(tl.x, tl.y, tl.z, 0));
                        vertices.Add(new Vector4(tr.x, tr.y, tr.z, 1));
                        vertices.Add(new Vector4(bl.x, bl.y, bl.z, 0));
                        vertices.Add(new Vector4(tr.x, tr.y, tr.z, 1));
                        vertices.Add(new Vector4(br.x, br.y, br.z, 1));
                    }

                    if (Width > 0)
                    {
                        //Center
                        bl = matrix0.MultiplyPoint(new Vector3(-halfWidth, 0, 0));
                        tl = matrix1.MultiplyPoint(new Vector3(-halfWidth, 0, 0));
                        tr = matrix1.MultiplyPoint(new Vector3(halfWidth, 0, 0));
                        br = matrix0.MultiplyPoint(new Vector3(halfWidth, 0, 0));
                        vertices.Add(new Vector4(bl.x, bl.y, bl.z, 1));
                        vertices.Add(new Vector4(tl.x, tl.y, tl.z, 1));
                        vertices.Add(new Vector4(tr.x, tr.y, tr.z, 1));
                        vertices.Add(new Vector4(bl.x, bl.y, bl.z, 1));
                        vertices.Add(new Vector4(tr.x, tr.y, tr.z, 1));
                        vertices.Add(new Vector4(br.x, br.y, br.z, 1));
                    }

                    if (FalloffWidth > 0)
                    {
                        //Right falloff
                        bl = matrix0.MultiplyPoint(new Vector3(halfWidth, 0, 0));
                        tl = matrix1.MultiplyPoint(new Vector3(halfWidth, 0, 0));
                        tr = matrix1.MultiplyPoint(new Vector3(halfWidth + FalloffWidth, 0, 0));
                        br = matrix0.MultiplyPoint(new Vector3(halfWidth + FalloffWidth, 0, 0));
                        vertices.Add(new Vector4(bl.x, bl.y, bl.z, 1));
                        vertices.Add(new Vector4(tl.x, tl.y, tl.z, 1));
                        vertices.Add(new Vector4(tr.x, tr.y, tr.z, 0));
                        vertices.Add(new Vector4(bl.x, bl.y, bl.z, 1));
                        vertices.Add(new Vector4(tr.x, tr.y, tr.z, 0));
                        vertices.Add(new Vector4(br.x, br.y, br.z, 0));
                    }
                }
            }

            return vertices;

        }

        public IEnumerable<Rect> SweepDirtyRect(GStylizedTerrain terrain)
        {
            if (terrain.TerrainData == null)
                return new List<Rect>();
            int gridSize = terrain.TerrainData.Geometry.ChunkGridSize;
            List<Rect> uvRects = new List<Rect>();
            for (int x = 0; x < gridSize; ++x)
            {
                for (int z = 0; z < gridSize; ++z)
                {
                    uvRects.Add(GCommon.GetUvRange(gridSize, x, z));
                }
            }

            HashSet<Rect> dirtyRects = new HashSet<Rect>();
            Vector3 terrainSize = new Vector3(
                terrain.TerrainData.Geometry.Width,
                terrain.TerrainData.Geometry.Height,
                terrain.TerrainData.Geometry.Length);
            float splineSize = Mathf.Max(1, Width + FalloffWidth * 2);
            Vector2 sweepRectSize = new Vector2(
                Mathf.InverseLerp(0, terrainSize.x, splineSize),
                Mathf.InverseLerp(0, terrainSize.z, splineSize));
            Rect sweepRect = new Rect();
            sweepRect.size = sweepRectSize;

            int segmentCount = Spline.Segments.Count;
            for (int sIndex = 0; sIndex < segmentCount; ++sIndex)
            {
                float tStep = 1f / (Smoothness - 1);
                for (int tIndex = 0; tIndex < Smoothness - 1; ++tIndex)
                {
                    float t = tIndex * tStep;
                    Vector3 worldPos = Spline.EvaluatePosition(sIndex, t);
                    Vector3 scale = Spline.EvaluateScale(sIndex, t);
                    float maxScaleComponent = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
                    Vector3 normalizedPos = terrain.WorldPointToNormalized(worldPos);
                    sweepRect.center = new Vector2(normalizedPos.x, normalizedPos.z);
                    sweepRect.size = sweepRectSize * maxScaleComponent;
                    for (int rIndex = 0; rIndex < uvRects.Count; ++rIndex)
                    {
                        if (uvRects[rIndex].Overlaps(sweepRect))
                        {
                            dirtyRects.Add(uvRects[rIndex]);
                        }
                    }
                }
            }

            return dirtyRects;
        }

        public static void MarkSplineChanged(GSplineCreator sender)
        {
            if (Editor_SplineChanged != null)
            {
                Editor_SplineChanged.Invoke(sender);
            }
        }
    }
}
