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
}
