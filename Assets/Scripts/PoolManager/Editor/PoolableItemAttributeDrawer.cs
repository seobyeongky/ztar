using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.IO;

[CustomPropertyDrawer(typeof(PoolableItemAttribute))]
public class PoolableItemAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        var attr = attribute as PoolableItemAttribute;

        var opts = new List<string>();

        var info = new DirectoryInfo("Assets/Resources/Poolable");
        foreach (var file in info.GetFiles())
        {
            if (file.Extension == ".prefab")
            {
                var filename = Path.GetFileNameWithoutExtension(file.Name);
                if (Resources.Load("Poolable/" + filename, attr.objType)
                    != null)
                {
                    opts.Add(filename);
                }
            }
        }

        var labelRect = new Rect(position.x, position.y, 0.37f * position.width, position.height);
        var objRect = new Rect(position.x + labelRect.width + 5, position.y, position.width - labelRect.width - 5, position.height);
        EditorGUI.PrefixLabel(labelRect, label);
        var selIdx = opts.FindIndex(x => x == prop.stringValue);
        EditorGUI.BeginChangeCheck();
        selIdx = EditorGUI.Popup(objRect, selIdx, opts.ToArray());
        if (EditorGUI.EndChangeCheck())
        {
            var optSelected = opts[selIdx];
            prop.stringValue = optSelected;
        }
    }
}
