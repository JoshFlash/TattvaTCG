using System;
using System.Collections.Generic;
using UnityEngine;

public static class MainCamera
{
    private static Camera kMainCamera = null;
    public static Camera Get() => kMainCamera ? kMainCamera : (kMainCamera = Camera.main);

    private const int kMaxHits = 30;
    private static RaycastHit[] kHitResults = new RaycastHit[kMaxHits];

    public static ArraySegment<RaycastHit> ScreenCast()
    {
        return ScreenCast(-1);
    }

    public static ArraySegment<RaycastHit> ScreenCast(int layerMask, float radius = 0.01f, float distance = 100f)
    {
        Ray ray = Get().ScreenPointToRay(Input.mousePosition);

        int count = radius < Mathf.Epsilon 
            ? Physics.RaycastNonAlloc(ray, kHitResults, distance, layerMask) 
            : Physics.SphereCastNonAlloc(ray, 0.01f, kHitResults, 100f, layerMask);
        
        return new ArraySegment<RaycastHit>(kHitResults, 0, count);
    }

    public static Vector3 WorldToScreenPoint(this Vector3 position)
    {
        return Get().WorldToScreenPoint(position);
    }
}