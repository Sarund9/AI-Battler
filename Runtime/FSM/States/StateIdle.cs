using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class StateIdle : State
{
    [Header("Transitions")]
    [SerializeField]
    State onGameStart;

    [Header("Components")]
    [SerializeField]
    UnitModel model;
    [SerializeField]
    UnitController control;

    private void Awake()
    {
        GameManager.OnGameStart += () =>
        {
            if (!this)
                return;
            Transition(onGameStart);
        };
    }

    public override void OnAwake()
    {
        control.StopMove();
    }
}