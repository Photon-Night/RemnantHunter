using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBase
{
    protected AniState currentState = AniState.None;
    public StateManager stateMgr;
    public EntityController controller; 
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
        stateMgr.ChangeState(this, AniState.Move);
    }

    public void Idle()
    {
        stateMgr.ChangeState(this, AniState.Idle);
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
        controller.Dir = dir;
    }
}
