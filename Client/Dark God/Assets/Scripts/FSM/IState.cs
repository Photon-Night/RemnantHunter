using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IState
{
    public void OnEnter(EntityBase entity, params object[] args);
    public void OnExit(EntityBase entity, params object[] args);
    public void Process(EntityBase entity, params object[] args);
}

public enum AniState
{
    None,
    Move,
    Idle,
    Attack,
    Born,
    Die,
    Hit,
    Patrol,
    Chase,
    Back,
}

