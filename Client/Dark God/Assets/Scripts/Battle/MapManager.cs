using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private BattleManager battleMgr;
    private int waveIndex = 1;
    public void InitManager(BattleManager bm)
    {
        PECommon.Log("MapManager Loading");
        battleMgr = bm;
        battleMgr.LoadMonsterByWaveID(waveIndex);
    }
}
