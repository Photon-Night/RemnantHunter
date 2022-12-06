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

    //UIAudio
    public const string UIClickBtn = "uiClickBtn";
    public const string UILoginBtn = "uiLoginBtn";
    public const string UIExtenBtn = "uiExtenBtn";
    public const string UIOpenPage = "uiOpenPage";
    public const string FBItem = "fbitem";

    //ScreenStandardWidthAndHeight
    public const int ScreenStandardWidth = 1920;
    public const int ScreenStandardHeight = 1080;

    //ҡ�˵�ҡ����Χ
    public const int ScreenOPDis = 90;

    public const int PlayerMoveSpeed = 8;
    public const int MonsterMoveSpeed = 4;

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
    public const string ColorEnd = "</color>";

    public const int BuyCoin = 0;
    public const int BuyPower = 1;

    public const int ComboSpace = 500;

    public const int DieAniLength = 5000;

    public static string Color(string str, string color)
    {
        string result = color + str + ColorEnd;
        return result;
    }

    public enum DmgType
    {
        None = 0, 
        AD = 1,
        AP = 2,
    }
}
