using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Obstacle : MonoBehaviour
{
    public Collider _collider;
    private Bounds bounds;
    public float extends;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (_collider == null) {
            _collider = GetComponent<Collider>();
        }
        bounds = _collider.bounds;
        bounds.Expand(extends);
        print($"{gameObject} - {bounds}");
    }

    public bool WithinBounds(Vector3 point)
    {
        Init();

        if (_collider.bounds.Contains(point)) {
            
            return true;
        }
        return false;
    }
    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireCube(bounds.center, bounds.extents);
        //Vector3 p = _collider.bounds.max;
    }
}
