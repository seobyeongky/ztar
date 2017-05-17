using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBone : MonoBehaviour
{
    [System.NonSerialized]
    public float viewSize = 0;

    [System.NonSerialized]
    public Transform T;

    void Awake()
    {
        T = GetComponent<Transform>();
    }
}