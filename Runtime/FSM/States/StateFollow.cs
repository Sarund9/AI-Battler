using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Follow squad Flocking
public class StateFollow : State
{
    [Header("Transitions")]
    [SerializeField]
    StateAttack attackState;
    [SerializeField]
    StateWin winState;
    [SerializeField]
    StateHide hideState;
    [SerializeField]
    StateIdle idleState;

    [Header("Components")]
    [SerializeField]
    UnitModel model;
    [SerializeField]
    UnitController control;

    [Header("Parameters")]
    [SerializeField]
    float minFearTime = 5f;
    [SerializeField, Range(0.0f, 1.0f)]
    float regroupHealthPercent = 0.5f;

    [Header("Flocking")]
    [SerializeField]
    float separationRange;
    [SerializeField]
    float separationWeight;
    [SerializeField]
    float alignWeight;
    [SerializeField]
    float leaderWeight;
    [SerializeField]
    float cohesionWeight;

    [SerializeField]
    float obsAvoidWeight = 1;
    [SerializeField]
    float obsAvoidRange = 3;
    [SerializeField]
    float enemyAvoidTime = 2f;

    IEnumerable<UnitModel> enemies;
    IEnumerable<UnitModel> allies;
    UnitModel leader;

    private float TotalWeight => separationWeight + alignWeight + leaderWeight + cohesionWeight;

    public override void OnAwake()
    {
        enemies = model.GetEnemies().Where(e => !e.Dead);
        allies = model.GetAllies().Where(e => !e.Dead);
        leader = model.GetLeader();
    }

    private void Awake()
    {
        GameManager.OnGameOver += team =>
        {
            if (!this)
                return;
            //if (team is null)
            //{
            //    //TODO: Idle
            //    return;
            //}
            
            if (model.Team == team)
            {
                Transition(winState);
            }
            else
            {
                Transition(idleState);
            }
            StopAllCoroutines();
        };
    }

    public override void OnUpdate()
    {
        if (model.HealthNormal < model.FleeHP)
        {
            Transition(hideState);
            return;
        }

        Vector3 baseDir = GetDir(model.transform, leader.transform, allies.Select(a => a.transform));

        Debug.DrawRay(transform.position, baseDir, Color.white);

        Vector3 obsAvoid = Steering.AvoidObstacle(baseDir, transform.position, obsAvoidRange, obsAvoidWeight);
        control.MovementDir = obsAvoid.normalized;

        if (!enemies.Any())
        {
            Transition(winState);
            return;
        }
        float closest = enemies
            .Min(u => Vector3.Distance(transform.position, u.transform.position));

        if (closest <= model.AttackRange)
        {
            //print($"{model.name} MOVE TO ATTACK");
            Transition(attackState);
        }

    }


    Vector3 GetDir(Transform unit, Transform leader, IEnumerable<Transform> targets)
    {
        Vector3 separation = Steering.Separate(unit.position, Steering.Average(targets.Select(d => d.position)), separationRange) * (separationWeight / TotalWeight);
        Vector3 cohesion = Steering.Seek(unit.position, Steering.Average(targets.Select(p => p.position))) * (cohesionWeight / TotalWeight);
        Vector3 align = Steering.Average(targets.Select(d => d.forward)) * (alignWeight / TotalWeight);
        Vector3 lead = Steering.Seek(unit.position, leader.position) * (leaderWeight / TotalWeight);

        //Debug.Log($"GET DIR || SEP:{separation}///COH:{cohesion}///ALG:{align}///LED:{lead}");

        Debug.DrawLine(unit.position, unit.position + separation.normalized * 10, Color.blue);
        Debug.DrawLine(unit.position, unit.position + cohesion.normalized * 10, Color.red);
        Debug.DrawLine(unit.position, unit.position + align.normalized * 10, Color.yellow);
        Debug.DrawLine(unit.position, unit.position + lead.normalized * 10, Color.black);

        Debug.DrawLine(unit.position, unit.position + (separation + cohesion + align + lead).normalized * 15, Color.red);

        return (separation + cohesion + align + lead).normalized;
    }
}
