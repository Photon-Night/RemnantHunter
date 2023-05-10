using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAttack : IState
{
    public void OnEnter(EntityBase entity, params object[] args)
    {
        entity.CurrentAniState = AniState.Attack;
    }

    public void OnExit(EntityBase entity, params object[] args)
    {
        entity.CurrentAniState = AniState.None;
    }

    public void Process(EntityBase entity, params object[] args)
    {
        entity.StopMove();
        entity.SetAttack();
    }
}
    

