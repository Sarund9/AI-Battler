using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField]
    float senseRadius = 1.2f;
    [SerializeField]
    float detectRadius = 20f;
    [SerializeField, Range(1, 180)]
    int detectAngle = 80;

    [SerializeField]
    LayerMask playerLayer;
    [SerializeField]
    LayerMask layerMask;

    //Extra Features
    public List<Transform> tracking;

    private HashSet<Transform> _detectedLastFrame = new HashSet<Transform>();

    /// <summary>
    /// Called on the Update if a tracking transform is in Sight
    /// </summary>
    public event Action<Transform> OnDetectStay;
    /// <summary>
    /// Called when a tracking transform enters Line of Sight
    /// </summary>
    public event Action<Transform> OnDetectEnter;
    /// <summary>
    /// Called when a tracking transform exits Line of Sight
    /// </summary>
    public event Action<Transform> OnDetectExit;

    private void Update()
    {
        foreach (var t in tracking) {
            if (IsInSight(t.position)) {
                if (!_detectedLastFrame.Contains(t)) {
                    OnDetectEnter?.Invoke(t);
                    //print("Detect Enter");
                }
                OnDetectStay?.Invoke(t);
                _detectedLastFrame.Add(t);
                //print("Detect Stay");
            }
            else if (_detectedLastFrame.Contains(t))
            {
                _detectedLastFrame.Remove(t);
                OnDetectExit?.Invoke(t);
                //print("Detect Exit");
            }
        }
    }
    public bool IsInSight(Vector3 target)
    {
        Vector3 playerDir = (target - transform.position).normalized;
        //Check if player outside of Automatic Detection Range
        if (Vector3.Distance(target, transform.position) >= senseRadius)
        {
            //Check if player is in Range
            if (Vector3.Distance(target, transform.position) > detectRadius)
                return false;

            //Check if player is in ViewAngle
            if (Vector3.Angle(transform.forward, playerDir) > detectAngle / 2)
                return false;

        }
        //Check if there are obstacles in the way
        Debug.DrawRay(transform.position, playerDir * detectRadius, Color.blue);
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, playerDir, out hit, detectRadius))
            return false;
        if (hit.collider.gameObject.layer != 10)
            return false;

        return true;
    }

}
