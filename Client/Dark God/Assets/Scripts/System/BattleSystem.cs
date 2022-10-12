using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : SystemRoot<BattleSystem>
{
    public BattleWin battleWin;
    private BattleManager bm;

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
        bm = go.AddComponent<BattleManager>();
        OpenBattleWin();
        bm.InitManager(mapId);
    }

    public void OpenBattleWin()
    {
        battleWin.SetWinState();
    }

    public void SetMoveDir(Vector2 dir)
    {
        bm.SetMoveDir(dir);
    }

    public void ReqReleaseSkill(int index)
    {
        bm.ReqReleaseSkill(index);
    }
}

