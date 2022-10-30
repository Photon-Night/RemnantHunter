using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBase
{
    protected AniState currentState = AniState.None;
    public StateManager stateMgr;
    public EntityController controller;
    public SkillManager skillMgr;
    public BattleManager battleMgr;
    public bool LockCtrl
    {
        get;
        set;
    }
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

    public virtual void SetFX(string name, float destroy)
    {
        if (controller != null)
            controller.SetFX(name, destroy);
    }

    public void SkillAttack(int skillId)
    {
        AttackEffect(skillId);
        AttackDamage(skillId);
    }

    public virtual void AttackEffect(int skillId)
    {
        skillMgr.AttackEffect(this, skillId);
    }

    public virtual void AttackDamage(int skillId)
    {
        skillMgr.AttackEffect(this, skillId);
    }

    public void SetSkillMoveState(bool skillMove, float moveSpeed = 0)
    {
        if(controller != null)
        {
            controller.SetSkillMoveState(skillMove, moveSpeed);
        }
    }

    public void Lock()
    {
        controller.LockCtrl = true;
    }

    public void UnLock()
    {
        controller.LockCtrl = false;
    }

    public virtual Vector2 GetInputDir()
    {
        return Vector2.zero;
    }

    public virtual Vector3 GetPos()
    {
        return controller.transform.position;
    }

    public virtual Transform GetTrans()
    {
        return controller.transform;
    }
}
