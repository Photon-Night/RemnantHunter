using Game.Common;
using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleSystem : SystemRoot<BattleSystem>, IPlayerInputSet
{
    public BattleWin battleWin;
    public BattleEndWin battleEndWin;
    public BattleManager bm;
    private Transform camTrans;

    public System.Action<int> onTargetDie;

    private int fid = 0;
    private double startTime;
    public override void InitSystem()
    {
        base.InitSystem();
        PECommon.Log("BattleSystem Loading");
    }

    #region Battle Interface
    public void Move(float ver, float hor)
    {        
        bm.SetMove(ver, hor);
    }

    public void Attack()
    {
        bm.SetNormalAttack();
    }

    public void Jump()
    {
        bm.SetJump();
    }

    public void Sprint(bool isSprint)
    {
        bm.SetSprint(isSprint);
    }

    public void Combo()
    {
        bm.SetCombo();
    }

    public void Roll()
    {
        bm.SetRoll();
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

    #endregion

    #region Battle Setting
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
    #endregion

    #region NetSetting
    public void RspFBFightEnd(GameMsg msg)
    {
        RspFBFightEnd data = msg.rspFBFightEnd;
        GameRoot.Instance.SetPlayerDataByFightEnd(data);

        battleEndWin.SetBattleEndData(data.fbid, data.costtime, data.resthp);
        SetBattleEndWinState(FBEndType.Win);
    }

    
    #endregion
}

