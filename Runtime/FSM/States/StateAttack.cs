using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Attack closest Enemy
public class StateAttack : State
{
    [Header("Transitions")]
    [SerializeField]
    State onFinished;
    [SerializeField]
    StateHide hideState;
    [SerializeField]
    StateWin winState;
    [SerializeField]
    StateIdle idleState;

    [Header("Components")]
    [SerializeField]
    UnitModel model;
    [SerializeField]
    UnitController control;

    [Header("Debug")]
    [SerializeField]
    bool log;

    public override void OnAwake()
    {
        control.StopMove();
        FindAndAttackEnemy();
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
    private void FindAndAttackEnemy()
    {
        if (model.HealthNormal < model.FleeHP)
        {
            Transition(hideState);
            return;
        }

        var enemies = model.GetEnemies().Where(e => !e.Dead);
        UnitModel closest = null;
        float dist = float.MaxValue;
        foreach (var enemy in enemies)
        {
            float newDist = Vector3.Distance(enemy.transform.position, model.transform.position);
            if (newDist < dist
                && newDist < model.AttackRange)
            {
                dist = newDist;
                closest = enemy;
            }
        }
        //target = closest;
        if (closest == null)
        {
            if (log) print($"{model.name} NO CLOSEST ENEMY");
            Transition(onFinished);
        }
        else
        {
            if (log) print($"{model.name} ATTACK AT {closest.name}");
            StartCoroutine(TurnAndAttack(closest.transform.position));
        }
    }

    public override void OnUpdate()
    {
        

    }

    IEnumerator TurnAndAttack(Vector3 target)
    {
        control.Target = target;
        control.MovementDir = Vector3.zero;

        float time = 3f;
        while (!Cond() && time > 0)
        {
            //if (control.Target is null)
            //{
            //    print($"{model.name} Control was null, WTF");
            //    control.Target = target;
            //}
            time -= Time.deltaTime;
            yield return null;
        }

        if (log) print($"{model.name} - Attack now");

        control.Attack(target);
        StartCoroutine(Cor(control.AtkDuration));

        bool Cond()
        {
            Vector3 f = transform.forward;
            Vector3 t = (target - transform.position).normalized;
            if (log) print($"{model.name} - Turning to Attack A[{Vector3.Angle(f, t)}]");
            return Vector3.Angle(f, t) < 35;
        }
    }

    IEnumerator Cor(float time)
    {
        yield return new WaitForSeconds(time);
        FindAndAttackEnemy();
    }

}
