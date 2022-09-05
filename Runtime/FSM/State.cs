using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    //Dictionary<string, State> _dic = new Dictionary<string, State>();
    FSM fsm;

    public virtual void OnAwake() { }
    public virtual void OnUpdate() { }
    //public virtual void Sleep() { }

    //public void AddTransition(string input, State state)
    //{
    //    if (!_dic.ContainsKey(input))
    //        _dic.Add(input, state);
    //}
    //public void RemoveTransition(string input)
    //{
    //    if (_dic.ContainsKey(input))
    //        _dic.Remove(input);
    //}
    //public State GetState(string input)
    //{
    //    if (_dic.ContainsKey(input))
    //        return _dic[input];
    //    else return null;
    //}

    public void Init(FSM fsm)
    {
        this.fsm = fsm;
    }

    protected void Transition(State other)
    {
        fsm.Transition(other);
    }
}
