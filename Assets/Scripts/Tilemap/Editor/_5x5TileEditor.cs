using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(_5x5Tile))]
public class _5x5TileEditor : Editor
{
    const int SIZE = 64;
    const int PADDING = 8;

    Texture textureToImport;

    [MenuItem("Assets/Create/5x5 Tile")]
    public static void CreateTile()
    {
        ScriptableObjectUtility.CreateAsset<_5x5Tile>();
    }

    private _5x5Tile self { get { return (target as _5x5Tile); } }
    private int patternNumber = 0;

    public void OnEnable()
    {
        if (self.spriteSets == null || self.spriteSets.Count != 13)
        {
            self.spriteSets = new List<_5x5Tile.SpriteSet>();
            for (int i = 0; i < 13; i++)
            {
                self.spriteSets.Add(new _5x5Tile.SpriteSet());
            }
            EditorUtility.SetDirty(self);
        }
    }

    void SpriteFor(int idx)
    {
        var patterns = self.spriteSets[idx].patterns;
        if (patternNumber > patterns.Count)
        {
            GUILayout.Space(SIZE + PADDING);
//            GUILayout.Box(GUIContent.none, GUILayout.Width(SIZE), GUILayout.Height(SIZE));
        }
        else
        {
            var previewSprite = patterns.Count == patternNumber ? null : patterns[patternNumber];
            EditorGUI.BeginChangeCheck();
            var inputSprite = (Sprite)EditorGUILayout.ObjectField(previewSprite, typeof(Sprite), false, GUILayout.Width(SIZE), GUILayout.Height(SIZE));
            if (EditorGUI.EndChangeCheck())
            {
                if (inputSprite == null)
                {
                    patterns[patternNumber] = null;
                    patterns.RemoveAll(x => x == null);
                }
                else
                {
                    if (patterns.Count == patternNumber)
                    {
                        patterns.Add(inputSprite);
                    }
                    else
                    {
                        patterns[patternNumber] = inputSprite;
                    }
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Label("5 x 5 격자에 맞게 스프라이트를 배치하시오");
        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
        { // row 1
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(SIZE + PADDING);
            SpriteFor(11);
            SpriteFor(1);
            SpriteFor(2);
            GUILayout.Space(SIZE + PADDING);
            EditorGUILayout.EndHorizontal();
        }

        { // row 2
            EditorGUILayout.BeginHorizontal();
            SpriteFor(11);
            SpriteFor(12);
            SpriteFor(0);
            SpriteFor(3);
            SpriteFor(2);
            EditorGUILayout.EndHorizontal();
        }

        { // row 3
            EditorGUILayout.BeginHorizontal();
            SpriteFor(10);
            SpriteFor(0);
            SpriteFor(0);
            SpriteFor(0);
            SpriteFor(4);
            EditorGUILayout.EndHorizontal();
        }

        { // row 4
            EditorGUILayout.BeginHorizontal();
            SpriteFor(8);
            SpriteFor(9);
            SpriteFor(0);
            SpriteFor(6);
            SpriteFor(5);
            EditorGUILayout.EndHorizontal();
        }

        { // row 5
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(SIZE + PADDING);
            SpriteFor(8);
            SpriteFor(7);
            SpriteFor(5);
            GUILayout.Space(SIZE + PADDING);
            EditorGUILayout.EndHorizontal();
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(self);
        }

        GUILayout.BeginHorizontal();
        if (patternNumber > 0 && GUILayout.Button("◀", GUILayout.Width(25)))
        {
            patternNumber--;
        }
        GUILayout.Label("패턴 : " + patternNumber);
        if (GUILayout.Button("▶", GUILayout.Width(25)))
        {
            patternNumber++;
        }
        GUILayout.EndHorizontal();

        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("height"));
        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(15);
        if (GUILayout.Button("Reset", GUILayout.Width(90)))
        {
            foreach (var spriteSet in self.spriteSets)
            {
                spriteSet.patterns.Clear();
            }
            EditorUtility.SetDirty(self);
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Import from", GUILayout.Width(90)))
        {
            if (textureToImport == null)
            {
                EditorUtility.DisplayDialog("...?", "no texture to import", "OK");
            }
            else
            {
                var sprites =
                    AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(textureToImport))
                    .Where(x => x.GetType() == typeof(Sprite))
                    .Select(x => (Sprite)x)
                    .ToArray();

                ApplySprites(sprites);
                EditorUtility.SetDirty(self);
            }
        }
        textureToImport = (Texture)EditorGUILayout.ObjectField(textureToImport, typeof(Texture), false);
        GUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    delegate Sprite SpriteSelector(string postfix);
    SpriteSelector MakeSpriteSelector(Sprite [] sprites)
    {
        return (postfix) =>
        {
            return sprites.FirstOrDefault(x => x.name.Substring(x.name.Length - postfix.Length) == postfix);
        };
    }

    void ApplySprites(Sprite [] sprites)
    {
        var sel = MakeSpriteSelector(sprites);
        self.spriteSets[0].patterns.Add(sel("_e"));
        self.spriteSets[1].patterns.Add(sel("_b"));
        self.spriteSets[2].patterns.Add(sel("_c"));
        self.spriteSets[3].patterns.Add(sel("_k"));
        self.spriteSets[4].patterns.Add(sel("_f"));
        self.spriteSets[5].patterns.Add(sel("_i"));
        self.spriteSets[6].patterns.Add(sel("_m"));
        self.spriteSets[7].patterns.Add(sel("_h"));
        self.spriteSets[8].patterns.Add(sel("_g"));
        self.spriteSets[9].patterns.Add(sel("_l"));
        self.spriteSets[10].patterns.Add(sel("_d"));
        self.spriteSets[11].patterns.Add(sel("_a"));
        self.spriteSets[12].patterns.Add(sel("_j"));
    }
}