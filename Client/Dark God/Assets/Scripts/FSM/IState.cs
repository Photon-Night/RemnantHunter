using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void OnEnter(EntityBase entity, params object[] args);
    public void OnUpdate(EntityBase entity, params object[] args);
    public void OnExit(EntityBase entity, params object[] args);
    public void Process(EntityBase entity, params object[] args);
}

public class StateInfo
{
    public int SkillId
    {
        get;
        set;
    }
}

public enum AniState
{
    None,
    Move,
    Idle,
    Attack,
}

