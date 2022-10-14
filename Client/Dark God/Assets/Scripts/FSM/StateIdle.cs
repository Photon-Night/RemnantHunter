using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdle : IState
{
    public void OnEnter(EntityBase entity)
    {
        entity.CurrentState = AniState.Idle;
        PECommon.Log("Enter Idle");
        
    }

    public void OnExit(EntityBase entity)
    {
        
    }

    public void OnUpdate(EntityBase entity)
    {
        
    }

    public void Process(EntityBase entity)
    {
        entity.SetBlend(Message.BlendIdle);
    }
}
