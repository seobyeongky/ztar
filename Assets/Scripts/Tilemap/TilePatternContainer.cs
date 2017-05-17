using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePatternContainer : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    byte[] patternMap = new byte[0];

    [SerializeField]
    Vector2Int minski; // inclusive

    [SerializeField]
    Vector2Int maxski; // exclusive

    Vector2Int size
    {
        get
        {
            return maxski - minski;
        }
    }

    public void SetPattern(Vector2Int pos, byte pattern)
    {
        if (!IsCovered(pos))
        {
            if (minski == maxski)
            {
                ResetSize(pos, pos + new Vector2Int(1, 1));
            }
            else
            {
                var newMinski = new Vector2Int(
                    Mathf.Min(minski.x, pos.x)
                    , Mathf.Min(minski.y, pos.y));
                var newMaxski = new Vector2Int(
                    Mathf.Max(maxski.x, pos.x + 1)
                    , Mathf.Max(maxski.y, pos.y + 1)
                    );
                ResetSize(newMinski, newMaxski);
            }
        }

        patternMap[GetIndex(pos)] = (byte)(pattern + 1);
    }

    public byte GetPattern(Vector2Int pos)
    {
        if (IsCovered(pos))
        {
            return (byte)(patternMap[GetIndex(pos)] - 1);
        }
        else
        {
            return 0;
        }
    }

    int GetIndex(Vector2Int pos)
    {
        return size.x * (pos.y - minski.y) + (pos.x - minski.x);
    }

    public bool IsCovered(Vector2Int pos)
    {
        if (pos.x < minski.x || pos.y < minski.y
                    || maxski.x <= pos.x || maxski.y <= pos.y)
        {
            return false;
        }

        return true;
    }

    public bool IsSet(Vector2Int pos)
    {
        if (!IsCovered(pos))
        {
            return false;
        }

        if (patternMap[GetIndex(pos)] == 0)
        {
            return false;
        }

        return true;
    }

    public void ResetSize(Vector2Int newMinski, Vector2Int newMaxski)
    {
        var newSize = (newMaxski - newMinski);
        byte[]newPatternMap = new byte[newSize.x * newSize.y];
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                var pos = new Vector2Int(x + minski.x, y + minski.y);
                newPatternMap[newSize.x * (pos.y - newMinski.y) + (pos.x - newMinski.x)]
                    = patternMap[size.x * y + x];
            }
        }

        minski = newMinski;
        maxski = newMaxski;
        patternMap = newPatternMap;
    }
}
