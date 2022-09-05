using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PathDebug
{

    public static void DrawPath(IEnumerable<Vector3> path, Vector3 offset, Color color, float duration)
    {
        Vector3[] nodes = path.ToArray();
        if (nodes.Length <= 1)
        {
            return;
        }

        Vector3 lastNode = nodes[0];
        for (int i = 1; i < nodes.Length; i++)
        {
            Debug.DrawLine(lastNode + offset, nodes[i] + offset, color, duration);
            lastNode = nodes[i];
        }
        Debug.Log($"Drawn a path {nodes.Length} long");
    }
    public static void DrawPath(IEnumerable<Vector3> path, Vector3 offset, Color color)
    {
        Vector3[] nodes = path.ToArray();
        if (nodes.Length <= 1)
        {
            return;
        }
        Vector3 lastNode = nodes[0];
        for (int i = 1; i < nodes.Length; i++)
        {
            Debug.DrawLine(lastNode + offset, nodes[i] + offset, color);
            lastNode = nodes[i];
        }
    }
    public static void DrawPath(IEnumerable<Vector3> path, Vector3 offset)
    {
        Vector3[] nodes = path.ToArray();
        if (nodes.Length <= 1)
        {
            return;
        }
        Vector3 lastNode = nodes[0];
        for (int i = 1; i < nodes.Length; i++)
        {
            Debug.DrawLine(lastNode + offset, nodes[i] + offset);
            lastNode = nodes[i];
        }
    }

    public static readonly Vector2Int[] dirChecks = new Vector2Int[4]
    {
        new Vector2Int(1,0),
        new Vector2Int(0,1),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0),
    };
}