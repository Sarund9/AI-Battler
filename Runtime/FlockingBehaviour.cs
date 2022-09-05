using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Flocking Behaviour", menuName = "AI/Flocking Behaviour")]
public class FlockingBehaviour : ScriptableObject
{
    public float range;

    public float separationWeight;
    public float cohesionWeight;
    public float alignWeight;
    public float leaderWeight;

    public float obsAvoidWeight;
    public float obsAvoidDistance;

    private float TotalWeight => separationWeight + alignWeight + leaderWeight + cohesionWeight;

    public Vector3 GetDir(Transform unit, Transform leader, Transform[] targets)
    {
        Vector3 separation = Steering.Separate(unit.position, Steering.Average(targets.Select(d => d.position)), range) * (separationWeight / TotalWeight);
        Vector3 cohesion = Steering.Seek(unit.position, Steering.Average(targets.Select(p => p.position))) * (cohesionWeight / TotalWeight);
        Vector3 align = Steering.Average(targets.Select(d => d.forward)) * (alignWeight / TotalWeight);
        Vector3 lead = Steering.Seek(unit.position, leader.position) * (leaderWeight / TotalWeight);

        //Debug.Log($"GET DIR || SEP:{separation}///COH:{cohesion}///ALG:{align}///LED:{lead}");

        //Debug.DrawLine(unit.position, unit.position + separation.normalized * 10, Color.blue);
        //Debug.DrawLine(unit.position, unit.position + cohesion.normalized * 10, Color.green);
        //Debug.DrawLine(unit.position, unit.position + align.normalized * 10, Color.yellow);
        //Debug.DrawLine(unit.position, unit.position + lead.normalized * 10, Color.black);

        //Debug.DrawLine(unit.position, unit.position + (separation + cohesion + align + lead).normalized * 15, Color.red);

        return (separation + cohesion + align + lead).normalized;
    }
}
