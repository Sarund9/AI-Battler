using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//Flock to Enemy Squad Center
public class StateLead : State
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
    float obsAvoidWeight = 1;
    [SerializeField]
    float obsAvoidRange = 5;

    [Header("Debug")]
    [SerializeField]
    bool stop = false;

    IEnumerable<UnitModel> enemies;

    public override void OnAwake()
    {
        enemies = model.GetEnemies().Where(e => !e.Dead);
    }
    private void Awake()
    {
        GameManager.OnGameOver += team =>
        {
            if (!this)
                return;
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
        if (stop)
            return;

        if (model.HealthNormal < model.FleeHP)
        {
            Transition(hideState);
            return;
        }

        Vector3 avg = Steering.Average(enemies.Select(u => u.transform.position));
        Vector3 dirBase = (avg - transform.position).normalized;

        Vector3 obsAvoid = Steering.AvoidObstacle(dirBase, transform.position, obsAvoidRange, obsAvoidWeight);

        //Debug.DrawRay(transform.position, obsAvoid * 3, Color.red);
        control.MovementDir = obsAvoid.normalized;

        //var alive = enemies.Where(e => !e.Dead);

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
}
