using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraView))]
public class CameraViewEditor : Editor
{
    [MenuItem("GameObject/Stage Object/Camera View", false, 'C' - 'A')]
    public static void CreateCameraView()
    {
        var obj = new GameObject("camera_view");
        obj.AddComponent<CameraView>();
        obj.transform.position = new Vector3(0, 0, -20);
        Undo.RegisterCreatedObjectUndo(obj, "create camera view");
        Selection.activeGameObject = obj;
    }

    CameraView self
    {
        get
        {
            return (CameraView)target;
        }
    }
    
    void OnEnable()
    {
        self.cam.enabled = true;
    }

    void OnDisable()
    {
        self.cam.enabled = false;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("viewSize"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("nearClipPlane"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("farClipPlane"));

        bool updateCamProps = EditorGUI.EndChangeCheck();

        serializedObject.ApplyModifiedProperties();

        if (updateCamProps)
        {
            self.UpdateCam();
        }
    }
}
