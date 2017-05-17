using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Sun))]
public class SunEditor : Editor
{
    [MenuItem("GameObject/Stage Object/Sun", false, 'S' - 'A')]
    public static void MakeSun()
    {
        var obj = new GameObject("sun");
        obj.AddComponent<Sun>();
        Undo.RegisterCreatedObjectUndo(obj, "new sun");
        Selection.activeGameObject = obj;
    }
}
