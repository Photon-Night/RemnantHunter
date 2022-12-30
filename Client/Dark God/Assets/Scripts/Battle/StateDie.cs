using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDie : IState
{
    public void OnEnter(EntityBase entity, params object[] args)
    {
        entity.CurrentState = AniState.Die;
        
        entity.RemoveSkillCB();
    }

    public void OnExit(EntityBase entity, params object[] args)
    {
        
    }

    public void OnUpdate(EntityBase entity, params object[] args)
    {
        
    }

    public void Process(EntityBase entity, params object[] args)
    {
        if (entity.entityType == Message.EntityType.Monster)
            entity.CloseCollider();
        entity.SetAction(Message.ActionDie);
        TimerService.Instance.AddTimeTask((int tid) =>
        {
            entity.SetActive(false);
        }, Message.DieAniLength);
    }
}
