using UnityEngine;

namespace Pinwheel.Griffin.SplineTool
{
    [System.Serializable]
    public struct GSplineToolSettings
    {
        [SerializeField]
        private Color anchorColor;
        public Color AnchorColor
        {
            get
            {
                return anchorColor;
            }
            set
            {
                anchorColor = value;
            }
        }

        [SerializeField]
        private Color segmentColor;
        public Color SegmentColor
        {
            get
            {
                return segmentColor;
            }
            set
            {
                segmentColor = value;
            }
        }

        [SerializeField]
        private Color meshColor;
        public Color MeshColor
        {
            get
            {
                return meshColor;
            }
            set
            {
                meshColor = value;
            }
        }

        [SerializeField]
        private Color selectedElementColor;
        public Color SelectedElementColor
        {
            get
            {
                return selectedElementColor;
            }
            set
            {
                selectedElementColor = value;
            }
        }

        [SerializeField]
        private Color positiveHighlightColor;
        public Color PositiveHighlightColor
        {
            get
            {
                return positiveHighlightColor;
            }
            set
            {
                positiveHighlightColor = value;
            }
        }

        [SerializeField]
        private Color negativeHighlighColor;
        public Color NegativeHighlightColor
        {
            get
            {
                return negativeHighlighColor;
            }
            set
            {
                negativeHighlighColor = value;
            }
        }

        [SerializeField]
        private bool showMesh;
        public bool ShowMesh
        {
            get
            {
                return showMesh;
            }
            set
            {
                showMesh = value;
            }
        }
    }
}
