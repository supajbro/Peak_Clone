using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FillefranzTools;


namespace PropMaker {

    [CustomEditor(typeof(Ladder))]
    public class LadderEditor : Editor
    {
        static string[] tabs = new string[] { "Shape", "Rails", "Steps", "Mesh Info" };
        Ladder ladder;
        private void OnEnable()
        {
            ladder = target as Ladder;
            Undo.undoRedoPerformed += OnUndo;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndo;
        }

        void OnUndo()
        {
            ladder.UpdateProp();
        }
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            ladder.editorTab = GUILayout.Toolbar(ladder.editorTab, tabs);

            switch (ladder.editorTab)
            {
                case 0:
                    Shape();
                    break;
                case 1:
                    Rails();
                    break;
                case 2:
                    Steps();
                    break;
                case 3:
                    MeshInfo();
                    break;
            }




            GUILayout.Space(6);





            if (ladder.editMode == PropEditMode.GameObject)
            {
                if (GUILayout.Button("Edit Ladder"))
                {
                    ladder.editMode = PropEditMode.Points;
                    Tools.current = Tool.None;
                }
            }

            else if (ladder.editMode == PropEditMode.Points)
            {


                GUI.color = new Color(0, 0.88f, 0.9f);
                if (GUILayout.Button("Editing Ladder"))
                {
                    ladder.editMode = PropEditMode.GameObject;
                    Tools.current = Tool.Move;
                }


                GUI.color = Color.white;
                ladder.editOrientation = (EditOrientation)EditorGUILayout.EnumPopup("Edit Orientation", ladder.editOrientation);
                EditorGUILayout.Space(12);
            }

            GUI.color = Color.white;

            if (!ladder.autoUpdate && GUILayout.Button("Update Bridge"))
            {
                ladder.UpdateProp();
            }


            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (GUI.changed)
            {
                Undo.RecordObject(ladder, "Change Values");
                EditorUtility.SetDirty(this);

                if (ladder.autoUpdate) { }
                ladder.UpdateProp();
            }

            Undo.RecordObject(ladder, "Change Parameter");
        }

        void Shape()
        {
            GUILayout.Label("Shape", EditorStyles.boldLabel);
            ladder.width = EditorGUILayout.FloatField("Width", ladder.width);
            ladder.rotation = EditorGUILayout.Slider("Rotation", ladder.rotation, 0, 360);

            GUILayout.Space(6);
            GUILayout.Label("Points", EditorStyles.boldLabel);
            GUI.color = Color.yellow;
            ladder.start = EditorGUILayout.Vector3Field("Start Point", ladder.start);
            GUI.color = Color.cyan;
            ladder.end = EditorGUILayout.Vector3Field("End Point", ladder.end);

            GUI.color = Color.white;

            if (GUILayout.Button("Straighten"))
            {
                ladder.end = ladder.start.OverrideY(ladder.end.y);
                ladder.UpdateProp();
            }
        }

        void Rails()
        {
            GUILayout.Label("Rails", EditorStyles.boldLabel);
            ladder.railWidth = EditorGUILayout.FloatField("Rail Width", ladder.railWidth);
            ladder.railThickness = EditorGUILayout.FloatField("Rail Thickness", ladder.railThickness);
        }

        void Steps()
        {
            GUILayout.Label("Steps", EditorStyles.boldLabel);
            ladder.stepRadius = Mathf.Max(0, EditorGUILayout.FloatField("Radius", ladder.stepRadius));
            ladder.stepResolution = Mathf.Max(3, EditorGUILayout.IntField("Resolution", ladder.stepResolution));

            ladder.fixedStepAmount = EditorGUILayout.Toggle("Fixed Step Amount", ladder.fixedStepAmount);





            if (ladder.fixedStepAmount)
            {
                ladder.numberOfSteps = Mathf.Max(0, EditorGUILayout.IntField("Number of Steps", ladder.numberOfSteps));
                ladder.stepPadding = Mathf.Max(0, EditorGUILayout.FloatField("Padding", ladder.stepPadding));
            }

            else
            {
                ladder.stepSpacing = Mathf.Max(0.025f, EditorGUILayout.FloatField("Spacing", ladder.stepSpacing));
            }
        }

        private void MeshInfo()
        {
            GUILayout.Label("Mesh Info", EditorStyles.boldLabel);
            GUILayout.Label($"Vertex Count: {ladder.vertexCount}");
            GUILayout.Label($"Triangle Count {ladder.triangleCount}");

            ladder.meshName = EditorGUILayout.DelayedTextField("Mesh Name", ladder.meshName);

            if (GUILayout.Button("Save Mesh As Asset"))
            {
                ladder.UpdateProp();
                string itemPath = EditorUtility.SaveFilePanelInProject("Save Mesh", $"{ladder.meshName}", "asset", "");

                if (itemPath != string.Empty)
                {
                    AssetDatabase.CreateAsset(ladder.mesh, itemPath);
                }

            }
        }
        private void OnSceneGUI()
        {
            Event currentEvent = Event.current;

            if (!ladder.hasLoaded)
            {
                ladder.UpdateProp();
                ladder.hasLoaded = true;
            }

            if (ladder.editMode == PropEditMode.Points)
            {
                if (Tools.current != Tool.None)
                {
                    ladder.editMode = PropEditMode.GameObject;
                    ladder.seed = EditorApplication.timeSinceStartup.GetHashCode();
                    return;
                }



                Quaternion orientataion = Quaternion.identity;

                if (ladder.editOrientation == EditOrientation.Local)
                    orientataion = Quaternion.LookRotation(ladder.forward, ladder.up);
                else if (ladder.editOrientation == EditOrientation.LocalXZ)
                    orientataion = Quaternion.LookRotation((ladder.end.OverrideY(0) - ladder.start.OverrideY(0)).normalized, Vector3.up);


                Vector3 start = ladder.transform.InverseTransformPoint(Handles.PositionHandle(ladder.transform.TransformPoint(ladder.start), orientataion));
                Vector3 end = ladder.transform.InverseTransformPoint(Handles.PositionHandle(ladder.transform.TransformPoint(ladder.end), orientataion));

                float startDst = Vector3.Distance(start, ladder.start);
                float endDst = Vector3.Distance(end, ladder.end);

                if ((startDst > 0.1f || endDst > 0.1f) || (currentEvent.type == EventType.MouseUp && (start != ladder.start || end != ladder.end)))
                {

                    Undo.RecordObject(ladder, "Move Points");
                    ladder.start = start;
                    ladder.end = end;
                    if (ladder.autoUpdate)
                    {


                        ladder.UpdateProp();
                    }


                }






            }


        }
    }
}
