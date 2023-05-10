using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message
{
    //SceneName
    public const string SceneLogin = "SceneLogin";
    public const string SceneMainCity = "SceneMainCity";

    public const int MainCityMapID = 10000;

    //GameBGM
    public const string BGLogin = "bgLogin";
    public const string BGMMainCity = "bgMainCity";
    public const string BGHuangYe = "bgHuangYe";

    //Timeline
    public const string TimelineLogin = "Login";

    //UIAudio
    public const string UIClickBtn = "uiClickBtn";
    public const string UILoginBtn = "uiLoginBtn";
    public const string UIExtenBtn = "uiExtenBtn";
    public const string UIOpenPage = "uiOpenPage";
    public const string FBItem = "fbitem";
    public const string PlayerHit = "assassin_Hit";
    public const string FBLose = "fblose";
    public const string FBWin = "fbwin";
    public const string Hit1 = "Hit_1";
    public const string Hit2 = "Hit_2";
    public const string SwordWave_Heavy = "SwordWave_4";
    public const string SwordWave_Normal = "SwordWave_2";

    //ScreenStandardWidthAndHeight
    public const int ScreenStandardWidth = 1920;
    public const int ScreenStandardHeight = 1080;

    //ҡ�˵�ҡ����Χ
    public const int ScreenOPDis = 90;

    public const int PlayerMoveSpeed = 5;
    public const int MonsterMoveSpeed = 3;

    public const float AccelerSpeed = 7f;
    public const float AccelerHpSpeed = .3f;

    public const int BlendIdle = 0;
    public const int BlendWalk = 1;

    //action code
    public const int ActionSkill1 = 1;
    public const int ActionNormal = -1;
    public const int ActionBorn = 0;
    public const int ActionDie = 100;
    public const int ActionHit = 101;

    //npcID
    public const int NPCWiseMan = 0;
    public const int NPCGeneral = 1;
    public const int NPCArtisan = 2;
    public const int NPCTrader = 3;

    //Color
    public const string ColorRed = "<color=#FF0000FF>";
    public const string ColorGreen = "<color=#00FF00FF>";
    public const string ColorBlue = "<color=#0000FF>";
    public const string ColorYellow = "<color=#FFFF00FF>";
    public const string ColorOrange = "<color=#FF6900FF>";
    public const string ColorEnd = "</color>";

    public const int BuyCoin = 0;
    public const int BuyPower = 1;


    //战斗时间数据
    public const int ComboSpace = 500;
    public const float AICheckTimeSpace = 2f;
    public const float AIAtkTimeSpace = 2f;
    public const float SkillPointRecoverTime = 5f;


    //范围数据
    public const float NPCCheckRange = 5f;

    public const int DieAniLength = 5000;

    public const int Layer_Monster = 8;

    public const int SkillPointCount = 5;

    public const int RollPowerCost = 10;

    #region EventName 
    #endregion

    public static string Color(string str, string color)
    {
        string result = color + str + ColorEnd;
        return result;
    }

   

  

   

}

public enum EntityType
{
    None = 0,
    Player = 1,
    Monster = 2,
}

public enum EntityState
{
    None = 0,
    BatiState = 1,
}

public enum MonsterType
{
    None = 0,
    Normal = 1,
    Boss = 2,
}
public enum TalkType
{
    None = 0,
    Talk = 1,
    Answer = 2,
}

public enum AttackType
{
    None = 0,
    Heavy = 1,
    Normal = 2,
}
public enum DmgType
{
    None = 0,
    AD = 1,
    AP = 2,
}

public enum EventNode
{
    None = 0,
    Root = 100,
    Group_Battle = 101,
    Group_UI = 102,
    Group_NPC = 103,
    Group_Task = 104,

    Event_OnSetUIWinState = 200,
    Event_OnKillMonster = 201,
    Event_OnTalkToNPC = 202,
    Event_OnTakeDamage = 203,
    Event_OnFinishTask = 204,
    Event_OnNPCTaskStatusChange = 205,
    Event_OnOverTalk = 206,
    Event_OnPlayerCloseToNpc = 207,
    Event_OnPlayerFarToNpc = 208,
    Event_OnBattleEnd = 209,
    Event_OnBattleStart = 210,
    Event_OnGetBagItem = 211,
    Event_OnChangeEquipment = 212,
}
public enum NPCFunction
{
    None = 0,
    OpenTaskWin = 1,
    OpenStrongWin = 2,
    OpenMissionWin = 3,
}

public enum MenuFunction
{
    None = 0,
    Task = 1,
    Mission = 2,
    Make = 3,
    Strong = 4,
    Bag = 5,
}

public enum SkillType
{
    None = 0,
    Normal = 1,
    Combo = 2,

}


public enum ItemFunction
{
    None   = 0,
    Power  = 1,
    Health = 2,
    ADAtk  = 3,
    Stamina= 4,
    ADDef  = 5,
    Dodge  = 6,
    APAtk  = 7,
    APDef  = 8,
}
