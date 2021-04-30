using UnityEditor;
using UnityEngine;

namespace Pinwheel.Griffin
{
    [CustomPropertyDrawer(typeof(GWizardMaterialTemplate))]
    public class GWizardMaterialTemplatePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            int lineIndex = -1;
            Rect r;

            lineIndex += 1;
            r = new Rect(position.x, position.y + lineIndex * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), position.width, EditorGUIUtility.singleLineHeight);

            SerializedProperty pipeline = property.FindPropertyRelative("pipeline");
            SerializedProperty lightingModel = property.FindPropertyRelative("lightingModel");
            SerializedProperty texturingModel = property.FindPropertyRelative("texturingModel");
            SerializedProperty splatsModel = property.FindPropertyRelative("splatsModel");
            SerializedProperty material = property.FindPropertyRelative("material");

            string headerLabel = string.Format("{0}_{1}_{2}",
                ((GRenderPipelineType)pipeline.enumValueIndex).ToString(),
                ((GLightingModel)lightingModel.enumValueIndex).ToString(),
                ((GTexturingModel)texturingModel.enumValueIndex).ToString());
            property.isExpanded = EditorGUI.Foldout(r, property.isExpanded, headerLabel);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel += 1;

                lineIndex += 1;
                r = new Rect(position.x, position.y + lineIndex * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(r, pipeline);

                lineIndex += 1;
                r = new Rect(position.x, position.y + lineIndex * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(r, lightingModel);

                lineIndex += 1;
                r = new Rect(position.x, position.y + lineIndex * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(r, texturingModel);

                if (texturingModel.enumValueIndex == (int)GTexturingModel.Splat)
                {
                    lineIndex += 1;
                    r = new Rect(position.x, position.y + lineIndex * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), position.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(r, splatsModel);
                }

                lineIndex += 1;
                r = new Rect(position.x, position.y + lineIndex * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(r, material);

                EditorGUI.indentLevel -= 1;
            }

            pipeline.Dispose();
            lightingModel.Dispose();
            texturingModel.Dispose();
            splatsModel.Dispose();
            material.Dispose();

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lineCount = 0;
            SerializedProperty texturingModel = property.FindPropertyRelative("texturingModel");
            if (texturingModel.enumValueIndex == (int)GTexturingModel.Splat)
            {
                lineCount = 5;
            }
            else
            {
                lineCount = 4;
            }
            texturingModel.Dispose();

            lineCount *= property.isExpanded ? 1 : 0;
            lineCount += property.depth > 0 ? 1 : 0;

            return lineCount * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }
    }
}
