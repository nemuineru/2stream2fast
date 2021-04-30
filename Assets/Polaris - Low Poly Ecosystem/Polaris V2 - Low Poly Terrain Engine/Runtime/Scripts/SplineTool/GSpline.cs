using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Griffin.SplineTool
{
    [System.Serializable]
    public class GSpline
    {
        [SerializeField]
        private List<GSplineAnchor> anchors;
        public List<GSplineAnchor> Anchors
        {
            get
            {
                if (anchors == null)
                {
                    anchors = new List<GSplineAnchor>();
                }
                return anchors;
            }
        }

        [SerializeField]
        private List<GSplineSegment> segments;
        public List<GSplineSegment> Segments
        {
            get
            {
                if (segments == null)
                {
                    segments = new List<GSplineSegment>();
                }
                return segments;
            }
        }

        public bool HasBranch
        {
            get
            {
                return CheckHasBranch();
            }
        }

        public bool IsSegmentValid(int segmentIndex)
        {
            GSplineSegment s = Segments[segmentIndex];
            if (s == null)
                return false;
            bool startIndexValid =
                s.StartIndex >= 0 &&
                s.StartIndex < Anchors.Count &&
                Anchors[s.StartIndex] != null;
            bool endIndexValid =
                s.EndIndex >= 0 &&
                s.EndIndex < Anchors.Count &&
                Anchors[s.EndIndex] != null;
            return startIndexValid && endIndexValid;
        }

        public void RemoveAnchor(int index)
        {
            GSplineAnchor a = Anchors[index];
            Segments.RemoveAll(s => s.StartIndex == index || s.EndIndex == index);
            Segments.ForEach(s =>
            {
                if (s.StartIndex > index)
                    s.StartIndex -= 1;
                if (s.EndIndex > index)
                    s.EndIndex -= 1;
            });
            Anchors.RemoveAt(index);
        }

        public GSplineSegment AddSegment(int startIndex, int endIndex)
        {
            GSplineSegment s = Segments.Find(s0 =>
                (s0.StartIndex == startIndex && s0.EndIndex == endIndex) ||
                (s0.StartIndex == endIndex && s0.EndIndex == startIndex));
            if (s != null)
                return s;
            GSplineSegment newSegment = new GSplineSegment();
            newSegment.StartIndex = startIndex;
            newSegment.EndIndex = endIndex;
            Segments.Add(newSegment);
            GSplineAnchor startAnchor = Anchors[newSegment.StartIndex];
            GSplineAnchor endAnchor = Anchors[newSegment.EndIndex];
            newSegment.StartTangent = (endAnchor.Position - startAnchor.Position) / 3f;
            newSegment.EndTangent = -newSegment.StartTangent;
            return newSegment;
        }

        public Vector3 EvaluatePosition(int segmentIndex, float t)
        {
            GSplineSegment s = Segments[segmentIndex];
            GSplineAnchor startAnchor = Anchors[s.StartIndex];
            GSplineAnchor endAnchor = Anchors[s.EndIndex];

            Vector3 p0 = startAnchor.Position;
            Vector3 p1 = startAnchor.Position + s.StartTangent;
            Vector3 p2 = endAnchor.Position + s.EndTangent;
            Vector3 p3 = endAnchor.Position;

            t = Mathf.Clamp01(t);
            float oneMinusT = 1 - t;
            Vector3 p =
                oneMinusT * oneMinusT * oneMinusT * p0 +
                3 * oneMinusT * oneMinusT * t * p1 +
                3 * oneMinusT * t * t * p2 +
                t * t * t * p3;
            return p;
        }

        public Quaternion EvaluateRotation(int segmentIndex, float t)
        {
            GSplineSegment s = Segments[segmentIndex];
            GSplineAnchor startAnchor = Anchors[s.StartIndex];
            GSplineAnchor endAnchor = Anchors[s.EndIndex];
            return Quaternion.Lerp(startAnchor.Rotation, endAnchor.Rotation, t);
        }

        public Vector3 EvaluateScale(int segmentIndex, float t)
        {
            GSplineSegment s = Segments[segmentIndex];
            GSplineAnchor startAnchor = Anchors[s.StartIndex];
            GSplineAnchor endAnchor = Anchors[s.EndIndex];
            return Vector3.Lerp(startAnchor.Scale, endAnchor.Scale, t);
        }

        public Vector3 EvaluateUpVector(int segmentIndex, float t)
        {
            Quaternion rotation = EvaluateRotation(segmentIndex, t);
            Matrix4x4 matrix = Matrix4x4.Rotate(rotation);
            return matrix.MultiplyVector(Vector3.up);
        }

        private bool CheckHasBranch()
        {
            int[] count = new int[Anchors.Count];
            for (int i = 0; i < Segments.Count; ++i)
            {
                if (!IsSegmentValid(i))
                    continue;
                GSplineSegment s = Segments[i];
                count[s.StartIndex] += 1;
                count[s.EndIndex] += 1;
                if (count[s.StartIndex] > 2 || count[s.EndIndex] > 2)
                    return true;
            }

            return false;
        }
    }
}
