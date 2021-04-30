using UnityEngine;

namespace Pinwheel.Griffin.PaintTool
{
    [System.Serializable]
    public struct GPaintToolSettings
    {
        public enum GPaintModeSelectorType
        {
            Grid, Dropdown
        }

        [SerializeField]
        private bool useSimpleCursor;
        public bool UseSimpleCursor
        {
            get
            {
                return useSimpleCursor;
            }
            set
            {
                useSimpleCursor = value;
            }
        }

        [SerializeField]
        private Color normalActionCursorColor;
        public Color NormalActionCursorColor
        {
            get
            {
                return normalActionCursorColor;
            }
            set
            {
                normalActionCursorColor = value;
            }
        }

        [SerializeField]
        private Color negativeActionCursorColor;
        public Color NegativeActionCursorColor
        {
            get
            {
                return negativeActionCursorColor;
            }
            set
            {
                negativeActionCursorColor = value;
            }
        }

        [SerializeField]
        private Color alternativeActionCursorColor;
        public Color AlternativeActionCursorColor
        {
            get
            {
                return alternativeActionCursorColor;
            }
            set
            {
                alternativeActionCursorColor = value;
            }
        }

        [SerializeField]
        private float radiusStep;
        public float RadiusStep
        {
            get
            {
                return radiusStep;
            }
            set
            {
                radiusStep = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float rotationStep;
        public float RotationStep
        {
            get
            {
                return rotationStep;
            }
            set
            {
                rotationStep = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float opacityStep;
        public float OpacityStep
        {
            get
            {
                return opacityStep;
            }
            set
            {
                opacityStep = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private int densityStep;
        public int DensityStep
        {
            get
            {
                return densityStep;
            }
            set
            {
                densityStep = value;
            }
        }

        [SerializeField]
        private GPaintModeSelectorType paintModeSelectorType;
        public GPaintModeSelectorType PaintModeSelectorType
        {
            get
            {
                return paintModeSelectorType;
            }
            set
            {
                paintModeSelectorType = value;
            }
        }
    }
}
