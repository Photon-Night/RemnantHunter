using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMove : IState
{
    public void OnEnter(EntityBase entity)
    {
        entity.CurrentState = AniState.Move;
        PECommon.Log("Enter Move");
    }

    public void OnExit(EntityBase entity)
    {
        
    }

    public void OnUpdate(EntityBase entity)
    {
        
    }
}
