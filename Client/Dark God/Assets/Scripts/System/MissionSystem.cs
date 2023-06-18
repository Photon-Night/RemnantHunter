using Game.Event;
using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSystem : SystemRoot<MissionSystem>
{
    public MissionWin missionWin;
   

    public override void InitSystem()
    {
        base.InitSystem();
        GameEventManager.SubscribeEvent<int>(EventNode.Event_OnOverTalk, OpenMissionWin_EventAction);
        PECommon.Log("MissionSystem Loading");
    }

    public void OpenMissionWin()
    {
        missionWin.SetWinState();
    }

    private void OpenMissionWin_EventAction(params int[] args)
    {
        int actionId = args[1];
        if(actionId == (int)NPCFunction.OpenMissionWin)
        {
            OpenMissionWin();
        }
    }
    public void RspMissionEnter(GameMsg msg)
    {
        RspMissionEnter data = msg.rspMissionEnter;
        GameRoot.Instance.SetPlayerDataByMissionEnter(data); 

        missionWin.SetWinState(false);
        MainCitySystem.Instance.CloseMainCityWin();
        BattleSystem.Instance.StartBattle(data.mid);
    }
}
