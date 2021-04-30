using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Type = System.Type;

namespace Pinwheel.Griffin.SplineTool
{
    [CustomEditor(typeof(GSplineCreator))]
    public class GSplineCreatorInspector : Editor
    {
        private List<Type> modifierTypes;
        public List<Type> ModifierTypes
        {
            get
            {
                if (modifierTypes == null)
                {
                    modifierTypes = new List<Type>();
                }
                return modifierTypes;
            }
            set
            {
                modifierTypes = value;
            }
        }

        private GSplineCreator instance;
        private int selectedAnchorIndex = -1;
        private int selectedSegmentIndex = -1;

        private const int BEZIER_DIVISION = 20;
        private const float BEZIER_SELECT_DISTANCE = 10;
        private const float BEZIER_WIDTH = 5;

        private Rect addModifierButtonRect;

        private void OnEnable()
        {
            instance = (GSplineCreator)target;
            InitModifierClasses();
            instance.Editor_Vertices = instance.GenerateVerticesWithFalloff();
            Tools.hidden = true;
            SceneView.duringSceneGui += DuringSceneGUI;
        }

        private void OnDisable()
        {
            Tools.hidden = false;
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        private void InitModifierClasses()
        {
            List<Type> loadedTypes = GCommon.GetAllLoadedTypes();
            ModifierTypes = loadedTypes.FindAll(
                t => t.IsSubclassOf(typeof(GSplineModifier)));
        }

        public override bool RequiresConstantRepaint()
        {
            return false;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            instance.GroupId = GEditorCommon.ActiveTerrainGroupPopupWithAllOption("Group Id", instance.GroupId);
            DrawInstructionGUI();
            DrawAnchorDefaultValueGUI();
            DrawSelectedAnchorGUI();
            DrawSegmentDefaultValueGUI();
            DrawSelectedSegmentGUI();
            DrawGizmosGUI();
            DrawActionsGUI();
            GEditorCommon.DrawBackupHelpBox();
            //DrawDebugGUI();
            if (EditorGUI.EndChangeCheck())
            {
                instance.Editor_Vertices = instance.GenerateVerticesWithFalloff();
                GSplineCreator.MarkSplineChanged(instance);
                GUtilities.MarkCurrentSceneDirty();
            }
        }

        private void DrawInstructionGUI()
        {
            string label = "Instruction";
            string id = "instruction" + instance.GetInstanceID().ToString();
            GEditorCommon.Foldout(label, true, id, () =>
            {
                string s = string.Format(
                    "Create a edit bezier spline.\n" +
                    "   - Left Click to select element.\n" +
                    "   - Ctrl & Left Click to delete element.\n" +
                    "   - Shift & Left Click to add element.\n" +
                    "Use Add Modifier to do specific tasks with spline data.");
                EditorGUILayout.LabelField(s, GEditorCommon.WordWrapItalicLabel);
            });
        }

        private void DrawAnchorDefaultValueGUI()
        {
            string label = "Anchor Defaults";
            string id = "anchordefaults" + instance.GetInstanceID().ToString();
            GEditorCommon.Foldout(label, true, id, () =>
            {
                EditorGUIUtility.wideMode = true;
                instance.PositionOffset = EditorGUILayout.Vector3Field("Position Offset", instance.PositionOffset);
                instance.InitialRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Initial Rotation", instance.InitialRotation.eulerAngles));
                instance.InitialScale = EditorGUILayout.Vector3Field("Initial Scale", instance.InitialScale);
                EditorGUIUtility.wideMode = false;
            });
        }

        private void DrawSelectedAnchorGUI()
        {
            string label = "Selected Anchor";
            string id = "selectedanchor" + instance.GetInstanceID().ToString();
            GEditorCommon.Foldout(label, true, id, () =>
            {
                if (selectedAnchorIndex >= 0 && selectedAnchorIndex < instance.Spline.Anchors.Count)
                {
                    GSplineAnchor a = instance.Spline.Anchors[selectedAnchorIndex];
                    GSplineAnchorInspectorDrawer.Create(a).DrawGUI();
                }
                else
                {
                    EditorGUILayout.LabelField("No Anchor selected!", GEditorCommon.ItalicLabel);
                }
            });
        }

        private void DrawSegmentDefaultValueGUI()
        {
            string label = "Segment Defaults";
            string id = "segmentdefaults" + instance.GetInstanceID().ToString();
            GEditorCommon.Foldout(label, true, id, () =>
            {
                EditorGUIUtility.wideMode = true;
                instance.Smoothness = EditorGUILayout.IntField("Smoothness", instance.Smoothness);
                instance.Width = EditorGUILayout.FloatField("Width", instance.Width);
                instance.FalloffWidth = EditorGUILayout.FloatField("Falloff Width", instance.FalloffWidth);
                EditorGUIUtility.wideMode = false;
            });
        }

        private void DrawSelectedSegmentGUI()
        {
            string label = "Selected Segment";
            string id = "selectedsegment" + instance.GetInstanceID().ToString();
            GEditorCommon.Foldout(label, true, id, () =>
            {
                if (selectedSegmentIndex >= 0 && selectedSegmentIndex < instance.Spline.Segments.Count)
                {
                    GSplineSegment s = instance.Spline.Segments[selectedSegmentIndex];
                    GSplineSegmentInspectorDrawer.Create(s).DrawGUI();
                }
                else
                {
                    EditorGUILayout.LabelField("No Segment selected!", GEditorCommon.ItalicLabel);
                }
            });
        }

        private void DrawGizmosGUI()
        {
            string label = "Gizmos";
            string id = "gizmos" + instance.GetInstanceID().ToString();
            GEditorCommon.Foldout(label, true, id, () =>
            {
                GSplineToolSettings settings = GGriffinSettings.Instance.SplineToolSettings;
                settings.ShowMesh = EditorGUILayout.Toggle("Show Mesh", settings.ShowMesh);
                GGriffinSettings.Instance.SplineToolSettings = settings;
                EditorUtility.SetDirty(GGriffinSettings.Instance);
            });
        }

        private void DrawActionsGUI()
        {
            string label = "Modifiers";
            string id = "modifiers" + instance.GetInstanceID().ToString();
            GEditorCommon.Foldout(label, true, id, () =>
            {
                if (ModifierTypes.Count == 0)
                    return;
                Rect r = EditorGUILayout.GetControlRect();
                if (Event.current.type == EventType.Repaint)
                    addModifierButtonRect = r;
                if (GUI.Button(r, "Add Modifier"))
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < ModifierTypes.Count; ++i)
                    {
                        Type t = ModifierTypes[i];
                        string menuLabel = string.Empty;
                        object[] alternativeClassNameAttributes = t.GetCustomAttributes(typeof(GDisplayName), false);
                        if (alternativeClassNameAttributes != null && alternativeClassNameAttributes.Length > 0)
                        {
                            GDisplayName att = alternativeClassNameAttributes[0] as GDisplayName;
                            if (att.DisplayName == null ||
                                att.DisplayName.Equals(string.Empty))
                                menuLabel = t.Name;
                            else
                                menuLabel = att.DisplayName;
                        }
                        else
                        {
                            menuLabel = t.Name;
                        }

                        menu.AddItem(
                            new GUIContent(ObjectNames.NicifyVariableName(menuLabel)),
                            false,
                            () =>
                            {
                                AddModifier(t);
                            });
                    }
                    menu.DropDown(addModifierButtonRect);
                }
            });
        }

