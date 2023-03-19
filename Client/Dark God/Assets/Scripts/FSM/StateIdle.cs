using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdle : IState
{
    public void OnEnter(EntityBase entity, params object[] args)
    {
        entity.CurrentAniState = AniState.Idle;
    }
    public void OnExit(EntityBase entity, params object[] args)
    {
        entity.CurrentAniState = AniState.None;
    }
    public void Process(EntityBase entity, params object[] args)
    {
        entity.SetIdle();
        
    }
}
