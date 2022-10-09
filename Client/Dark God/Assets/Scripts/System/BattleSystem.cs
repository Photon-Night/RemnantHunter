using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : SystemRoot<BattleSystem>
{
    public override void InitSystem()
    {
        base.InitSystem();
        PECommon.Log("BattleSystem Loading");


    }

    public void StartBattle(int mapId)
    {
        GameObject go = new GameObject
        {
            name = "BattleRoot"
        };

        go.transform.SetParent(GameRoot.Instance.transform);
        BattleManager battleMgr = go.AddComponent<BattleManager>();
        battleMgr.InitManager(mapId);
    }
}

