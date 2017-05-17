using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ViewArea : MonoBehaviour
{
#if UNITY_EDITOR
    const float FOG_THICKNESS = 5f;
    const float FOG_DENSITY = 0.2f;
    const float FOG_DENSITY_RATE = 0.8f;
    const float EPSILON = 0.01f;

    RaycastHit2D[] hitsRightBuf = new RaycastHit2D[100];
    RaycastHit2D[] hitsLeftBuf = new RaycastHit2D[100];

    private void OnDrawGizmos()
    {
        var edge = GetComponentInChildren<EdgeCollider2D>();
        if (edge == null)
        {
            return;
        }

        var origin = edge.transform.position;


        for (int i = 0; i < edge.pointCount - 1; i++)
        {
            var point = origin.xy() + edge.points[i];
            var nextPoint = origin.xy() + edge.points[i + 1];

            var centerPoint = 0.5f * (point + nextPoint);
            var delta = nextPoint - point;
            var deltaDir = delta.normalized;
            var right = Vector3.Cross(deltaDir, new Vector3(0, 0, 1)).xy();

            Gizmos.color = Color.black;
            Gizmos.DrawLine(point, nextPoint);

            int rightHitsCount = Physics2D.RaycastNonAlloc(centerPoint + right * EPSILON
                , right
                , hitsRightBuf
                , Mathf.Infinity
                , LayerMask.GetMask("Camera"));

            int leftHitsCount = Physics2D.RaycastNonAlloc(centerPoint - right * EPSILON
                , -right
                , hitsLeftBuf
                , Mathf.Infinity
                , LayerMask.GetMask("Camera"));


            if (rightHitsCount % 2 == 0)
            {
                float alpha = 1f;
                for (float space = FOG_DENSITY; space < FOG_THICKNESS; space += FOG_DENSITY)
                {
                    alpha *= FOG_DENSITY_RATE;
                    Gizmos.color = new Color(0, 0, 0, alpha);
                    Gizmos.DrawLine(point + space * right, nextPoint + space * right);
                }
            }
            else if (leftHitsCount % 2 == 0)
            {
                float alpha = 1f;
                for (float space = FOG_DENSITY; space < FOG_THICKNESS; space += FOG_DENSITY)
                {
                    alpha *= FOG_DENSITY_RATE;
                    Gizmos.color = new Color(0, 0, 0, alpha);
                    Gizmos.DrawLine(point - space * right , nextPoint - space * right);
                }
            }
        }
    }
#endif
}
