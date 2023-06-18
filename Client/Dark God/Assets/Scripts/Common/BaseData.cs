using PEProtocol;
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
    public int entityID;
    public string skillName;
    public string animName;
    public string fxName;
    public SkillType skillType;
    public int nextComboID;
    public int comboCheckTime;
    public bool isCollide;
    public bool isBreak;
    public float powerCost;
    public DmgType dmgType;
    public List<int> skillActionLst;
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
    public MonsterType mType;
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

public class GameItemCfg : BaseData
{
    public string name;
    public string des;
    public BagItemType ItemType;
    public EquipmentType equipmentType;
    public ItemFunction funcType;
    public float funcNum;
    public float duration;
    public string objPath;
    public string iconPath;
    public bool useWithoutBattle;
}

public class BagItemData
{
    public GameItemCfg cfg;

    public BagItemData(GameItemCfg cfg, int count)
    {
        this.cfg = cfg;
        this.count = count;
    } 
    public bool CanUse
    {
        get
        {
            return cfg.useWithoutBattle | BattleSystem.IsEnterBattle;
        }
    }

    public int count;
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
    public int lv;
}

public class SkillActionData : BaseData
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
    public string[] item;
}


public class BattleProps
{
    public int power;
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


