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
    public int power;
    public Vector3 mainCamPos;
    public Vector3 mainCamRote;
    public Vector3 playerBornPos;
    public Vector3 playerBornRote;
    public List<MonsterData> monsterLst;
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

public class SkillCfg : BaseData<SkillCfg>
{
    public string skillName;
    public int skillTime;
    public int aniAction;
    public string fx;
    public List<int> skillMoveLst;
    public List<int> skillActionLst;
    public List<int> skillDamageLst;
}

public class SkillMoveCfg : BaseData<SkillCfg>
{
    public int moveTime;
    public float moveDis;
    public float delayTime;
}

public class MonsterCfg : BaseData<MonsterCfg>
{
    public string mName;
    public string resPath;
}

public class MonsterData : BaseData<MonsterData>
{
    public int mWave;
    public int mIndex;
    public MonsterCfg mCfg;
    public Vector3 mBornPos;
    public Vector3 mBornRote;
}

public class SkillActionCfg : BaseData<SkillActionCfg>
{
    public float delayTime; 
    public float radius;
    public float angle;
}







