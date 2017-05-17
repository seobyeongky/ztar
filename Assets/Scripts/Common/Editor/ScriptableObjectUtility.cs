﻿using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static T CreateAsset<T>(string pathToSave = null, bool noSelection = false) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        if (pathToSave == null)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            pathToSave = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
        }


        AssetDatabase.CreateAsset(asset, pathToSave);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        if (!noSelection)
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        return asset;
    }
}