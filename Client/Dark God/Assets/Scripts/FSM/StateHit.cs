using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHit : IState
{
    public void OnEnter(EntityBase entity, params object[] args)
    {
        entity.CurrentAniState = AniState.Hit;

    }

    public void OnExit(EntityBase entity, params object[] args)
    {
        entity.canReleaseSkill = true;
        
    }

    public void OnUpdate(EntityBase entity, params object[] args)
    {
        
    }

    public void Process(EntityBase entity, params object[] args)
    {       
        entity.SetHit();       
    }

}
