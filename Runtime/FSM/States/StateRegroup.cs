using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//Find allied leader and Pathfind to him
public class StateRegroup : State
{
    [Header("Transitions")]
    [SerializeField]
    State hideState;
    [SerializeField]
    State onBackInRange;
    [SerializeField]
    StateWin winState;
    [SerializeField]
    StateAttack attackState;

    [Header("Components")]
    [SerializeField]
    UnitModel model;
    [SerializeField]
    UnitController control;

    [Header("Parameters")]
    [SerializeField]
    float obsAvoidWeight = 1;
    [SerializeField]
    float obsAvoidRange = 3;
    [SerializeField]
    float enemyAvoidTime = 2f;

    IEnumerable<UnitModel> enemies;
    UnitModel leader;

    public override void OnAwake()
    {
        enemies = model.GetEnemies().Where(e => !e.Dead);
        leader = model.GetLeader();
    }

    public override void OnUpdate()
    {

        if (model.HealthNormal < model.FleeHP)
        {
            Transition(hideState);
            return;
        }

        Vector3 avg = Steering.Pursuit(
            transform.position,
            leader.transform.position,
            leader.Velocity,
            2f);

        Vector3 dirBase = (avg - transform.position).normalized;

        Vector3 obsAvoid = Steering.AvoidObstacle(
            dirBase, transform.position,
            obsAvoidRange, obsAvoidWeight);

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
            Transition(attackState);
        }
    }
}