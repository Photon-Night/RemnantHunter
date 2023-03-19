using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBorn : IState
{
    public void OnEnter(EntityBase entity, params object[] args)
    {
        entity.CurrentAniState = AniState.Born;
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
