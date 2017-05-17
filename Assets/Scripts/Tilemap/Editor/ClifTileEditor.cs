using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(CliffTile))]
public class CliffTileEditor : Editor
{
    const int SIZE = 48;
    const int PADDING = 8;

    Texture textureToImport;

    [MenuItem("Assets/Create/Cliff Tile")]
    public static void CreateTile()
    {
        ScriptableObjectUtility.CreateAsset<CliffTile>();
    }

    private CliffTile self { get { return (target as CliffTile); } }
    private int patternNumber = 0;

    public void OnEnable()
    {
        if (self.spriteSets == null || self.spriteSets.Count != 18)
        {
            self.spriteSets = new List<CliffTile.SpriteSet>();
            for (int i = 0; i < 18; i++)
            {
                self.spriteSets.Add(new CliffTile.SpriteSet());
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
        GUILayout.Label("격자에 맞게 스프라이트를 배치하시오");
        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
        { // row 1
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(SIZE + PADDING);
            SpriteFor(10);
            SpriteFor(0);
            SpriteFor(1);
            GUILayout.Space(SIZE + PADDING);
            EditorGUILayout.EndHorizontal();
        }

        { // row 2
            EditorGUILayout.BeginHorizontal();
            SpriteFor(10);
            SpriteFor(11);
            GUILayout.Space(SIZE + PADDING);
            SpriteFor(2);
            SpriteFor(1);
            EditorGUILayout.EndHorizontal();
        }

        { // row 3
            EditorGUILayout.BeginHorizontal();
            SpriteFor(9);
            GUILayout.Space(SIZE + PADDING);
            GUILayout.Space(SIZE + PADDING);
            GUILayout.Space(SIZE + PADDING);
            SpriteFor(3);
            EditorGUILayout.EndHorizontal();
        }

        { // row 4
            EditorGUILayout.BeginHorizontal();
            SpriteFor(7);
            SpriteFor(8);
            GUILayout.Space(SIZE + PADDING);
            SpriteFor(5);
            SpriteFor(4);
            EditorGUILayout.EndHorizontal();
        }

        { // row 5
            EditorGUILayout.BeginHorizontal();
            SpriteFor(12);
            SpriteFor(7);
            SpriteFor(6);
            SpriteFor(4);
            SpriteFor(14);
            EditorGUILayout.EndHorizontal();
        }

        { // row 6
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(SIZE + PADDING);
            SpriteFor(12);
            SpriteFor(13);
            SpriteFor(14);
            GUILayout.Space(SIZE + PADDING);
            SpriteFor(15);
            SpriteFor(16);
            SpriteFor(17);
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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("longCliff"));
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
    SpriteSelector MakeSpriteSelector(Sprite[] sprites)
    {
        return (postfix) =>
        {
            return sprites.FirstOrDefault(x => x.name.Substring(x.name.Length - postfix.Length) == postfix);
        };
    }

    void ApplySprites(Sprite[] sprites)
    {
        var sel = MakeSpriteSelector(sprites);
        self.spriteSets[0].patterns.Add(sel("_cb"));
        self.spriteSets[1].patterns.Add(sel("_cc"));
        self.spriteSets[2].patterns.Add(sel("_cj"));
        self.spriteSets[3].patterns.Add(sel("_ce"));
        self.spriteSets[4].patterns.Add(sel("_ch"));
        self.spriteSets[5].patterns.Add(sel("_cl"));
        self.spriteSets[6].patterns.Add(sel("_cg"));
        self.spriteSets[7].patterns.Add(sel("_cf"));
        self.spriteSets[8].patterns.Add(sel("_ck"));
        self.spriteSets[9].patterns.Add(sel("_cd"));
        self.spriteSets[10].patterns.Add(sel("_ca"));
        self.spriteSets[11].patterns.Add(sel("_ci"));
        self.spriteSets[12].patterns.Add(sel("_cm"));
        self.spriteSets[13].patterns.Add(sel("_cn"));
        self.spriteSets[14].patterns.Add(sel("_co"));
        self.spriteSets[15].patterns.Add(sel("_cp"));
        self.spriteSets[16].patterns.Add(sel("_cq"));
        self.spriteSets[17].patterns.Add(sel("_cr"));
    }
}