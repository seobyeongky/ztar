using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    public float viewSize = 3;
    
    public float nearClipPlane = 0.1f;
    public float farClipPlane = 50f;
    static Color backColor = Color.black;

#if UNITY_EDITOR
    Camera _cam;
    Vector3[] _frustumPoints = new Vector3[4];

    public Camera cam
    {
        get
        {
            if (_cam == null)
            {
                _cam = CreateCam();
                _cam.transform.SetParent(transform, false);
                UpdateCam();
            }

            return _cam;
        }
    }

    static Camera CreateCam()
    {
        var obj = new GameObject("camcamcam");
        var cam = obj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = backColor;
        cam.enabled = false;
        obj.hideFlags = HideFlags.HideAndDontSave;
        return cam;
    }

    public void UpdateCam()
    {
        _cam.orthographicSize = viewSize;
        _cam.nearClipPlane = nearClipPlane;
        _cam.farClipPlane = farClipPlane;
    }

#if UNITY_EDITOR
    static System.Reflection.MethodInfo methodInfo = null;

    public static Vector2 GetMainGameViewSize()
    {
        if (methodInfo == null)
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            methodInfo = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        }
        System.Object Res = methodInfo.Invoke(null, null);
        return (Vector2)Res;
    }

#endif

    void UpdateFrustumPoints(Vector2 resol, float halfSize)
    {
        _frustumPoints[0] = cam.transform.position + new Vector3(-halfSize * resol.x / resol.y, -halfSize, 0);
        _frustumPoints[1] = cam.transform.position + new Vector3(-halfSize * resol.x / resol.y, +halfSize, 0);
        _frustumPoints[2] = cam.transform.position + new Vector3(+halfSize * resol.x / resol.y, +halfSize, 0);
        _frustumPoints[3] = cam.transform.position + new Vector3(+halfSize * resol.x / resol.y, -halfSize, 0);
    }

    void DrawFrustum(Color c)
    {
        var orgColor = Gizmos.color;
        Gizmos.color = c;
        Gizmos.DrawLine(_frustumPoints[0], _frustumPoints[1]);
        Gizmos.DrawLine(_frustumPoints[1], _frustumPoints[2]);
        Gizmos.DrawLine(_frustumPoints[2], _frustumPoints[3]);
        Gizmos.DrawLine(_frustumPoints[3], _frustumPoints[0]);
        Gizmos.color = orgColor;
    }

    void OnDrawGizmosSelected()
    {
        UpdateFrustumPoints(GetMainGameViewSize(), viewSize);
        DrawFrustum(new Color(1.0f, 0.1f, 0.1f));
    }

    void OnDrawGizmos()
    {
        UpdateFrustumPoints(GetMainGameViewSize(), viewSize);
        DrawFrustum(new Color(0.5f, 0.3f, 0.3f));
    }
#endif
}
