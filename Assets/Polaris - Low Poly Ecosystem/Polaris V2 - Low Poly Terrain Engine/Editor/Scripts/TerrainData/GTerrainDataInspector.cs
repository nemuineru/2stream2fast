using UnityEditor;
using UnityEngine;

namespace Pinwheel.Griffin
{
    [CustomEditor(typeof(GTerrainData))]
    public class GTerrainDataInspector : Editor
    {
        private GTerrainData instance;

        private void OnEnable()
        {
            instance = (GTerrainData)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Select the corresponding terrain in the scene to edit terrain data.", GEditorCommon.WordWrapItalicLabel);
        }
    }
}
