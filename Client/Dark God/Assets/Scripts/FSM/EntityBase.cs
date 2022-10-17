using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBase
{
    protected AniState currentState = AniState.None;
    public StateManager stateMgr;
    public EntityController controller;
    public SkillManager skillMgr;
    public AniState CurrentState
    {
        get
        {
            return currentState;
        }

        set
        {
            currentState = value;
        }
    }

   

    public void Move()
    {
        stateMgr.ChangeState(this, AniState.Move,null);
    }

    public void Idle()
    {
        stateMgr.ChangeState(this, AniState.Idle,null);
    }

    public void Attack(int skillId)
    {
        stateMgr.ChangeState(this, AniState.Attack, skillId);
    }

    public virtual void SetBlend(int blend)
    {
        if(controller != null)
        {
            controller.SetBlend(blend);
        }
    }

    public virtual void SetDir(Vector2 dir)
    {
        if (controller != null)
            controller.Dir = dir;
    }

    public virtual void SetAction(int action)
    {
        if (controller != null)
            controller.SetAction(action);
    }

    public virtual void AttackEffect(int skillId)
    {
        skillMgr.AttackEffect(this, skillId);
    }
}
