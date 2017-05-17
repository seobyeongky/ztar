using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class PoolableItemAttribute : PropertyAttribute
{
    public Type objType;

    public PoolableItemAttribute(Type objType)
    {
        this.objType = objType;
    }
}