        private void AddModifier(Type t)
        {
            GSplineModifier modifier = instance.gameObject.AddComponent(t) as GSplineModifier;
            modifier.SplineCreator = instance;
        }

        private void DuringSceneGUI(SceneView sv)
        {
            EditorGUI.BeginChangeCheck();
            HandleSegmentModifications();
            HandleAnchorModifications();
            HandleAddRemoveElements();
            DrawGizmos();
            if (EditorGUI.EndChangeCheck())
            {
                instance.Editor_Vertices = instance.GenerateVerticesWithFalloff();
                GSplineCreator.MarkSplineChanged(instance);
                GUtilities.MarkCurrentSceneDirty();
            }
        }

        private void HandleAddRemoveElements()
        {
            if (instance.enabled == false)
                return;
            if (Event.current == null)
                return;

            int controlId = GUIUtility.GetControlID(this.GetHashCode(), FocusType.Passive);
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    GUIUtility.hotControl = controlId;
                    OnMouseDown(Event.current);
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                if (GUIUtility.hotControl == controlId)
                {
                    OnMouseUp(Event.current);
                    //Return the hot control back to Unity, use the default
                    GUIUtility.hotControl = 0;
                }
            }
            else if (Event.current.type == EventType.KeyDown)
            {
                OnKeyDown(Event.current);
            }
        }

        private void OnMouseDown(Event e)
        {
            if (GGuiEventUtilities.IsShift)
            {
                HandleAddAnchor();
            }
            else
            {
                selectedAnchorIndex = -1;
                selectedSegmentIndex = -1;
            }
        }

        private void HandleAddAnchor()
        {
            Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            if (GStylizedTerrain.Raycast(r, out hit, float.MaxValue, instance.GroupId))
            {
                instance.AddAnchorAutoTangent(hit.point, selectedAnchorIndex);
                selectedAnchorIndex = instance.Spline.Anchors.Count - 1;
                selectedSegmentIndex = -1;
                GUI.changed = true;
                Event.current.Use();
            }
        }

        private void OnMouseUp(Event e)
        {

        }

        private void OnKeyDown(Event e)
        {

        }

        private void HandleAnchorModifications()
        {
            List<GSplineAnchor> anchors = instance.Spline.Anchors;
            for (int i = 0; i < anchors.Count; ++i)
            {
                Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
                Handles.color = i == selectedAnchorIndex ?
                    GGriffinSettings.Instance.SplineToolSettings.SelectedElementColor :
                    GGriffinSettings.Instance.SplineToolSettings.AnchorColor;
                GSplineAnchor a = anchors[i];
                if (a == null)
                    continue;
                Vector3 groundPosition = new Vector3(a.Position.x, 0, a.Position.z);
                Handles.DrawDottedLine(
                    groundPosition,
                    a.Position, 5);
                if (GGuiEventUtilities.IsLeftMouseUp && GGuiEventUtilities.IsCtrl)
                {
                    float d0 = GHandleUtility.DistanceMouseToLine(Event.current.mousePosition, groundPosition, a.Position);
                    float d1 = GHandleUtility.DistanceMouseToPoint(Event.current.mousePosition, groundPosition);
                    float d2 = GHandleUtility.DistanceMouseToPoint(Event.current.mousePosition, a.Position);
                    if (d0 <= BEZIER_SELECT_DISTANCE &&
                        d1 > BEZIER_SELECT_DISTANCE &&
                        d2 > BEZIER_SELECT_DISTANCE)
                    {
                        SnapAnchorToSurface(a);
                        GUI.changed = true;
                    }
                }

                float handleSize = HandleUtility.GetHandleSize(a.Position) * 0.2f;
                if (Handles.Button(a.Position, Camera.current.transform.rotation, handleSize, handleSize * 0.5f, Handles.SphereHandleCap))
                {
                    if (GGuiEventUtilities.IsShift)
                    {
                        if (selectedAnchorIndex >= 0 && selectedAnchorIndex < anchors.Count)
                        {
                            instance.Spline.AddSegment(selectedAnchorIndex, i);
                            GUI.changed = true;
                        }
                        selectedAnchorIndex = i;
                        selectedSegmentIndex = -1;
                        Event.current.Use();
                    }
                    else if (GGuiEventUtilities.IsCtrl)
                    {
                        instance.Spline.RemoveAnchor(i);
                        selectedAnchorIndex = -1;
                        selectedSegmentIndex = -1;
                        GUI.changed = true;
                        Event.current.Use();
                    }
                    else
                    {
                        selectedAnchorIndex = i;
                        selectedSegmentIndex = -1;
                        Event.current.Use();
                    }
                }

                if (i == selectedAnchorIndex)
                {
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
                    if (Tools.current == Tool.Move)
                    {
                        a.Position = Handles.PositionHandle(a.Position, a.Rotation);
                    }
                    else if (Tools.current == Tool.Rotate)
                    {
                        a.Rotation = Handles.RotationHandle(a.Rotation, a.Position);
                    }
                    else if (Tools.current == Tool.Scale)
                    {
                        a.Scale = Handles.ScaleHandle(a.Scale, a.Position, a.Rotation, HandleUtility.GetHandleSize(a.Position));
                    }
                    instance.transform.position = instance.Spline.Anchors[i].Position;
                }
            }
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        }

        private void HandleSegmentModifications()
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            List<GSplineSegment> segments = instance.Spline.Segments;
            List<GSplineAnchor> anchors = instance.Spline.Anchors;
            for (int i = 0; i < segments.Count; ++i)
            {
                if (!instance.Spline.IsSegmentValid(i))
                    continue;
                if (i == selectedSegmentIndex)
                    HandleSelectedSegmentModifications();
                int i0 = segments[i].StartIndex;
                int i1 = segments[i].EndIndex;
                GSplineAnchor a0 = anchors[i0];
                GSplineAnchor a1 = anchors[i1];
                Vector3 startPosition = a0.Position;
                Vector3 endPosition = a1.Position;
                Vector3 startTangent = startPosition + segments[i].StartTangent;
                Vector3 endTangent = endPosition + segments[i].EndTangent;
                Color color = i == selectedSegmentIndex ?
                    GGriffinSettings.Instance.SplineToolSettings.SelectedElementColor :
                    GGriffinSettings.Instance.SplineToolSettings.SegmentColor;
                Handles.color = color;
                Vector3[] bezierPoints = Handles.MakeBezierPoints(startPosition, endPosition, startTangent, endTangent, instance.Smoothness);
                Handles.DrawAAPolyLine(BEZIER_WIDTH, bezierPoints);

                if (GGuiEventUtilities.IsLeftMouseUp)
                {
                    float d0 = GHandleUtility.DistanceMouseToSpline(Event.current.mousePosition, bezierPoints);
                    float d1 = GHandleUtility.DistanceMouseToPoint(Event.current.mousePosition, bezierPoints[0] - instance.PositionOffset);
                    float d2 = GHandleUtility.DistanceMouseToPoint(Event.current.mousePosition, bezierPoints[bezierPoints.Length - 1] - instance.PositionOffset);
                    if (d0 <= BEZIER_SELECT_DISTANCE &&
                        d1 > BEZIER_SELECT_DISTANCE &&
                        d2 > BEZIER_SELECT_DISTANCE)
                    {
                        selectedSegmentIndex = i;
                        if (GGuiEventUtilities.IsCtrl)
                        {
                            instance.Spline.Segments.RemoveAt(selectedSegmentIndex);
                            selectedSegmentIndex = -1;
                            GUI.changed = true;
                        }
                        //don't Use() the event here
                    }
                }
            }
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        }

        private void HandleSelectedSegmentModifications()
        {
            if (selectedSegmentIndex < 0 || selectedSegmentIndex >= instance.Spline.Segments.Count)
                return;
            if (!instance.Spline.IsSegmentValid(selectedSegmentIndex))
                return;
            GSplineSegment segment = instance.Spline.Segments[selectedSegmentIndex];
            GSplineAnchor startAnchor = instance.Spline.Anchors[segment.StartIndex];
            GSplineAnchor endAnchor = instance.Spline.Anchors[segment.EndIndex];
            Vector3 worldStartTangent = startAnchor.Position + segment.StartTangent;
            Vector3 worldEndTangent = endAnchor.Position + segment.EndTangent;

            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            worldStartTangent = Handles.PositionHandle(worldStartTangent, Quaternion.identity);
            worldEndTangent = Handles.PositionHandle(worldEndTangent, Quaternion.identity);

            segment.StartTangent = worldStartTangent - startAnchor.Position;
            segment.EndTangent = worldEndTangent - endAnchor.Position;

            Handles.color = Color.white;
            Handles.DrawLine(startAnchor.Position, worldStartTangent);
            Handles.DrawLine(endAnchor.Position, worldEndTangent);

            instance.transform.position = (startAnchor.Position + endAnchor.Position) * 0.5f;
        }

        private void DrawGizmos()
        {
            if (Event.current != null && Event.current.type == EventType.Repaint)
            {
                DrawMesh();
            }
        }

        private void DrawMesh()
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            if (GGriffinSettings.Instance.SplineToolSettings.ShowMesh)
            {
                Handles.color = GGriffinSettings.Instance.SplineToolSettings.MeshColor;
                int trisCount = instance.Editor_Vertices.Count / 3;
                for (int i = 0; i < trisCount; ++i)
                {
                    Vector4 p0 = instance.Editor_Vertices[i * 3 + 0];
                    Vector4 p1 = instance.Editor_Vertices[i * 3 + 1];
                    Vector4 p2 = instance.Editor_Vertices[i * 3 + 2];
                    if (p0.w > 0 && p1.w > 0 && p2.w > 0)
                    {
                        Handles.DrawPolyLine(p0, p1, p2, p0);
                    }
                    else
                    {
                        Handles.DrawDottedLines(new Vector3[]
                        {
                            p0, p1,
                            p1, p2,
                            p2, p0
                        }, 3);
                    }
                }
            }
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        }

        private Vector3 GetAnchorAvgPosition()
        {
            List<GSplineAnchor> anchors = instance.Spline.Anchors;
            if (anchors.Count == 0)
                return Vector3.zero;
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < anchors.Count; ++i)
            {
                sum += anchors[i].Position;
            }
            return sum / anchors.Count;
        }

        private void OffsetAnchors(Vector3 offset)
        {
            List<GSplineAnchor> anchors = instance.Spline.Anchors;
            for (int i = 0; i < anchors.Count; ++i)
            {
                anchors[i].Position += offset;
            }
        }

        private void SnapAnchorToSurface(GSplineAnchor a)
        {
            Ray r = new Ray(new Vector3(a.Position.x, 10000, a.Position.z), Vector3.down);
            RaycastHit hit;
            if (GStylizedTerrain.Raycast(r, out hit, float.MaxValue, instance.GroupId))
            {
                a.Position = hit.point;
            }
        }

        private void DrawDebugGUI()
        {
            string label = "Debug";
            string id = "debug" + instance.GetInstanceID().ToString();
            GEditorCommon.Foldout(label, true, id, () =>
            {
                EditorGUILayout.LabelField("Has Branch", instance.Spline.HasBranch ? "Yes" : "No");
            });
        }
    }
}
