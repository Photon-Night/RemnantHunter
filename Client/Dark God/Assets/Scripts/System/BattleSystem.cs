using Cinemachine;
using Game.Common;
using Game.Manager;
using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleSystem : SystemRoot<BattleSystem>, IPlayerInputSet
{
    public BattleWin battleWin;
    public BattleEndWin battleEndWin;
    public BattleManager bm;
    private CinemachineFreeLook freeLookCam;
    private CinemachineVirtualCamera battleEndCam;

    private int fid = 0;
    private double startTime;

    public static bool IsEnterBattle { get; private set; }

    public override void InitSystem()
    {
        base.InitSystem();
        PECommon.Log("BattleSystem Loading");
        IsEnterBattle = false;
    }
    #region Input Interface
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
    #endregion
    #region Battle Interface
    public bool isPlayerAttack()
    {
        return bm.isPlayerAttack();
    }

    public void SetHPUI(int hp)
    {
        battleWin.SetHPUI(hp);
    }

    public void SetPowerUI(float power)
    {
        battleWin.SetPowerUI(power);
    }

    public void SetBossHPVal(int oldVal, int newVal, int sumVal)
    {
        battleWin.SetBossHPVal(oldVal, newVal, sumVal);
    }
    public void SetMonsterHPState(bool state, float prg = 1)
    {
        battleWin.SetMonsterHPState(state, prg);
    }

    public EntityMonster FindClosedMonster(float distance = 0f)
    {
        return bm.FindClosedMonster(distance);
    }


    public void SkillAttack(EntityBase entity, SkillData skill, int dmgIndex)
    {
        bm.SkillAttack(entity, skill, dmgIndex);
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
        battleWin.SetWinState();
        bm.InitManager(mapId, () =>
        {
            startTime = timeSvc.GetCurrentTime();
            InputManager.Instance.SetPlayerInputRoot(this);
        });
        fid = mapId;

        IsEnterBattle = true;
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

        IsEnterBattle = false;
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

    public void SetCamLock(bool state)
    {
        throw new System.NotImplementedException();
    }

    public void SetInteraction()
    {
        throw new System.NotImplementedException();
    }


    #endregion

    #region Battle Win
    public void SetSkillCount(int count)
    {
        battleWin.SetSkillPointUI(count);
    }
    #endregion

    public void ChangePlayerEquipment(int itemID)
    {
        bm?.ChangePlayerEquipment(itemID);
    }
}

