using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[Serializable]
public class ColliderTile : TileBase
{
    public float size;
    public GameObject boxR;
    public GameObject boxD;
    public GameObject boxRD;
    public GameObject singleCircle;
    public GameObject boxRCircle;
    public GameObject boxDCircle;
    public GameObject boxRDCircle;

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
    }

    public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
    {
        UpdateTile(location, tileMap, ref tileData);
    }

    private void UpdateTile(Vector3Int location, ITilemap tileMap, ref TileData tileData)
    {
        tileData.transform = Matrix4x4.identity;
        tileData.color = Color.white;

        bool rightExists = TileValue(tileMap, location + new Vector3Int(1, 0, 0));
        bool downExists = TileValue(tileMap, location + new Vector3Int(0, -1, 0));
        bool upExists = TileValue(tileMap, location + new Vector3Int(0, 1, 0));
        bool leftExists = TileValue(tileMap, location + new Vector3Int(-1, 0, 0));
        bool needCircle = !upExists && !leftExists
            || !upExists && !rightExists
            || !rightExists && !downExists
            || !downExists && !leftExists;

        tileData.gameObject = GetGameObject(rightExists, downExists, needCircle);
        tileData.sprite = Resources.Load<Sprite>("collider_brush_preview");
        tileData.transform = Matrix4x4.identity; //GetTransform((byte)mask);
        tileData.color = Color.white;
        tileData.flags = TileFlags.LockAll;
        tileData.colliderType = Tile.ColliderType.None;
    }

    GameObject GetGameObject(bool R, bool D, bool circle)
    {
        if (R && D && circle)
        {
            return boxRDCircle;
        }
        else if (R && D)
        {
            return boxRD;
        }
        else if (R && circle)
        {
            return boxRCircle;
        }
        else if (D && circle)
        {
            return boxDCircle;
        }
        else if (circle)
        {
            return singleCircle;
        }
        else if (R)
        {
            return boxR;
        }
        else if (D)
        {
            return boxD;
        }

        return null;
    }

    private bool TileValue(ITilemap tileMap, Vector3Int position)
    {
        bool weAreTheOne = false;

        TileBase tile = tileMap.GetTile(position);

        if (tile != null && tile == this)
        {
            weAreTheOne = true;
        }

        return weAreTheOne;
    }

    // 7 0 1
    // 6 8 2
    // 5 4 3
    private int GetIndex(byte mask)
    {
        switch (mask)
        {
            case 85: return 8;

            case 5: return 5;
            case 20: return 7;
            case 80: return 1;
            case 65: return 3;

            case 21: return 6;
            case 84: return 0;
            case 81: return 2;
            case 69: return 4;
        }
        return 8;
    }
}