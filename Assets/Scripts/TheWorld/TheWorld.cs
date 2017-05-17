using UnityEngine;
using System.Collections;

public class TheWorld : MonoBehaviour
{
    public static bool isLoaded = false;

    void Awake ()
    {
        isLoaded = true;
    }

    private void OnDestroy()
    {
        isLoaded = false;
    }
}
