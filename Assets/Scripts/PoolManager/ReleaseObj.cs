using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseObj : MonoBehaviour
{
    public float time;

    private void OnEnable()
    {
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(time);

        Global.poolManager.Release(this);
    }
}
