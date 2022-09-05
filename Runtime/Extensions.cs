using System;
using System.Collections.Generic;
using UnityEngine;

static class Extensions
{


    public static Vector3 ProjectOnPlane(this Vector3 v, Vector3 upAxis)
    {
        return ProjectOnPlane(v, new Plane(upAxis, 0));
    }
    public static Vector3 ProjectOnPlane(this Vector3 v, Plane plane)
    {
        return plane.ClosestPointOnPlane(v).normalized * v.magnitude;
    }

    public static T GetMin<T>(this IEnumerable<T> col, Func<T, float> func)
    {
        float lowest = float.MaxValue;
        T best = default;
        foreach (var item in col)
        {
            float current = func(item);
            if (current < lowest)
            {
                lowest = current;
                best = item;
            }
        }
        return best;
    }
    public static Vector3 MultBy(this Vector3 a, Vector3 b) => new Vector3
    {
        x = a.x * b.x,
        y = a.y * b.y,
        z = a.z * b.z,
    };
}
