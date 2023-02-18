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
 
    }

    public void OnExit(EntityBase entity, params object[] args)
    {
        entity.canReleaseSkill = true;
        entity.ExitCurrentAtk();
        entity.SetAction(Message.ActionNormal);
    }

    public void OnUpdate(EntityBase entity, params object[] args)
    {
        
    }

    public void Process(EntityBase entity, params object[] args)
    {
        entity.SkillAttack((int)args[0]);
        entity.canReleaseSkill = false;
    }
}
    

