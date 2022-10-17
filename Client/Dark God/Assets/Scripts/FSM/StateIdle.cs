using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdle : IState
{
    public void OnEnter(EntityBase entity, params object[] args)
    {
        entity.CurrentState = AniState.Idle;
        PECommon.Log("Enter Idle");
        
    }

    public void OnExit(EntityBase entity, params object[] args)
    {
        
    }

    public void OnUpdate(EntityBase entity, params object[] args)
    {
        
    }

    public void Process(EntityBase entity, params object[] args)
    {
        entity.SetBlend(Message.BlendIdle);
    }
}
