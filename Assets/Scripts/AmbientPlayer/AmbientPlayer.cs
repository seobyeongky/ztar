using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientPlayer : MonoBehaviour
{
    AudioSource src;

	void Awake ()
    {
        src = GetComponent<AudioSource>();

        Global.ambientPlayer = this;
    }

    public void Play(AudioClip clip)
    {
        src.clip = clip;
        src.Play();
    }
}
