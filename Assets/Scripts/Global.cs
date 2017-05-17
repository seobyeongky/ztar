using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static Transform T;

    public static AudioSource bgm;
    public AudioSource _bgm;

    public static TheWorld theWorld;

    //public static EventDispatcher eventDispatcher;

    public static PoolManager poolManager;

    public static AmbientPlayer ambientPlayer;

    public static CameraRig cameraRig;

    public static GodOfSun helios;

    void Awake()
    {
        T = transform;
        bgm = _bgm;
        poolManager = GetComponent<PoolManager>();
        //eventDispatcher = GetComponent<EventDispatcher>();
        helios = GetComponent<GodOfSun>();
    }
}
