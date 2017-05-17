using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ViewArea))]
public class ViewAreaEditor : Editor
{
    EdgeCollider2D col_;
    int indexToFocus = -1;

    [MenuItem("GameObject/Stage Object/View Area", false, 'V' - 'A')]
    public static void CreateViewArea()
    {
        var go = new GameObject("view_area");
        var area = go.AddComponent<ViewArea>();
        Undo.RegisterCreatedObjectUndo(go, "created view area");
        Selection.activeGameObject = go;
    }

    protected virtual void OnSceneGUI()
    {
        for (int i = 0; i < col.pointCount; i++)
        {
            EditorGUI.BeginChangeCheck();
            var point = Handles.DoPositionHandle(col.points[i].xy0()
                + col.transform.position, Quaternion.identity);
            point -= col.transform.position;
            if (EditorGUI.EndChangeCheck())
            {
                var newPoints = new Vector2[col.pointCount];
                for (int j = 0; j < col.pointCount; j++)
                {
                    newPoints[j] = col.points[j];
                }
                newPoints[i] = point;
                Undo.RecordObject(col, "Change x y of edge collider");
                col.points = newPoints;
            }

            if (indexToFocus == i)
            {
                Handles.color = new Color(0.2f, 0.7f, 0.5f, 0.5f);
                Handles.DrawSolidDisc(point + col.transform.position
                    , Vector3.forward, 0.5f);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        int indexToRemove = -1;
        bool shouldRepaintSceneView = false;
        if (indexToFocus != -1) shouldRepaintSceneView = true;
        indexToFocus = -1;

        for (int i = 0; i < col.pointCount; i++)
        {
            var point = col.points[i];
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Point " + i, GUILayout.Width(120));
            EditorGUI.BeginChangeCheck();
            EditorGUIUtility.labelWidth = 25;
            var x = EditorGUILayout.FloatField("x", point.x, GUILayout.Width(100));
            GUILayout.Space(50);
            var y = EditorGUILayout.FloatField("y", point.y, GUILayout.Width(100));
            if (EditorGUI.EndChangeCheck())
            {
                var newPoints = new Vector2[col.pointCount];
                for (int j = 0; j < col.pointCount; j++)
                {
                    newPoints[j] = col.points[j];
                }
                newPoints[i] = new Vector2(x, y);
                Undo.RecordObject(col, "Change x y of edge collider");
                col.points = newPoints;
            }
            if (GUILayout.Button("-", GUILayout.Width(25)))
            {
                indexToRemove = i;
            }
            GUILayout.Space(50);
            EditorGUILayout.EndHorizontal();
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                indexToFocus = i;
                if (indexToFocus != i) shouldRepaintSceneView = true;
            }
        }

        if (indexToRemove >= 0)
        {
            var newPoints = new Vector2[col.pointCount - 1];
            for (int i = 0; i < col.pointCount - 1; i++)
            {
                newPoints[i] = col.points[i >= indexToRemove ? i + 1 : i];
            }
            Undo.RecordObject(col, "Change points of edge collider");
            col.points = newPoints;
        }

        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("+", GUILayout.Width(25)))
        {
            var newPoints = new Vector2[col.pointCount + 1];
            for (int i = 0; i < col.pointCount; i++)
            {
                newPoints[i] = col.points[i];
            }
            var addDir = Vector2.up;
            if (col.pointCount >= 2)
            {
                addDir = (col.points[col.pointCount - 1]
                    - col.points[col.pointCount - 2]).normalized;
            }
            newPoints[col.pointCount] = col.points[col.pointCount - 1] + addDir;
            Undo.RecordObject(col, "Add x y of edge collider");
            col.points = newPoints;
        }

        if (GUILayout.Button("complete edge", GUILayout.Width(200)))
        {
             var newPoints = new Vector2[col.pointCount + 1];
            for (int i = 0; i < col.pointCount; i++)
            {
                newPoints[i] = col.points[i];
            }
            newPoints[col.pointCount] = col.points[0];
            Undo.RecordObject(col, "Complete of edge collider");
            col.points = newPoints;
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        Repaint();
        if (shouldRepaintSceneView)
        {
            SceneView.RepaintAll();
        }
    }

    EdgeCollider2D col
    {
        get
        {
            if (col_ != null) return col_;

            col_ = self.GetComponentInChildren<EdgeCollider2D>();
            if (col_ == null)
            {
                var go = new GameObject("view_area_collider");
                //go.hideFlags = HideFlags.HideInHierarchy;
                go.transform.SetParent(self.transform, false);
                go.layer = LayerMask.NameToLayer("Camera");
                EditorSceneManager.MarkSceneDirty(go.scene);
                col_ = go.AddComponent<EdgeCollider2D>();
                col_.points = new Vector2[] {
                    new Vector2(-2, -2)
                    , new Vector2(2, -2)
                    , new Vector2(2, 2)
                    , new Vector2(-2, 2)
                    , new Vector2(-2, -2)
                };
            }

            return col_;
        }
    }

    ViewArea self { get { return (ViewArea)target; } }
}
