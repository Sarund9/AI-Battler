using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    public Type CurrentStateType => CurrentState.GetType();
    public State CurrentState { get; private set; }

    public FSM() { }
    public FSM(State initialState)
    {
        Transition(initialState);
    }

    public void OnUpdate()
    {
        CurrentState.OnUpdate();
    }
    
    public void Transition(State state)
    {
        if (state == null || state == CurrentState)
            return;

        CurrentState = state;
        CurrentState.OnAwake();
    }
    
    //public void Transition(string input)
    //{
    //    State newState = CurrentState.GetState(input);
    //    if (newState == null) 
    //        return;
    //    //CurrentState.Sleep();
    //    CurrentState = newState;
    //    CurrentState.OnAwake();
    //}
}
