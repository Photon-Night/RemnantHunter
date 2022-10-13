using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void OnEnter(EntityBase entity);
    public void OnUpdate(EntityBase entity);
    public void OnExit(EntityBase entity);
}

public enum AniState
{
    None,
    Move,
    Idle,
}

