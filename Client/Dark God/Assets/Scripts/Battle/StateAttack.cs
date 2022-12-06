using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAttack : IState
{
    public void OnEnter(EntityBase entity, params object[] args)
    {
        entity.CurrentState = AniState.Attack;
        entity.currentSkillCfg = ResService.Instance.GetSkillData((int)args[0]);
        PECommon.Log("Enter Attack");
    }

    public void OnExit(EntityBase entity, params object[] args)
    {
        entity.ExitCurrentAtk();
        entity.SetAction(Message.ActionNormal);
    }

    public void OnUpdate(EntityBase entity, params object[] args)
    {
        
    }

    public void Process(EntityBase entity, params object[] args)
    {
        entity.SkillAttack((int)args[0]);
    }
}
    

