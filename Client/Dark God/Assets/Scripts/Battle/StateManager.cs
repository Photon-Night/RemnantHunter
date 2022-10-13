using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private Dictionary<AniState, IState> FSM = new Dictionary<AniState, IState>();

    public void InitManager()
    {
        PECommon.Log("StateManager Loading");

        FSM.Add(AniState.Idle, new StateIdle());
        FSM.Add(AniState.Move, new StateMove());
    }

    public void ChangeState(EntityBase entity, AniState state)
    {
        if (entity.CurrentState == state) return;

        if(FSM.ContainsKey(state))
        {
            if(entity.CurrentState != AniState.None)
            FSM[entity.CurrentState].OnExit(entity);

            FSM[state].OnEnter(entity);
            FSM[state].OnUpdate(entity);
        }
    }
}
