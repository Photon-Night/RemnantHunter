using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAttack : IState
{
    public void OnEnter(EntityBase entity, params object[] args)
    {
        entity.CurrentState = AniState.Attack;
        PECommon.Log("Enter Attack");
    }

    public void OnExit(EntityBase entity, params object[] args)
    {
       
    }

    public void OnUpdate(EntityBase entity, params object[] args)
    {
        
    }

    public void Process(EntityBase entity, params object[] args)
    {
        
    }
}
    

