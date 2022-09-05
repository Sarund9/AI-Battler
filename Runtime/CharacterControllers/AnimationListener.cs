using System;
using UnityEngine;

public class AnimationListener : MonoBehaviour
{
    public event Action OnAttackHit;
    public event Action OnFootstep;


    public void AttackHit() => OnAttackHit?.Invoke();
    public void Footstep() => OnFootstep?.Invoke();
}