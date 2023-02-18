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
        
        PECommon.Log("MissionSystem Loading");
    }

    public void OpenMissionWin()
    {
        missionWin.SetWinState();
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
