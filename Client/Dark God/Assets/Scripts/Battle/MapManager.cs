using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //private BattleManager battleMgr;
    //private int waveIndex = 1;
    //
    //public TriggerData[] TirggerArr;
    //private int triggerIndex = -1;
    //public void InitManager(BattleManager bm)
    //{
    //    PECommon.Log("MapManager Loading");
    //    battleMgr = bm;
    //    battleMgr.LoadMonsterByWaveID(waveIndex);
    //}
    //
    //public void TriggerMonsterBorn(TriggerData trigger, int waveIndex)
    //{
    //    if(battleMgr != null && trigger != null)
    //    {
    //        BoxCollider col = trigger.gameObject.GetComponent<BoxCollider>();
    //        col.isTrigger = false;
    //        battleMgr.LoadMonsterByWaveID(waveIndex);
    //        battleMgr.ActiveCurrentBatchMonsters();
    //    }
    //}
    //
    //public bool SetNextTrigger()
    //{
    //    waveIndex += 1;
    //    triggerIndex += 1;
    //    if (triggerIndex == TirggerArr.Length)
    //    {
    //        return true;
    //    }
    //    TirggerArr[triggerIndex].gameObject.GetComponent<BoxCollider>().isTrigger = true;
    //
    //    return false;
    //}
}
