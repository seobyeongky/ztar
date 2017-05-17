using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraRig : MonoBehaviour
{
    public CameraBone foot;
    [System.NonSerialized]
    public Rigidbody2D rbody;
    public CameraBone body;
    public CameraBone neck;
    [System.NonSerialized]
    public BoxCollider2D box;
    public Camera mainCamera;
    public float restoringPower;
    [System.NonSerialized]
    public bool isContacting;
    [System.NonSerialized]
    public Scene orgScene;

    CameraRigTrigger trigger;
    ContactPoint2D[] contactPointBuf = new ContactPoint2D[50];

    public float totalViewSize
    {
        get
        {
            return foot.viewSize + body.viewSize + neck.viewSize;
        }
    }

    void Awake()
    {
        trigger = GetComponent<CameraRigTrigger>();
        box = body.GetComponent<BoxCollider2D>();
        rbody = foot.GetComponent<Rigidbody2D>();
        
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    void OnEnable()
    {
        foot.viewSize = mainCamera.orthographicSize;
        body.viewSize = 0;
        neck.viewSize = 0;

        var viewSize = mainCamera.orthographicSize;
        box.size = 2 * new Vector2(Screen.width * viewSize / Screen.height, viewSize);
    }

    private void SceneManager_sceneUnloaded(Scene scene)
    {
        if (scene.name == orgScene.name)
        {
            Destroy(mainCamera.gameObject);
            orgScene = new Scene();
            enabled = false;
            trigger.enabled = true;
        }
    }

    void LateUpdate()
    {
        if (mainCamera == null)
        {
            enabled = false;
            trigger.enabled = true;
        }

        var sumOf = totalViewSize;
        if (sumOf != mainCamera.orthographicSize)
        {
            mainCamera.orthographicSize = sumOf;
        }

        var boxSize = 2 * foot.viewSize;
        if (box.size.y != boxSize)
        {
            box.size = new Vector2(Screen.width * boxSize / Screen.height, boxSize);
        }
    }

    private void FixedUpdate()
    {
        //var localPos = neck.transform.localPosition;
        //var localPosSqrMagni = localPos.sqrMagnitude;
        isContacting = box.GetContacts(contactPointBuf) > 0;
        /*
        if (localPosSqrMagni > 0
            && isContacting)
        {
            var localPosMagni = Mathf.Sqrt(localPosSqrMagni);
            if (restoringPower * Time.fixedDeltaTime >= localPosMagni)
            {
                //rbody.MovePosition(neck.transform.parent.transform.position);
            }
            else
            {
                var delta = -restoringPower * localPos / localPosMagni;
                rbody.velocity = delta;
            }
        }
        */
    }
}
