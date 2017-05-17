using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;
using System.IO;

[CustomEditor(typeof(ColliderTile))]
public class ColliderTileEditor : Editor
{
    const float DEFAULT_SIZE = 0.32f;

    bool isSizeDirty = false;

    enum BoxMode
    {
        R, D, RD
    }

    [MenuItem("Assets/Create/Collider Tile")]
    public static void CreateTile()
    {
        var tile = ScriptableObjectUtility.CreateAsset<ColliderTile>();

        if (EditorSceneManager.loadedSceneCount == 0)
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        }
        var gos = new List<GameObject>();

        tile.boxR = NewGameObject(tile.name + "_box_r", gos, tile);
        tile.boxD = NewGameObject(tile.name + "_box_d", gos, tile);
        tile.boxRD = NewGameObject(tile.name + "_box_rd", gos, tile);
        tile.singleCircle = NewGameObject(tile.name + "_single_circle", gos, tile);
        tile.boxRDCircle = NewGameObject(tile.name + "_box_rdcircle", gos, tile);
        tile.boxRCircle = NewGameObject(tile.name + "_box_rcircle", gos, tile);
        tile.boxDCircle = NewGameObject(tile.name + "_box_dcircle", gos, tile);
        SetTileSize(tile, DEFAULT_SIZE);
    }

    class Importer : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var path in importedAssets.Concat(movedAssets))
            {
                var tile = AssetDatabase.LoadAssetAtPath<ColliderTile>(path);
                // NOTE: should ignore fresh tile
                if (tile != null && tile.boxR != null)
                {
                    EnsurePrefabPaths(tile);
                    SetTileSize(tile, tile.size);
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        var sizeProp = serializedObject.FindProperty("size");
        EditorGUILayout.PropertyField(sizeProp);
        isSizeDirty |= EditorGUI.EndChangeCheck();
        if (isSizeDirty && GUILayout.Button("적용"))
        {
            AssetDatabase.SaveAssets();
        }

        serializedObject.ApplyModifiedProperties();
    }

    static void EnsurePrefabPath(GameObject prefab, string name, ColliderTile tile)
    {
        var tilePath = AssetDatabase.GetAssetPath(tile);
        var orgPath = AssetDatabase.GetAssetPath(prefab);
        var tileDir = Path.GetDirectoryName(tilePath);

        if (prefab.name != name
            || tileDir != Path.GetDirectoryName(orgPath))
        {
            var newPath = tileDir + "/" + tile.name + "_" + name + ".prefab";
            AssetDatabase.MoveAsset(orgPath, newPath);
        }
    }

    static void EnsurePrefabPaths(ColliderTile tile)
    {
        EnsurePrefabPath(tile.boxR, "box_r", tile);
        EnsurePrefabPath(tile.boxD, "box_d", tile);
        EnsurePrefabPath(tile.boxRD, "box_rd", tile);
        EnsurePrefabPath(tile.singleCircle, "single_circle", tile);
        EnsurePrefabPath(tile.boxRDCircle, "rdcircle", tile);
        EnsurePrefabPath(tile.boxRCircle, "rcircle", tile);
        EnsurePrefabPath(tile.boxDCircle, "dcircle", tile);
    }

    static void SetTileSize(ColliderTile tile, float newSize)
    {
        SetBox(tile.boxR, BoxMode.R, newSize);
        EditorUtility.SetDirty(tile.boxR);

        SetBox(tile.boxD, BoxMode.D, newSize);
        EditorUtility.SetDirty(tile.boxD);

        SetBox(tile.boxRD, BoxMode.RD, newSize);
        EditorUtility.SetDirty(tile.boxRD);

        SetCircle(tile.singleCircle, newSize);
        EditorUtility.SetDirty(tile.singleCircle);

        SetBox(tile.boxRDCircle, BoxMode.RD, newSize);
        SetCircle(tile.boxRDCircle, newSize);
        EditorUtility.SetDirty(tile.boxRDCircle);

        SetBox(tile.boxRCircle, BoxMode.R, newSize);
        SetCircle(tile.boxRCircle, newSize);
        EditorUtility.SetDirty(tile.boxRCircle);

        SetBox(tile.boxDCircle, BoxMode.D, newSize);
        SetCircle(tile.boxDCircle, newSize);
        EditorUtility.SetDirty(tile.boxDCircle);

        tile.size = newSize;
        EditorUtility.SetDirty(tile);
    }

    static GameObject NewGameObject(string name, List<GameObject> gos, ColliderTile tileObj)
    {
        var go = new GameObject(name);
        go.isStatic = true;
        go.layer = LayerMask.NameToLayer("Wall");
        gos.Add(go);
        var thePath = AssetDatabase.GetAssetPath(tileObj);
        var dir = Path.GetDirectoryName(thePath);
        var oldGo = go;
        go = PrefabUtility.CreatePrefab(dir + "/" + name + ".prefab", go, ReplacePrefabOptions.Default);
        DestroyImmediate(oldGo);
        //AssetDatabase.AddObjectToAsset(go, tileObj);
        return go;
    }

    static void SetBox(BoxCollider2D box
        , bool R
        , float size)
    {
        if (R)
        {
            box.offset = new Vector2(size, size / 2);
            box.size = new Vector2(size, size);
        }
        else // D
        {
            box.offset = new Vector2(size / 2, 0);
            box.size = new Vector2(size, size);
        }
    }

    static void SetBox(GameObject go
        , BoxMode mode
        , float size)
    {
        switch (mode)
        {
            case BoxMode.R:
            case BoxMode.D:
                var box = go.GetComponent<BoxCollider2D>();
                if (box == null)
                {
                    box = go.AddComponent<BoxCollider2D>();
                }
                SetBox(box, mode == BoxMode.R, size);
                break;
            case BoxMode.RD:
                var boxCount = go.GetComponents<BoxCollider2D>().Length;
                for (int i = 0; i < 2 - boxCount; i++)
                {
                    go.AddComponent<BoxCollider2D>();
                }
                var boxes = go.GetComponents<BoxCollider2D>();
                SetBox(boxes[0], true, size);
                SetBox(boxes[1], false, size);
                break;
        }
    }

    static void SetCircle(GameObject go, float size)
    {
        var circle = go.GetComponent<CircleCollider2D>();
        if (circle == null)
        {
            circle = go.AddComponent<CircleCollider2D>();
        }
        circle.radius = size / 2;
        circle.offset = new Vector2(size / 2, size / 2);
    }

    ColliderTile self { get { return (ColliderTile)target; } }
}
