using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Sun : MonoBehaviour
{
    public float intensity = 1;
    public Color color = Color.white;
    public Transform T;

#if UNITY_EDITOR
    Light myLight;
#endif

    void Awake()
    {
        T = GetComponent<Transform>();
    }

    IEnumerator DelayedInit()
    {
        while (Global.helios == null)
        {
            yield return null;
        }

        Global.helios.NotifyRisen(this);
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false && gameObject.scene.name != "route_map")
        {
            Rehab();
            if (myLight == null)
            {
                myLight = GodOfSun.MakeDirectionalLight();
                myLight.enabled = true;
                myLight.transform.SetParent(transform, false);
            }
        }
        else
#endif
        {
            StartCoroutine(DelayedInit());
        }
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        {
            Global.helios.NotifyFallen(this);
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.isPlaying == false && myLight != null)
        {
            myLight.intensity = intensity;
            myLight.color = color;
        }
    }

    void Rehab()
    {
        myLight = GetComponentInChildren<Light>();
    }
#endif
}
