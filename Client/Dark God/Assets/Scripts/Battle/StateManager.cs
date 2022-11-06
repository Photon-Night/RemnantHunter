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
        FSM.Add(AniState.Attack, new StateAttack());
        FSM.Add(AniState.Born, new StateBorn());
        FSM.Add(AniState.Die, new StateDie());
        FSM.Add(AniState.Hit, new StateHit());
    }

    public void ChangeState(EntityBase entity, AniState state, params object[] args)
    {
        if (entity.CurrentState == state) return;

        if(FSM.ContainsKey(state))
        {
            if(entity.CurrentState != AniState.None)
            FSM[entity.CurrentState].OnExit(entity, args);

            FSM[state].OnEnter(entity, args);
            FSM[state].Process(entity, args);
        }
    }
}
