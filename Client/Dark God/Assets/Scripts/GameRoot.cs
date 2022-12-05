using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;

public class GameRoot : MonoSingleton<GameRoot>
{
    public LoadingWin loadingWin;  
    public DynamicWin dynamicWin;

    // Start is called before the first frame update
    void Start()
    {
        ClearUIRoot();
        Init();
        PECommon.Log("Game Start");
    }

    private void Init()
    {
        //�����ʼ������
        NetService.Instance.ServiceInit();
        ResService.Instance.ServiceInit();
        AudioService.Instance.ServiceInit();
        TimerService.Instance.ServiceInit();

        MainCitySystem.Instance.InitSystem();
        MissionSystem.Instance.InitSystem();
        BattleSystem.Instance.InitSystem();

        LoginSystem.Instance.InitSystem();
        LoginSystem.Instance.OnLoginEnter();

        dynamicWin.SetWinState();
    }

     private void ClearUIRoot()
    {
        Transform canvas = this.transform.Find("Canvas");
        for(int i = 0; i < canvas.childCount; i++)
        {
            canvas.GetChild(i).gameObject.SetActive(false);
        }

        
    }

    public static void AddTips(string tip)
    {
        Instance.dynamicWin.AddTips(tip);
    }

    private PlayerData playerData = null;
    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }
    }

    public void SetPlayerData(RspLogin data)
    {
        playerData = data.playerData;
    }

    public void SetPlayerName(string name)
    {
        PlayerData.name = name;
    }

    public void SetPlayerDataByGuide(RspGuide data)
    {
        playerData.coin = data.coin;
        playerData.exp = data.exp;
        playerData.lv = data.lv;
        playerData.guideid = data.guideid;
    }

    public void SetPlayerDataByStrong(RspStrong data)
    {
        playerData.coin = data.coin;
        playerData.crystal = data.crystal;
        playerData.hp = data.hp;
        playerData.ad = data.ad;
        playerData.ap = data.ap;
        playerData.addef = data.addef;
        playerData.apdef = data.apdef;

        playerData.strong = data.strong;

    }

    public void SetPlayerDataByBuy(RspBuy data)
    {
        PlayerData.coin = data.coin;
        PlayerData.diamond = data.diamond;
        PlayerData.power = data.power;
    }

    public void SetPlayerDataByPower(PushPower data)
    {
        PlayerData.power = data.power;
    }

    public void SetPlayerDataByTakeTaskReward(RspTakeTaskReward data)
    {
        PlayerData.coin = data.coin;
        PlayerData.lv = data.lv;
        PlayerData.exp = data.exp;
        PlayerData.task = data.taskArr;
    }

    public void SetPlayerDataByTaskPrgs(PushTaskPrgs data)
    {
        PlayerData.task = data.taskArr;
    }

    public void SetPlayerDataByMissionEnter(RspMissionEnter data)
    {
        PlayerData.power = data.power;
    }

    public void AddHpUIItem(string name, int hp, Transform trans)
    {
        dynamicWin.AddHpUIItem(name, hp, trans);
    }

    public void RemoveHpUIItem(string name)
    {
        dynamicWin.ReMoveHpUIItem(name);
    }
  
}
