using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseData
{
    public int ID;
}

public class MapCfg : BaseData
{
    public string mapName;
    public string sceneName;
    public int power;
    public Vector3 mainCamPos;
    public Vector3 mainCamRote;
    public Vector3 playerBornPos;
    public Vector3 playerBornRote;
    public List<MonsterData> monsterLst;
    public List<GroupData> monsterGroups;
    public List<int> npcs;
    public int exp;
    public int coin;
    public int crystal;
}

public class GuideCfg : BaseData
{ 
    public int npcID;
    public string dilogArr;
    public int actID;
    public int coin;
    public int exp;
}

public class TalkCfg : BaseData
{
    public int index;
    public int entityID;
    public TalkType type;
    public int[] selectLst;
    public int nextTalkID;
    public NPCFunction actID;
    public string[] dialogArr;
    public int nextIndex;
    public bool isRoot;
}


public class StrongCfg : BaseData
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

public class TaskRewardData : BaseData
{
    public int prgs;
    public bool taked;
}

public class TaskCfg : BaseData
{
    public string taskName;
    public int count;
    public int coin;
    public int exp;
}

public class SkillCfg : BaseData
{
    public string skillName;
    public int skillTime;
    public int aniAction;
    public string fx;
    public bool isCombo;
    public int nextComboID;
    public int transitionTime;
    public bool isCollide;
    public bool isBreak;
    public DmgType dmgType;
    public int cdTime;
    public List<int> skillMoveLst;
    public List<int> skillActionLst;
    public List<int> skillDamageLst;
}

public class SkillData : BaseData
{
    public string skillName;
    public bool isCollide;
    public bool isBreak;
    public DmgType type;
    public List<int> skillDamageLst;
}

public class SkillMoveCfg : BaseData
{
    public int moveTime;
    public float moveDis;
    public float delayTime;
}

public class MonsterCfg : BaseData
{
    public string mName;
    public Message.MonsterType mType;
    public bool isStop;
    public string resPath;
    public int skillID;
    public BattleProps bps;
}

public class MonsterData : BaseData
{
    public int lv;
    public int mWave;
    public int mIndex;
    public MonsterCfg mCfg;
    public Vector3 mBornPos;
    public Vector3 mBornRote;
}

public class NPCCfg : BaseData
{
    public string name;
    public string resPath;
    public Vector3 pos;
    public Vector3 rote;
    public NPCType type;
    public NPCFunction func;
}

public class GroupData : BaseData
{
    public int mpaID;
    public Vector3 pos;
    public float normalRange;
    public float battleRange;
    public List<MonsterData> monsters;
    public Vector3 patrolPos;
}

public class SkillActionCfg : BaseData
{
    public float delayTime; 
    public float radius;
    public float angle;
}

public class TaskDefine : BaseData
{
    public string taskName;
    public TaskType taskType;
    public int preTaskID;   
    public bool isAutoGetNextTask;

    public int acceptNpcID;
    public int submitNpcID;
    public int accTalkID;
    public int subTalkID;

    public int limitLevel;

    public int targetID;
    public int targetCount;
    public Vector3 targetPos;
    public string description;

    public int exp;
    public int coin;
    public int diomand;
}


public class BattleProps
{
    public float atkDis;
    public int hp;
    public int ad;
    public int ap;
    public int addef;
    public int apdef;
    public int dodge;
    public int pierce;
    public int critical;
}


