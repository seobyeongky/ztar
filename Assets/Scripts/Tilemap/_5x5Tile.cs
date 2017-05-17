using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class _5x5Tile : TileBase
{
    [System.Serializable]
    public class SpriteSet
    {
        public List<Sprite> patterns = new List<Sprite>();
    }

    [SerializeField]
    public List<SpriteSet> spriteSets = new List<SpriteSet>();

    [SerializeField]
    public int height;

    public static Vector3Int lastRefreshedLoc;
    public static ITilemap lastRefreshedTileMap = null;

    public override void RefreshTile(Vector3Int location, ITilemap tileMap)
    {
        for (int yd = -1; yd <= 1; yd++)
        {
            for (int xd = -1; xd <= 1; xd++)
            {
                Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                if (TileValue(tileMap, position))
                    tileMap.RefreshTile(position);
            }
        }
        lastRefreshedLoc = location;
        lastRefreshedTileMap = tileMap;
    }

    public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
    {
        UpdateTile(location, tileMap, ref tileData);
    }

    private void UpdateTile(Vector3Int location, ITilemap tileMap, ref TileData tileData)
    {
        tileData.transform = Matrix4x4.identity;
        tileData.color = Color.white;

        int mask = TileValue(tileMap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
        mask += TileValue(tileMap, location + new Vector3Int(1, 1, 0)) ? 2 : 0;
        mask += TileValue(tileMap, location + new Vector3Int(1, 0, 0)) ? 4 : 0;
        mask += TileValue(tileMap, location + new Vector3Int(1, -1, 0)) ? 8 : 0;
        mask += TileValue(tileMap, location + new Vector3Int(0, -1, 0)) ? 16 : 0;
        mask += TileValue(tileMap, location + new Vector3Int(-1, -1, 0)) ? 32 : 0;
        mask += TileValue(tileMap, location + new Vector3Int(-1, 0, 0)) ? 64 : 0;
        mask += TileValue(tileMap, location + new Vector3Int(-1, 1, 0)) ? 128 : 0;

        int index = GetIndex((byte)mask);
        if (index >= 0 && index < spriteSets.Count && TileValue(tileMap, location))
        {
            var spriteSet = spriteSets[index];
            if (spriteSet.patterns.Count == 1)
            {
                tileData.sprite = spriteSet.patterns[0];
            }
            else if (spriteSet.patterns.Count > 1)
            {
                var patternContainer = tileMap.GetComponent<TilePatternContainer>();
                if (patternContainer == null)
                {
#if UNITY_EDITOR
                    var go = tileMap.GetComponent<Transform>().gameObject;
                    UnityEditor.Undo.RecordObject(go, "gonna add tile pattern container");
                    patternContainer = go.AddComponent<TilePatternContainer>();
                    UnityEditor.Undo.RegisterCreatedObjectUndo(patternContainer, "new pattern container");
#endif
                }
                var loc2d = new Vector2Int(location.x, location.y);
#if UNITY_EDITOR
                if (!patternContainer.IsSet(loc2d))
                {
                    UnityEditor.Undo.RecordObject(patternContainer, "set set ");
                    patternContainer.SetPattern(loc2d, (byte)UnityEngine.Random.Range(0, 256));
                }
#endif
                var pattern = patternContainer.GetPattern(loc2d);
                //                UnityEngine.Random.InitState(pattern);
                tileData.sprite = spriteSet.patterns[pattern % spriteSet.patterns.Count];
            }
            else
            {
                tileData.sprite = null;
            }
            tileData.transform = Matrix4x4.identity; //GetTransform((byte)mask);
            tileData.color = Color.white;
            tileData.flags = TileFlags.LockAll;
            tileData.colliderType = Tile.ColliderType.Sprite;
        }
    }

    private bool TileValue(ITilemap tileMap, Vector3Int position)
    {
        TileBase tile = tileMap.GetTile(position);

        if (tile != null && tile == this)
        {
            return true;
        }

        if (tile != null && tile.GetType() == typeof(_5x5Tile))
        {
            var _5x5tile = (_5x5Tile)tile;
            if (_5x5tile.height > height)
            {
                return true;
            }
        }

        if (tile != null && tile.GetType() == typeof(CliffTile))
        {
            var cliffTile = (CliffTile)tile;
            if (cliffTile.height > height)
            {
                return true;
            }
        }

        return false;
    }

    private int GetIndex(byte mask)
    {
        // 701
        // 6d2
        // 543
        var UP = ((1 << 0) & mask) > 0;
        var UP_RIGHT = ((1 << 1) & mask) > 0;
        var RIGHT = ((1 << 2) & mask) > 0;
        var DOWN_RIGHT = ((1 << 3) & mask) > 0;
        var DOWN = ((1 << 4) & mask) > 0;
        var DOWN_LEFT = ((1 << 5) & mask) > 0;
        var LEFT = ((1 << 6) & mask) > 0;
        var UP_LEFT = ((1 << 7) & mask) > 0;

        if (UP && UP_RIGHT && RIGHT && DOWN_RIGHT && DOWN && DOWN_LEFT && LEFT && UP_LEFT)
        {
            return 0;
        }
        else if (UP && RIGHT && DOWN_RIGHT && DOWN && DOWN_LEFT && LEFT && UP_LEFT)
        {
            return 3;
        }
        else if (UP && UP_RIGHT && RIGHT && DOWN_RIGHT && DOWN && LEFT && UP_LEFT)
        {
            return 9;
        }
        else if (UP && UP_RIGHT && RIGHT && DOWN && DOWN_LEFT && LEFT && UP_LEFT)
        {
            return 6;
        }
        else if (UP && UP_RIGHT && RIGHT && DOWN_RIGHT && DOWN && DOWN_LEFT && LEFT)
        {
            return 12;
        }
        else if (UP && RIGHT && DOWN && LEFT)
        {
            return 0;
        }
        else if (RIGHT && DOWN_RIGHT && DOWN && DOWN_LEFT && LEFT)
        {
            return 1;
        }
        else if (UP && DOWN && DOWN_LEFT && LEFT && UP_LEFT)
        {
            return 4;
        }
        else if (UP && UP_RIGHT && RIGHT && LEFT && UP_LEFT)
        {
            return 7;
        }
        else if (UP && UP_RIGHT && RIGHT && DOWN_RIGHT && DOWN)
        {
            return 10;
        }
        else if (DOWN && DOWN_LEFT && LEFT)
        {
            return 2;
        }
        else if (UP && LEFT && UP_LEFT)
        {
            return 5;
        }
        else if (UP && UP_RIGHT && RIGHT)
        {
            return 8;
        }
        else if (RIGHT && DOWN_RIGHT && DOWN)
        {
            return 11;
        }
        else
        {
            return 0;
        }
    }


#if UNITY_EDITOR
    public static void ResetSeedMap()
    {
        if (lastRefreshedTileMap == null)
        {
            return;
        }

        var patternContainer = lastRefreshedTileMap.GetComponent<TilePatternContainer>();
        if (patternContainer == null)
        {
            return;
        }
#if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(patternContainer, "gonna randomize");
#endif

        for (int yd = -1; yd <= 1; yd++)
        {
            for (int xd = -1; xd <= 1; xd++)
            {
                Vector2Int position = new Vector2Int(lastRefreshedLoc.x + xd, lastRefreshedLoc.y + yd);
                patternContainer.SetPattern(position, (byte)UnityEngine.Random.Range(0, 256));
            }
        }
    }
#endif
}
