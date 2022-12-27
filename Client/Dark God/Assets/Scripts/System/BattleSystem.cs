using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : SystemRoot<BattleSystem>
{
    public BattleWin battleWin;
    public BattleEndWin battleEndWin;
    public BattleManager bm;

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

    public void StopBattle(bool isWin, int restHP)
    {
        battleWin.SetWinState(false);
        GameRoot.Instance.RemoveAllHPUIItem();
    }

    public void SetBattleEndWinState(FBEndType endType, bool isActive = true)
    {
        battleEndWin.SetEndType(endType);
        battleEndWin.SetWinState(isActive);
    }
}

