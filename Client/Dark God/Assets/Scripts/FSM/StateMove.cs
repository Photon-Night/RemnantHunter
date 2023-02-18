using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMove : IState
{
    public void OnEnter(EntityBase entity, params object[] args)
    {
        entity.CurrentState = AniState.Move;
        
    }

    public void OnExit(EntityBase entity, params object[] args)
    {
        
    }

    public void OnUpdate(EntityBase entity, params object[] args)
    {
        
    }

    public void Process(EntityBase entity, params object[] args)
    {
        entity.SetBlend(Message.BlendWalk);
    }
}
