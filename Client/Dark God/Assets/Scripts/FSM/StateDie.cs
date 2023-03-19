using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDie : IState
{
    public void OnEnter(EntityBase entity, params object[] args)
    {
        entity.CurrentAniState = AniState.Die;
    }

    public void OnExit(EntityBase entity, params object[] args)
    {

    }

    public void Process(EntityBase entity, params object[] args)
    {
        entity.SetDie();
    }
}
