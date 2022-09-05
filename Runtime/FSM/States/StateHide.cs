using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Flock Away from Enemies
public class StateHide : State
{
    [Header("Transitions")]
    [SerializeField]
    State onFearOver;
    [SerializeField]
    StateWin winState;
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
    float runAwayDistance = 20;
    [SerializeField]
    float obsAvoidWeight = 1;
    [SerializeField]
    float obsAvoidRange = 3;
    [SerializeField]
    float enemyAvoidTime = 2f;

    IEnumerable<UnitModel> enemies;

    public override void OnAwake()
    {
        enemies = model.GetEnemies().Where(e => !e.Dead);

        StartCoroutine(Cor());
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
        //Vector3 avg = Steering.Average(enemies.Select(u => u.transform.position));

        Vector3 avgAvoid = Vector3.zero;
        int c = 0;
        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(
                enemy.transform.position,
                transform.position);

            //float mult = Mathf.Pow(2, -dist) / dist;
            //if (enemies.Count() == 1)
            //    mult = 1;

            avgAvoid += Steering.Avoid(
                transform.position, enemy.transform.position,
                enemy.Velocity, enemyAvoidTime) * Mathf.Clamp01(-(dist/ runAwayDistance) + 1);
            c++;
        }
        avgAvoid /= c;

        Vector3 obsAvoid = Steering.AvoidObstacle(
            avgAvoid, transform.position, obsAvoidRange, obsAvoidWeight);
        
        Debug.DrawRay(transform.position, obsAvoid * 3, Color.red);
        control.MovementDir = obsAvoid.normalized;

        if (!enemies.Any())
        {
            Transition(winState);
            return;
        }
    }

    IEnumerator Cor()
    {
        yield return new WaitForSeconds(minFearTime);

        yield return new WaitUntil(
            () => model.HealthNormal > regroupHealthPercent);

        Transition(onFearOver);
    }
}
