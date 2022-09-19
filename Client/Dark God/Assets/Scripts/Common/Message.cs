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

    //UIAudio
    public const string UIClickBtn = "uiClickBtn";
    public const string UILoginBtn = "uiLoginBtn";
    public const string UIExtenBtn = "uiExtenBtn";
    public const string UIOpenPage = "uiOpenPage";
    public const string FBItem = "fbitem";

    //ScreenStandardWidthAndHeight
    public const int ScreenStandardWidth = 1920;
    public const int ScreenStandardHeight = 1080;

    //“°∏Àµ„“°∂Ø∑∂Œß
    public const int ScreenOPDis = 90;

    public const int PlayerMoveSpeed = 8;
    public const int MonsterMoveSpeed = 4;

    public const float AccelerSpeed = 7;

    public const int BlendIdle = 0;
    public const int BlendWalk = 1;

    //npcID
    public const int NPCWiseMan = 0;
    public const int NPCGeneral = 1;
    public const int NPCArtisan = 2;
    public const int NPCTrader = 3;

    //Color
    public const string ColorRed = "<color=#FF0000FF>";
    public const string ColorGreen = "<color=00FF00FF>";
    public const string ColorBlue = "<color=#00B4FFFF>";
    public const string ColorYellow = "<color=#FFFF00FF>";
    public const string ColorEnd = "</color>";
    
    public static string Color(string str, string color)
    {
        string result = color + str + ColorEnd;
        return result;
    }
}
