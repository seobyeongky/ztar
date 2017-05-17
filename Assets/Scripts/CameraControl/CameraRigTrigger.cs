using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraRigTrigger : MonoBehaviour
{
    CameraRig rig;

    void Awake()
    {
        rig = GetComponent<CameraRig>();
        if (CheckCamera())
        {
            enabled = false;
            rig.enabled = true;
        }
        Global.cameraRig = rig;
    }

    void Update()
    {
        if (CheckCamera())
        {
            enabled = false;
            rig.enabled = true;
        }
    }

    bool CheckCamera()
    {
        var mainCam = Camera.main;
        if (mainCam == null)
        {
            return false;
        }
        transform.position = mainCam.transform.position;
        rig.orgScene = mainCam.gameObject.scene;
        mainCam.transform.SetParent(rig.neck.T, false);
        mainCam.transform.localPosition = new Vector3(0, 0, -10);
        rig.mainCamera = mainCam;
        return true;
    }
}
