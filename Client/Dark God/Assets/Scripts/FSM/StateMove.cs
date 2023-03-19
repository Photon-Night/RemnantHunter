using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMove : IState
{
    public void OnEnter(EntityBase entity, params object[] args)
    {
        entity.CurrentAniState = AniState.Move;       
    }

    public void OnExit(EntityBase entity, params object[] args)
    {
        entity.CurrentAniState = AniState.None;
    }

    public void Process(EntityBase entity, params object[] args)
    {
        Vector3 pos = (Vector3)args[0];
        entity.MoveTo(pos);
    }
}
