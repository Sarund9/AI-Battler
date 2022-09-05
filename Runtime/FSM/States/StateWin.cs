using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class StateWin : State
{

    [SerializeField]
    UnitModel model;
    [SerializeField]
    UnitController control;

    public override void OnAwake()
    {
        control.StopMove();
        model.Win();
    }
}
