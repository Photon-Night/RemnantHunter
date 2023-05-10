using Game.FSM;
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
        FSM.Add(AniState.Chase, new StateChase());
        FSM.Add(AniState.Back, new StateBack());
    }

    public void ChangeState(EntityBase entity, AniState state, params object[] args)
    {
        if(FSM.ContainsKey(state))
        {
            if(entity.CurrentAniState != AniState.None && entity.CurrentAniState != state)
            FSM[entity.CurrentAniState].OnExit(entity, args);

            FSM[state].OnEnter(entity, args);
            FSM[state].Process(entity, args);
        }
    }
}
