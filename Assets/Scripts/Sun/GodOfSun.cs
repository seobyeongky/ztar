using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GodOfSun : MonoBehaviour
{
    Light myLight;
    List<Sun> stars = new List<Sun>();

    private void Awake()
    {
        myLight = MakeDirectionalLight();
        myLight.transform.SetParent(transform, false);
    }

    public void NotifyRisen(Sun sun)
    {
        stars.Add(sun);
    }

    public void NotifyFallen(Sun sun)
    {
        stars.Remove(sun);
    }

    void OnlyDarkness()
    {
        if (myLight.enabled)
        {
            myLight.enabled = false;
        }
    }

    void OneSun(Sun sun)
    {
        if (!myLight.enabled)
        {
            myLight.enabled = true;
        }

        myLight.intensity = sun.intensity;
        myLight.color = sun.color;
        if (sun.T.hasChanged)
        {
            myLight.transform.rotation = sun.T.rotation;
            sun.T.hasChanged = false;
        }
    }

    void MultipleStars()
    {
        if (!myLight.enabled)
        {
            myLight.enabled = true;
        }

        // do nothing
    }

    public void MixBinaryStar(Sun a, Sun b, float t)
    {
        if (!myLight.enabled)
        {
            myLight.enabled = true;
        }

        var intensityA = a != null ? a.intensity : 0;
        var intensityB = b != null ? b.intensity : 0;
        var colorA = a != null ? a.color : Color.black;
        var colorB = b != null ? b.color : Color.black;

        myLight.intensity = Mathf.Lerp(intensityA, intensityB, t);
        myLight.color = Color.Lerp(colorA, colorB, t);

        if (a != null && b != null)
        {
            myLight.transform.rotation = Quaternion.Lerp(a.transform.rotation
                , b.transform.rotation, t);
        }
        else if (a != null)
        {
            myLight.transform.rotation = a.transform.rotation;
        }
        else if (b != null)
        {
            myLight.transform.rotation = b.transform.rotation;
        }
    }

    void LateUpdate()
    {
        if (stars.Count == 0)
        {
            OnlyDarkness();
        }
        else if (stars.Count == 1)
        {
            OneSun(stars[0]);
        }
        else
        {
            MultipleStars();
        }
    }

    public static Light MakeDirectionalLight()
    {
        var go = new GameObject("directional_light");
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            go.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
        }
#endif
        Light light = go.AddComponent<Light>();
        light.type = LightType.Directional;
        light.shadows = LightShadows.None;
        light.enabled = false;
        return light;
    }
}
