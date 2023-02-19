using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleSystem : SystemRoot<BattleSystem>
{
    public BattleWin battleWin;
    public BattleEndWin battleEndWin;
    public BattleManager bm;

    private int fid = 0;
    private double startTime;
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
        SetBattleWinState();
        bm.InitManager(mapId, () => 
        {
            startTime = timeSvc.GetCurrentTime();
        });

        fid = mapId;
    }

    public void SetBattleWinState(bool isActive = true)
    {
        battleWin.SetWinState(isActive);
    }

    public void SetMoveDir(Vector2 dir)
    {
        bm.SetMoveDir(dir);
    }

    public void ReqReleaseSkill(int index)
    {
        bm.ReqReleaseSkill(index);
    }

    public Vector2 GetInputDir()
    {
        return battleWin.GetCurrentDir();
    }

    public bool isPlayerAttack()
    {
        return bm.isPlayerAttack();
    }

    public void SetHPUI(int hp)
    {
        battleWin.SetHPUI(hp);
    }

    public void SetBossHPVal(int oldVal, int newVal, int sumVal)
    {
        battleWin.SetBossHPVal(oldVal, newVal, sumVal);
    }

    public void SetMonsterHPState(bool state, float prg = 1)
    {
        battleWin.SetMonsterHPState(state, prg);
    }

    public void EndBattle(bool isWin, int restHP)
    {

        battleWin.SetWinState(false);
        SetMonsterHPState(false);
        GameRoot.Instance.RemoveAllHPUIItem();

        if(isWin)
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqFBFightEnd,
                reqFBFightEnd = new ReqFBFightEnd
                {
                    win = isWin,
                    fbid = fid,
                    costtime = (int)((timeSvc.GetCurrentTime() - startTime)/1000),
                    resthp = restHP
                }
            };

            netSvc.SendMessage(msg);
        }
        else
        {
            SetBattleEndWinState(FBEndType.Lose);
        }
    }

    public void SetBattleEndWinState(FBEndType endType, bool isActive = true)
    {
        battleEndWin.SetEndType(endType);
        battleEndWin.SetWinState(isActive);
    }

    public void DestroyBattle()
    {
        SetBattleEndWinState(FBEndType.None, false);
        SetBattleWinState(false);
        GameRoot.Instance.RemoveAllHPUIItem();
        Destroy(bm.gameObject);
    }

    public void RspFBFightEnd(GameMsg msg)
    {
        RspFBFightEnd data = msg.rspFBFightEnd;
        GameRoot.Instance.SetPlayerDataByFightEnd(data);

        battleEndWin.SetBattleEndData(data.fbid, data.costtime, data.resthp);
        SetBattleEndWinState(FBEndType.Win);
    }

    public void SetRegiesterEventOnTargetDie(System.Action<int> action, bool isReg = true)
    {
        bm.SetRegiesterEventOnTargetDie(action, isReg);
    }
}

