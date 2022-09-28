using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseData<T>
{
    public int ID;
}

public class MapCfg : BaseData<MapCfg>
{
    public string mapName;
    public string sceneName;
    public Vector3 mainCamPos;
    public Vector3 mainCamRote;
    public Vector3 playerBornPos;
    public Vector3 playerBornRote;
}

public class GuideCfg : BaseData<GuideCfg>
{
    public int npcID;
    public string dilogArr;
    public int actID;
    public int coin;
    public int exp;
}

public class StrongCfg : BaseData<StrongCfg>
{
    public int pos;
    public int starLv;
    public int addHp;
    public int addHurt;
    public int addDef;
    public int minLv;
    public int coin;
    public int crystal;
}

public class TaskRewardData : BaseData<TaskRewardData>
{
    public int prgs;
    public bool taked;
}

public class TaskCfg : BaseData<TaskCfg>
{
    public string taskName;
    public int count;
    public int coin;
    public int exp;
}



