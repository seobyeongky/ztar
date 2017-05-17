using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PoolableItem))]
public class PoolableItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var self = (PoolableItem)target;
        GUILayout.Label("id : " + self.id);

        if (Application.isPlaying == false)
        {
            self.id = -1;
            if (self.gameObject.scene.isLoaded == false)
            {
                EditorUtility.SetDirty(self);
            }
        }
    }
}
